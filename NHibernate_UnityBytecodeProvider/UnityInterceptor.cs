using NHibernate;
using System.Linq;

namespace Unity.Bytecode
{
    public class UnityInterceptor : EmptyInterceptor
    {
        public override string GetEntityName(object entity)
        {
            var interceptedType = entity.GetType().GetInterfaces().Where(i => i.IsGenericType
                                                                         && i.GetGenericTypeDefinition() == typeof(IIntercepted<>)
                                                                         && i.GetGenericArguments().Count() == 1).SingleOrDefault();
            if (interceptedType != null)
            {
                return interceptedType.GetGenericArguments()[0].FullName;
            }
            else
            {
                return base.GetEntityName(entity);
            }

        }
    }
}
