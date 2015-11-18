using Microsoft.Practices.Unity.InterceptionExtension;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Proxy.Poco;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernate_UnityBytecodeProvider
{
    [Serializable]
    public class UnityLazyInitializer : BasicLazyInitializer, IInterceptionBehavior
    {
        private static readonly MethodInfo ExceptionInternalPreserveStackTrace = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

        private bool constructed;

        public UnityLazyInitializer(string entityName, Type persistentClass, object id, MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType, ISessionImplementor session)
            : base(entityName, persistentClass, id, getIdentifierMethod, setIdentifierMethod, componentIdType, session)
        {
        }

        public bool WillExecute
        {
            get { return true; }
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return new[] { typeof(INHibernateProxy) };
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            try
            {
                if (this.constructed)
                {
                    var args = new object[input.Arguments.Count];
                    input.Arguments.CopyTo(args, 0);

                    var result = base.Invoke((MethodInfo)input.MethodBase, args, input.Target);

                    if (result == AbstractLazyInitializer.InvokeImplementation)
                    {
                        return input.CreateMethodReturn(input.MethodBase.Invoke(this.GetImplementation(), args));
                    }

                    return input.CreateMethodReturn(result);
                }

                return input.CreateMethodReturn(null);
            }
            catch (TargetInvocationException tie)
            {
                ExceptionInternalPreserveStackTrace.Invoke(tie.InnerException, new object[0]);

                throw tie.InnerException;
            }
        }

        public void SetConstructed()
        {
            this.constructed = true;
        }
    }
}
