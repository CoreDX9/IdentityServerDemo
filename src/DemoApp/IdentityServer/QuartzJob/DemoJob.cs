using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.QuartzJob
{
    public class DemoJob : IJob
    {
        private readonly ILogger<DemoJob> _logger;
        public DemoJob(ILogger<DemoJob> logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => _logger.LogInformation($"Demo Job Executed on {DateTimeOffset.Now} !"));
        }
    }

    // Quartz.Net启动后注册job和trigger
    public class QuartzManager
    {
        public IScheduler _scheduler { get; set; }

        private readonly ILogger _logger;
        private readonly IJobFactory jobfactory;
        public QuartzManager(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<QuartzManager>();
            jobfactory = new DependencyInjectionSupportedJobFactory(serviceProvider);
            var schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            _scheduler.JobFactory = jobfactory;
        }

        public void Start()
        {
            _logger.LogInformation("Schedule job load as application start.");
            _scheduler.Start().Wait();

            var demoJob = JobBuilder.Create<DemoJob>()
               .WithIdentity("DemoJob")
               .Build();

            var demoJobTrigger = TriggerBuilder.Create()
                .WithIdentity("DemoJob")
                .StartNow()
                // 每分钟执行一次
                .WithCronSchedule("0 * * * * ?")      // Seconds,Minutes,Hours，Day-of-Month，Month，Day-of-Week，Year（optional field）
                .Build();
            _scheduler.ScheduleJob(demoJob, demoJobTrigger).Wait();

            _scheduler.TriggerJob(new JobKey("DemoJob"));
        }

        public void Stop()
        {
            if (_scheduler == null)
            {
                return;
            }

            if (_scheduler.Shutdown(waitForJobsToComplete: true).Wait(30000))
                _scheduler = null;
            else
            {
            }
            _logger.LogCritical("Schedule job upload as application stopped");
        }
    }

    public class DependencyInjectionSupportedJobFactory : IJobFactory
    {
        protected readonly IServiceProvider _serviceProvider;

        public DependencyInjectionSupportedJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //Called by the scheduler at the time of the trigger firing, in order to produce
        //     a Quartz.IJob instance on which to call Execute.
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        // Allows the job factory to destroy/cleanup the job if needed.
        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}
