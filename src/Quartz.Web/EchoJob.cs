namespace Quartz.Web
{
    using System.Threading.Tasks;
    using EnsureThat;
    using Microsoft.Extensions.Logging;
    using Quartz;

    [DisallowConcurrentExecution]
    public class EchoJob : IJob
    {
        private readonly ILogger<EchoJob> logger;

        public EchoJob(ILogger<EchoJob> logger)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));

            this.logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            this.logger.LogInformation($"echo from job: {context.JobDetail.Key}");
            return Task.CompletedTask;
        }
    }
}
