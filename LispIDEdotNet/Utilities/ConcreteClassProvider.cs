#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

#endregion

namespace AbstractBaseClass {

    // Place this attribute on any abstract class where you want to declare
    // a concrete version of that class at design time.
    [AttributeUsage(AttributeTargets.Class)]
    internal class ConcreteClassAttribute : Attribute {
        Type _concreteType;
        public ConcreteClassAttribute(Type concreteType) {
            _concreteType = concreteType;
        }

        public Type ConcreteType { get { return _concreteType; } }
    }

    // Here is our type description provider.  This is the same provider
    // as ConcreteClassProvider except that it uses the ConcreteClassAttribute
    // to find the concrete class.
    internal class ConcreteClassProvider : TypeDescriptionProvider {
        Type _abstractType;
        Type _concreteType;

        public ConcreteClassProvider() : base(TypeDescriptor.GetProvider(typeof(Form))) { }

        // This method locates the abstract and concrete
        // types we should be returning.
        private void EnsureTypes(Type objectType) {
            if (_abstractType == null) {
                Type searchType = objectType;
                while (_abstractType == null && searchType != null && searchType != typeof(Object)) {
                    foreach (ConcreteClassAttribute cca in searchType.GetCustomAttributes(typeof(ConcreteClassAttribute), false)) {
                        _abstractType = searchType;
                        _concreteType = cca.ConcreteType;
                        break;
                    }
                    searchType = searchType.BaseType;
                }

                if (_abstractType == null) {
                    // If this happens, it means that someone added
                    // this provider to a class but did not add
                    // a ConcreteTypeAttribute
                    throw new InvalidOperationException(string.Format("No ConcreteClassAttribute was found on {0} or any of its subtypes.", objectType));
                }
            }
        }

        // Tell anyone who reflects on us that the concrete form is the
        // form to reflect against, not the abstract form. This way, the
        // designer does not see an abstract class.
        public override Type GetReflectionType(Type objectType, object instance) {
            EnsureTypes(objectType);
            if (objectType == _abstractType) {
                return _concreteType;
            }
            return base.GetReflectionType(objectType, instance);
        }


        // If the designer tries to create an instance of AbstractForm, we override 
        // it here to create a concerete form instead.
        public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args) {
            EnsureTypes(objectType);
            if (objectType == _abstractType) {
                objectType = _concreteType;
            }

            return base.CreateInstance(provider, objectType, argTypes, args);
        }
    }
 }
