using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LispIDEdotNet.Utilities.Configuration
{
    [ConfigurationCollection(typeof(OpenDocumentElement), CollectionType = ConfigurationElementCollectionType.BasicMap, 
        AddItemName = "OpenDocument")]
    class OpenDocumentsCollection : ConfigurationElementCollection
    {
        #region Fields

        private static ConfigurationPropertyCollection properties;

        #endregion Fields

        #region Properties

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override string ElementName
        {
            get
            {
                return "OpenDocument";
            }
        }

        #endregion Properties

        #region Constructors

        static OpenDocumentsCollection()
        {
            properties = new ConfigurationPropertyCollection();
        }

        public OpenDocumentsCollection()
        {}

        #endregion Constructors

        #region Indexers

        public OpenDocumentElement this[int index]
        {
            get
            {
                return (OpenDocumentElement)base.BaseGet(index);
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public OpenDocumentElement this[string name]
        {
            get { return (OpenDocumentElement)base.BaseGet(name); }
        }

        #endregion Indexers

        #region Methods

        protected override ConfigurationElement CreateNewElement()
        {
            return new OpenDocumentElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as OpenDocumentElement).Name;
        }

        public void Add(OpenDocumentElement document)
        {
            base.BaseAdd(document);
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void Remove(OpenDocumentElement document)
        {
            base.BaseRemove(GetElementKey(document));
        }

        public void Clear()
        {
            base.BaseClear();
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public string GetKey(int index)
        {
            return (string)base.BaseGetKey(index);
        }

        #endregion Methods
    }
}
