using Microsoft.Practices.Unity;
using NHibernate.Bytecode.Lightweight;
using NHibernate.Properties;
using System;

namespace Unity.Bytecode
{
    /// <summary>
    /// Reflection optimizer implementation.
    /// </summary>
    public class UnityReflectionOptimizer : ReflectionOptimizer
    {
        private readonly IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityReflectionOptimizer"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="mappedType">The type being mapped.</param>
        /// <param name="getters">The getters.</param>
        /// <param name="setters">The setters.</param>
        public UnityReflectionOptimizer(IUnityContainer container, Type mappedType, IGetter[] getters, ISetter[] setters)
            : base(mappedType, getters, setters)
        {
            this.container = container;
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <returns>The instance.</returns>
        public override object CreateInstance()
        {
            if (container.IsRegistered(mappedType))
                return container.Resolve(mappedType);

            //return _container.IsRegisteredWithName(mappedType.FullName, mappedType)
            //           ? _container.ResolveNamed(mappedType.FullName, mappedType)
            //           : base.CreateInstance();

            return base.CreateInstance();
        }

        /// <summary>
        /// Determines if an exception should be thrown for when no default constructor is found.
        /// </summary>
        /// <param name="type">The type.</param>
        protected override void ThrowExceptionForNoDefaultCtor(Type type) { }
    }
}