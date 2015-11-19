using NHibernate.Bytecode;
using NHibernate.Proxy;

namespace TestProxy
{
    public class AEEDefaultProxyFactoryFactory : IProxyFactoryFactory
    {
        #region IProxyFactoryFactory Members

        public IProxyFactory BuildProxyFactory()
        {
            return new AEEDefaultProxyFactory();
        }

        public IProxyValidator ProxyValidator
        {
            get { return new DynProxyTypeValidator(); }
        }

        public bool IsInstrumented(System.Type entityClass)
        {
            return true;
        }

        public bool IsProxy(object entity)
        {
            return entity is INHibernateProxy;
        }

        #endregion
    }
}