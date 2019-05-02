using System;
using System.Threading;
using System.Threading.Tasks;
using BinarApp.Core.Interfaces;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace BinarApp.Jobs
{
    public interface IDataCacheJobListener : IJobListener
    {
        event EventHandler<EventArgs> DataCachingExecuted;
    }

    public class DataCacheJobListener : IDataCacheJobListener
    {
        public DataCacheJobListener()
        {
            Name = "DataCacheJobListener";
        }

        public event EventHandler<EventArgs> DataCachingExecuted;

        public string Name { get; set; }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Raise Job Executed event
            OnDataCacheExecute();
            return Task.FromResult(0);
        }

        private void OnDataCacheExecute()
        {
            DataCachingExecuted?.Invoke(this, new EventArgs());
        }
    }

    public class JobScheduler : IJobScheduler
    {
        IDataCacheJobListener _jobListener;

        public JobScheduler(IDataCacheJobListener jobListener)
        {
            _jobListener = jobListener;
        }

        public async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail networkConnectionJob = JobBuilder.Create<NetworkConnectionJob>()
                .WithIdentity("NetworkConnectionJob", "BinarGroup")
                .RequestRecovery(true)
                .Build();

            IJobDetail dataCacheJob = JobBuilder.Create<DataCacheJob>()
                .WithIdentity("DataCacheJob", "BinarGroup")
                .RequestRecovery(true)
                .Build();

            // Trigger the job to run now, and then every 5 seconds
            ITrigger networkConnectionTrigger = TriggerBuilder.Create()
              .WithIdentity("NetworkConnectionTrigger", "BinarGroup")
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithIntervalInSeconds(5)
                  .RepeatForever())
              .Build();

            ITrigger dataCacheTrigger = TriggerBuilder.Create()
              .WithIdentity("DataCacheTrigger", "BinarGroup")
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithIntervalInHours(12)
                  .RepeatForever())
              .Build();

            //_jobListener = new DataCacheJobListener();
            //_jobListener.Name = "DataCacheJobListener";

            scheduler.ListenerManager.
                AddJobListener(_jobListener, KeyMatcher<JobKey>.KeyEquals(new JobKey("DataCacheJob", "BinarGroup")));

            await scheduler.ScheduleJob(networkConnectionJob, networkConnectionTrigger);
            await scheduler.ScheduleJob(dataCacheJob, dataCacheTrigger);
        }
    }
}
