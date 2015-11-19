using Microsoft.Practices.Unity;
using NHibernate.Bytecode;
using NHibernate.Proxy;
using Unity.Bytecode;

namespace NHibernate_UnityBytecodeProvider
{
    public class UnityProxyFactoryFactory : IProxyFactoryFactory
    {
        IUnityContainer container;

        public UnityProxyFactoryFactory(IUnityContainer container)
        {
            this.container = container;
        }

        public IProxyValidator ProxyValidator
        {
            get { return new DynProxyTypeValidator(); }
        }

        public IProxyFactory BuildProxyFactory()
        {
            return new UnityProxyFactory(container);
        }

        public bool IsInstrumented(System.Type entityClass)
        {
            return true;
        }

        public bool IsProxy(object entity)
        {
            return entity is INHibernateProxy;
        }
    }
}
