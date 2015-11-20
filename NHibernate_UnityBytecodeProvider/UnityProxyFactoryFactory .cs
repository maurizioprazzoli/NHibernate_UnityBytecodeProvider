using NHibernate.Bytecode;
using NHibernate.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate_UnityBytecodeProvider
{
    public class UnityProxyFactoryFactory : IProxyFactoryFactory
    {
        public IProxyValidator ProxyValidator
        {
            get { return new DynProxyTypeValidator(); }
        }

        public IProxyFactory BuildProxyFactory()
        {
            return new UnityProxyFactory();
        }

        public bool IsInstrumented(System.Type entityClass)
        {
            return false;
        }

        public bool IsProxy(object entity)
        {
            return entity is INHibernateProxy;
        }
    }
}
