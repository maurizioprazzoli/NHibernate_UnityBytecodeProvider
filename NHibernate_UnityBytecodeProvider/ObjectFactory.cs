using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Unity.Bytecode;

namespace NHibernate_UnityBytecodeProvider
{
    public static class AEEObjectFactory
    {
        public static IUnityContainer Container { get; set; }

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        internal static bool Contains(System.Type instanceType)
        {
            return Container.IsRegistered(instanceType);
        }

        internal static object Resolve(System.Type instanceType)
        {
            return Container.Resolve(instanceType);
        }

        internal static void RegisterTypeInterception(System.Type type)
        {
            // Construct closed generic interface starting from type to inject
            var closedTypeInterfaceToInject = typeof(IIntercepted<>).MakeGenericType(type);
            // Registed new type inro container injecting specific interface for eneable
            // tracking of injected objects
            Container.RegisterType(type,
                                   type,
                                   new InterceptionBehavior<PolicyInjectionBehavior>(),
                                   new Interceptor<VirtualMethodInterceptor>(),
                                   new AdditionalInterface(closedTypeInterfaceToInject));
        }
    }
}
