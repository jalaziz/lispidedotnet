using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Windows.Forms;

// This class is a modified version of http://www.codeproject.com/KB/threads/SingleInstancingWithIpc.aspx
// Credit goes to Shy Agam for the inital idea. I simply modified the code to make it simpler and use
// .Net 3.5 classes.

namespace SingleInstancing
{
    /// <summary>
    /// Represents an object used to check for a previous instance of an application, and sending messages to it.
    /// </summary>
    public class SingleInstance : Form
    {
        #region Member Variables

        private Mutex singleInstanceMutex;
        private bool isFirstInstance;
        private NamedPipeServerStream serverStream;
        private NamedPipeClientStream clientStream;
        private IAsyncResult asyncResult;

        private delegate void CallBack(IAsyncResult result);

        private object _lock = new object();

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raised when the server (first instance) receives a message from a client (second instance).
        /// </summary>
        [Category("Single Instance"), Description("Occurs when a second instance sends a message to the first instance.")]
        public event EventHandler<MessageEventArgs> MessageReceived;

        #endregion Event Handlers

        #region Construction / Destruction

        /// <summary>
        /// Instantiates a new SingleInstance object.
        /// </summary>
        /// <exception cref="SingleInstancing.SingleInstancingException">A general error occurred while trying to instantiate the SingleInstance. See InnerException for more details.</exception>
        public SingleInstance()
        {
            try
            {
                string name = Application.ProductName + "_" + Application.ProductVersion;
                singleInstanceMutex = new Mutex(true, name, out isFirstInstance);

                if(isFirstInstance)
                {
                    serverStream = new NamedPipeServerStream(name, PipeDirection.In, 1, PipeTransmissionMode.Message,
                                                             PipeOptions.Asynchronous);
                }
                else
                {
                    clientStream = new NamedPipeClientStream(".", name, PipeDirection.Out);
                }
            } 
            catch (Exception ex)
            {
                throw new SingleInstancingException("Failed to instantiate a new SingleInstance object. See InnerException for more details.", ex);
            }
        }

        /// <summary>
        /// Releases all unmanaged resources used by the object.
        /// </summary>
        ~SingleInstance()
        {
            Dispose(false);
        }

        #region IDisposable Members

        /// <summary>
        /// Releases all unmanaged resources used by the object, and potentially releases managed resources.
        /// </summary>
        /// <param name="disposing">true to dispose of managed resources; otherwise false.</param>
        protected override void Dispose(bool disposing)
        {
            Debug.WriteLine("In Dispose", "Debug");
            Debug.WriteLine("Disposing: " + disposing, "Debug");
            Debug.WriteLine("Disposed: " + IsDisposed, "Debug");
            if (!IsDisposed)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (isFirstInstance && singleInstanceMutex != null)
                    {
                        Debug.WriteLine("Closing Mutex", "Debug");
                        singleInstanceMutex.Close();
                        singleInstanceMutex = null;
                    }

                    if (serverStream != null)
                    {
                        try
                        {
                            Debug.WriteLine("Closing Server Stream", "Debug");
                            serverStream.Close();
                            serverStream = null;
                        } catch { }
                    }
                    if (clientStream != null)
                    {
                        try
                        {
                            Debug.WriteLine("Closing Client Stream", "Debug");
                            clientStream.Close();
                            clientStream = null;
                        } catch { }
                    }
                }
            } 
            else
            {
                base.Dispose(disposing);
            }

