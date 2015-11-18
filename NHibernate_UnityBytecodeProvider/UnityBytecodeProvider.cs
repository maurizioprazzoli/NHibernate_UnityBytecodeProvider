using Microsoft.Practices.Unity;
using NHibernate.Bytecode;
using NHibernate.Properties;
using System;

namespace Unity.Bytecode
{
    /// <summary>
    /// Unity ByteCode provider implementation.
    /// this is implementation of Bytecode Provider that integrate Unity Container
    /// with NHibernate.
    /// </summary>
    public class UnityBytecodeProvider : IBytecodeProvider
    {
        private readonly IUnityContainer container;
        private readonly IProxyFactoryFactory proxyFactoryFactory;
        private readonly ICollectionTypeFactory collectionTypeFactory;
        private readonly UnityObjectsFactory objectsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityBytecodeProvider"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="proxyFactoryFactory">The proxy factory factory.</param>
        /// <param name="collectionTypeFactory">The collection type factory.</param>
        public UnityBytecodeProvider(IUnityContainer container, IProxyFactoryFactory proxyFactoryFactory, ICollectionTypeFactory collectionTypeFactory)
        {
            this.container = container;
            this.proxyFactoryFactory = proxyFactoryFactory;
            this.collectionTypeFactory = collectionTypeFactory;
            this.objectsFactory = new UnityObjectsFactory(container);
        }

        /// <summary>
        /// Retrieve the <see cref="IReflectionOptimizer" /> delegate for this provider
        /// capable of generating reflection optimization components.
        /// </summary>
        /// <param name="clazz">The class to be reflected upon.</param>
        /// <param name="getters">All property getters to be accessed via reflection.</param>
        /// <param name="setters">All property setters to be accessed via reflection.</param>
        /// <returns>
        /// The reflection optimization delegate.
        /// </returns>
        public IReflectionOptimizer GetReflectionOptimizer(Type clazz, IGetter[] getters, ISetter[] setters)
        {
            return new UnityReflectionOptimizer(container, clazz, getters, setters);
        }

        /// <summary>
        /// NHibernate's object instaciator.
        /// </summary>
        /// <remarks>
        /// For entities <see cref="IReflectionOptimizer" /> and its implementations.
        /// </remarks>
        public IObjectsFactory ObjectsFactory
        {
            get { return objectsFactory; }
        }

        /// <summary>
        /// The specific factory for this provider capable of
        /// generating run-time proxies for lazy-loading purposes.
        /// </summary>
        public IProxyFactoryFactory ProxyFactoryFactory
        {
            get { return proxyFactoryFactory; }
        }

        /// <summary>
        /// Instanciator of NHibernate's collections default types.
        /// </summary>
        public ICollectionTypeFactory CollectionTypeFactory
        {
            get { return collectionTypeFactory; }
        }
    }
}