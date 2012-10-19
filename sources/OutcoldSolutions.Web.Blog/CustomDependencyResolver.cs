// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    internal class CustomDependencyResolver : IDependencyResolver
    {
        private readonly IDependencyResolverContainer container;

        public CustomDependencyResolver(IDependencyResolverContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            if (this.container.IsRegistered(serviceType))
            {
                return this.container.Resolve(serviceType);
            }

            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (this.container.IsRegistered(serviceType))
            {
                yield return this.container.Resolve(serviceType);
            }
        }
    }
}