namespace Quartz.Web
{
    using System;
    using System.Threading.Tasks;
    using EnsureThat;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Spi;

    public partial class ScopedJobFactory : IJobFactory
    {
        private readonly IServiceProvider rootServiceProvider;

        public ScopedJobFactory(IServiceProvider rootServiceProvider)
        {
            EnsureArg.IsNotNull(rootServiceProvider, nameof(rootServiceProvider));

            this.rootServiceProvider = rootServiceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            EnsureArg.IsNotNull(bundle, nameof(bundle));

            var jobType = bundle.JobDetail.JobType;
            var scope = this.rootServiceProvider.CreateScope(); // Create a new scope for the job, this allows the job to be registered using .AddScoped<T>() which means we can use scoped dependencies (like database contexts)
            var job = (IJob)scope.ServiceProvider.GetRequiredService(jobType);

            return new ScopedJob(scope, job);
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }

        private class ScopedJob : IJob, IDisposable
        {
            private readonly IServiceScope scope;
            private readonly IJob innerJob;

            public ScopedJob(IServiceScope scope, IJob innerJob)
            {
                this.scope = scope;
                this.innerJob = innerJob;
            }

            public Task Execute(IJobExecutionContext context) => this.innerJob.Execute(context);

            public void Dispose()
            {
                this.scope?.Dispose();
                (this.innerJob as IDisposable)?.Dispose();
            }
        }
    }
}
