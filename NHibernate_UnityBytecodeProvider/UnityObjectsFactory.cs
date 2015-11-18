using Microsoft.Practices.Unity;
using NHibernate.Bytecode;
using System;

namespace Unity.Bytecode
{
    /// <summary>
    /// Interface for instanciate all NHibernate objects.
    /// </summary>
    public class UnityObjectsFactory : IObjectsFactory
    {
        private readonly IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityObjectsFactory"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public UnityObjectsFactory(IUnityContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Creates an instance of the specified type.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <returns>
        /// A reference to the created object.
        /// </returns>
        public object CreateInstance(Type type)
        {
            if (container.IsRegistered(type))
            {
                return container.Resolve(type);
            }
            else
            {
                return Activator.CreateInstance(type);
            }
        }

        /// <summary>
        /// Creates an instance of the specified type.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="nonPublic">true if a public or nonpublic default constructor can match; false if only a public default constructor can match.</param>
        /// <returns>
        /// A reference to the created object.
        /// </returns>
        public object CreateInstance(Type type, bool nonPublic)
        {
            return Activator.CreateInstance(type, nonPublic);
        }

        /// <summary>
        /// Creates an instance of the specified type using the constructor
        /// that best matches the specified parameters.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="ctorArgs">An array of constructor arguments.</param>
        /// <returns>
        /// A reference to the created object.
        /// </returns>
        public object CreateInstance(Type type, params object[] ctorArgs)
        {
            return Activator.CreateInstance(type, ctorArgs);
        }
    }
}