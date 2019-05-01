using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace GraphQL_Core.Tests
{
    [TestClass]
    public class Initializer
    {
        public IServiceCollection services { get; set; }
        public ServiceProvider provider { get; set; }
        public IDependencyResolver resolver { get; set; }

        [TestInitialize()]
        public void Init()
        {
            services = new ServiceCollection();
            provider = services.BuildServiceProvider();
            resolver = new FuncDependencyResolver(type =>
            {
                var service = services.Where(svc => svc.ServiceType == type).FirstOrDefault();
                if (service is null || service.ImplementationInstance is null)
                {
                    return provider.GetService(type);
                }

                return service.ImplementationInstance;
            });
        }
    }
}
