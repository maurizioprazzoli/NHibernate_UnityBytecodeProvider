using Microsoft.Practices.Unity.InterceptionExtension;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Proxy;
using System;
using NHibernate_UnityBytecodeProvider;
using NHibernate.Bytecode.Unity;
using Microsoft.Practices.Unity;

namespace Unity.Bytecode
{
    public class UnityProxyFactory : AbstractProxyFactory
    {
        IUnityContainer container;

        private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(UnityProxyFactory));

        public UnityProxyFactory(IUnityContainer container)
        {
            this.container = container;
        }

        public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            try
            {
                var initializer = new UnityLazyInitializer(this.EntityName, this.PersistentClass, id, this.GetIdentifierMethod, this.SetIdentifierMethod, this.ComponentIdType, session, this.OverridesEquals);

                var generatedProxy = Intercept.NewInstanceWithAdditionalInterfaces(
                   this.IsClassProxy ? this.PersistentClass : this.Interfaces[0],
                   new VirtualMethodInterceptor(),
                   new[] { initializer },
                   this.Interfaces);

                initializer.SetConstructed();

                return (INHibernateProxy)generatedProxy;
            }
            catch (Exception e)
            {
                Log.Error("Creating a proxy instance failed", e);

                throw new HibernateException("Creating a proxy instance failed", e);
            }
        }

        public override object GetFieldInterceptionProxy(object instanceToWrap)
        {
            AddAdditionalInterface(typeof(IFieldInterceptorAccessor));

            var proxy = container.Resolve(KeyType, EntityName);

            (proxy as IInterceptingProxy).AddInterceptionBehavior(new UnityLazyFieldInterceptor(instanceToWrap, Interfaces));

            return proxy;
        }

        protected System.Type KeyType
        {
            get { return IsClassProxy ? PersistentClass : typeof(object); }
        }

        private void AddAdditionalInterface(System.Type @interface)
        {
            new AdditionalInterface(@interface).AddPolicies(null, KeyType, EntityName, container.Configure<NHibernateProxyUnityExtension>().Policies);
        }

    }
}
