namespace Quartz.Web
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using EnsureThat;
    using Microsoft.Extensions.Logging;
    using Quartz;

    public abstract class JobBase : IJob
    {
        protected JobBase(ILoggerFactory loggerFactory)
        {
            EnsureArg.IsNotNull(loggerFactory, nameof(loggerFactory));

            this.Logger = loggerFactory.CreateLogger(this.GetType());
        }

        public ILogger Logger { get; }

        public virtual async Task Execute(IJobExecutionContext context)
        {
            try
            {
                EnsureArg.IsNotNull(context, nameof(context));

                this.Logger.LogInformation("job: processing (type={jobType}, id={jobId})", this.GetType().Name, context.FireInstanceId);
                var watch = Stopwatch.StartNew();

                await this.Process(context).ConfigureAwait(false);

                watch.Stop();
                this.Logger.LogInformation("job: processed (type={jobType}, id={jobId}) -> took {elapsed} ms", this.GetType().Name, context.FireInstanceId, watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "job: processing error (type={jobType}, id={jobId}): {errorMessage}", this.GetType().Name, context.FireInstanceId, ex.Message);
                throw;
            }
        }

        public abstract Task Process(IJobExecutionContext context);
    }
}
