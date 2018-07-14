[assembly: WebActivator.PreApplicationStartMethod(typeof(CountingKs.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(CountingKs.App_Start.NinjectWebCommon), "Stop")]

namespace CountingKs.App_Start
{
    using Data;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using System;
    using System.Web;
    using System.Web.Http;
    using WebApiContrib.IoC.Ninject;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            //So what we are doing here, we are telling is someone wants ICountingKsRepository
            // which our foodscontroller demands in its constructor and now
            // ICountingKsRepository will call the TO <CountingKsRepository>
            // but CountingKsRepository in turn needs CountingKsContext
            // so we need to bind that too
            // CountingKsContext to CountingKsContext
            // See here we have not told anywhere to bind ICountingKsRepository to CountingKsContext
            // all this mapping and searching is done as per the reqiurement
            // as long as we hace a "TO" to it, it will be mapped and a new instance will be provided.
            kernel.Bind<Data.ICountingKsRepository>().To<CountingKsRepository>();
            kernel.Bind<CountingKsContext>().To<CountingKsContext>();
        }
    }
}
