namespace Quartz.Web
{
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the job scheduler
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddJobScheduling(this IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, ScopedJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddHostedService<QuartzHostedService>();

            return services;
        }

        /// <summary>
        /// Adds a scoped scheduled job
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="cronExpression">the cron expression: https://www.freeformatter.com/cron-expression-generator-quartz.html</param>
        /// <returns></returns>
        public static IServiceCollection AddScopedJob<T>(this IServiceCollection services, string cronExpression)
            where T : class, IJob
        {
            services.AddScoped<T>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(T),
                cronExpression: cronExpression));

            return services;
        }

        /// <summary>
        /// Adds a singleton scheduled job
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="cronExpression">the cron expression: https://www.freeformatter.com/cron-expression-generator-quartz.html</param>
        /// <returns></returns>
        public static IServiceCollection AddSingletonJob<T>(this IServiceCollection services, string cronExpression)
            where T : class, IJob
        {
            services.AddSingleton<T>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(T),
                cronExpression: cronExpression));

            return services;
        }
    }
}
