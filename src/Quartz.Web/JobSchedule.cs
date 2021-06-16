namespace Quartz.Web
{
    using System;
    using EnsureThat;

    public class JobSchedule
    {
        public JobSchedule(Type jobType, string cronExpression)
        {
            EnsureArg.IsNotNull(jobType, nameof(jobType));

            this.JobType = jobType;
            this.CronExpression = cronExpression ?? "0/5 * * * * ?"; //every 5 seconds
        }

        public Type JobType { get; }

        public string CronExpression { get; }
    }
}