            Debug.WriteLine("Diposed", "Debug");
        }

        #endregion

        #endregion

        #region Member Methods

        /// <summary>
        /// Indicates that the server (first instance) is ready to accept connections 
        /// from clients (second instances)
        /// </summary>
        protected void BeginWaitForConnection()
        {
            if (isFirstInstance)
            {
                Debug.WriteLine("Beginning WaitForConnection", "Debug");
                asyncResult = serverStream.BeginWaitForConnection(ClientConnected, null);
            }
        }

        /// <summary>
        /// The callback method for BeginWaitForConnection. This method is
        /// used to determine when a client has connected to the server.
        /// </summary>
        /// <param name="result">The <see cref="IAsyncResult"/> from the asynchronous operation.</param>
        private void ClientConnected(IAsyncResult result)
        {
            Debug.WriteLine("In CientConnected", "Debug");

            // Unfortunately, there is a severe lack of documentation on the 
            // asychronous features of the System.IO.Pipes namespace.
            // Thanks to http://eraser.heidi.ie/trac/changeset/749 and some
            // experimentation, I figured it out.
            try
            {
                // If the object is disposed, this callback was probably a result of the
                // the server stream being closed. I don't believe there's a way around this.
                if(IsDisposed)
                {
                    Debug.WriteLine("Leaving CientConnected: Disposed", "Debug");
                    return;
                }

                if(!serverStream.IsConnected)
                {
                    Debug.WriteLine("Ending WaitForConnection", "Debug");
                    // EndWaitForConnection must be called to actually establish
                    // the connection.
                    lock (_lock)
                    {
                        serverStream.EndWaitForConnection(asyncResult);
                    }
                }

                // We have to check again in case the stream had an error connecting.
                if(serverStream.IsConnected)
                {
                    if (this.InvokeRequired)
                    {
                        Debug.WriteLine("Invoke Required", "Debug");
                        this.Invoke(new CallBack(ClientConnected), result);
                        Debug.WriteLine("Invoke Method Called", "Debug");
                    } else
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        object message = bf.Deserialize(serverStream);
                        Debug.WriteLine("Raising MessageReceived", "Debug");
                        OnMessageReceived(new MessageEventArgs(message));
                        Debug.WriteLine("Done Raising MessageReceived", "Debug");

                        // Disconnect must be called to free the server for future
                        // connections.
                        serverStream.Disconnect();

                        Debug.WriteLine("Waiting for new connection", "Debug");
                        lock (_lock)
                        {
                            asyncResult = serverStream.BeginWaitForConnection(ClientConnected, null);
                        }
                    }
                }
                
                Debug.WriteLine("Leaving CientConnected", "Debug");
            }
            catch (NullReferenceException) { Debug.WriteLine("ClientConnected: NullReferenceException", "Error"); } 
            catch (ObjectDisposedException) { Debug.WriteLine("ClientConnected: ObjectDisposedException", "Error"); }
        }

        /// <summary>
        /// Sends a message to the first instance of the application.
        /// </summary>
        /// <param name="message">The message to send to the first instance of the application. The message must be serializable.</param>
        /// <exception cref="SingleInstancing.SingleInstancingException">The SingleInstance has failed to send the message to the first application instance. The first instance might have terminated.</exception>
        public void SendMessageToFirstInstance(object message)
        {
            Debug.WriteLine("In SendMessageToFirstInstance", "Debug");

            if (IsDisposed)
                throw new ObjectDisposedException("The SingleInstance object has already been disposed.");

            if (!message.GetType().IsSerializable)
                throw new SingleInstancingException(
                    "Failed to send message to the first instance of the application. The message is not serializable.");
                
            try
            {
                clientStream.Connect();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(clientStream, message);
            }
            catch (Exception ex)
            {
                throw new SingleInstancingException("Failed to send message to the first instance of the application. The first instance might have terminated.", ex);
            }

            Debug.WriteLine("Leaving SendMessageToFirstInstance", "Debug");
        }

        #endregion

        #region Events

        /// <summary>
        /// Raises the <see cref="MessageReceived"/> event.
        /// </summary>
        /// <param name="e">A <see cref="MessageEventArgs"/> that contains the event data. </param>
        protected virtual void OnMessageReceived(MessageEventArgs e)
        {
            if (MessageReceived != null)
                MessageReceived(this, e);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance of the application is the first instance.
        /// </summary>
        public bool IsFirstInstance
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException("The SingleInstance object has already been disposed.");
                
                return isFirstInstance;
            }
        }

        #endregion
    }
}