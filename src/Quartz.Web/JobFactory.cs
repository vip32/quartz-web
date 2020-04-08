namespace Quartz.Web
{
    using System;
    using EnsureThat;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Spi;

    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            EnsureArg.IsNotNull(serviceProvider, nameof(serviceProvider));

            this.serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            EnsureArg.IsNotNull(bundle, nameof(bundle));

            return this.serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            // the DI container handles this
        }
    }
}
