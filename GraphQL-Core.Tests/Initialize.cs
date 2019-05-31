using GraphQL;
using GraphQL.Types;
using GraphQLCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
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
        public IDocumentExecuter executer { get; set; }

        [TestInitialize()]
        public void Init()
        {
            services = new ServiceCollection();
            provider = services.BuildServiceProvider();
            executer = new DocumentExecuter();
            resolver = new FuncDependencyResolver(type =>
            {
                var service = services.Where(svc => svc.ServiceType == type).FirstOrDefault();
                if (service is null || service.ImplementationInstance is null)
                {
                    return provider.GetService(type);
                }

                return service.ImplementationInstance;
            });
            GraphQLCoreTypeWrapperGenerator.Clear();
        }

        public (bool error, ExecutionResult) Ask(string query, string operation = null, JObject variables = null)
        {
            try
            {
                ISchema schema = resolver.Resolve<ISchema>();
                var inputs = variables?.ToInputs();

                var executionOptions = new ExecutionOptions()
                {
                    Schema = schema,
                    Query = query,
                    Inputs = inputs,
                    ExposeExceptions = true,
                    EnableMetrics = true
                };

                var resultTask = executer.ExecuteAsync(executionOptions);
                resultTask.Wait();
                var result = resultTask.Result;

                return (result.Errors != null && result.Errors.Any(), result);
            }
            catch(Exception e)
            {
                return (false, null);
            }
        }
    }
}
