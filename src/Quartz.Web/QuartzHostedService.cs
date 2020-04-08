namespace Quartz.Web
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EnsureThat;
    using Microsoft.Extensions.Hosting;
    using Quartz;
    using Quartz.Spi;

    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IJobFactory jobFactory;
        private readonly IEnumerable<JobSchedule> jobSchedules;

        public QuartzHostedService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IEnumerable<JobSchedule> jobSchedules)
        {
            EnsureArg.IsNotNull(schedulerFactory, nameof(schedulerFactory));
            EnsureArg.IsNotNull(jobFactory, nameof(jobFactory));

            this.schedulerFactory = schedulerFactory;
            this.jobSchedules = jobSchedules;
            this.jobFactory = jobFactory;
        }

        public IScheduler Scheduler { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.Scheduler = await this.schedulerFactory.GetScheduler(cancellationToken).ConfigureAwait(false);
            this.Scheduler.JobFactory = this.jobFactory;

            if (this.jobSchedules?.Any() == true)
            {
                foreach (var jobSchedule in this.jobSchedules)
                {
                    var job = CreateJob(jobSchedule);
                    var trigger = CreateTrigger(jobSchedule);

                    await this.Scheduler.ScheduleJob(job, trigger, cancellationToken).ConfigureAwait(false);
                }
            }

            await this.Scheduler.Start(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await (this.Scheduler?.Shutdown(cancellationToken)).ConfigureAwait(false);
        }

        private static ITrigger CreateTrigger(JobSchedule schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.JobType.FullName}.trigger")
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .Build();
        }

        private static IJobDetail CreateJob(JobSchedule schedule)
        {
            var jobType = schedule.JobType;
            return JobBuilder
                .Create(jobType)
                .WithIdentity(jobType.FullName)
                .WithDescription(jobType.Name)
                .Build();
        }
    }
}
