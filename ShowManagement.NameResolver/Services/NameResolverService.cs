using Microsoft.Practices.Unity;
using ShowManagement.NameResolver.Services.Activities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Services
{
    public class NameResolverService : INameResolverService
    {
        public NameResolverService(SettingsManager settingsManager, IShowManagementService service)
        {
            if (settingsManager == null)
            {
                throw new ArgumentNullException("settingsManager");
            }

            this.ItemRetryAttempts = settingsManager.ItemRetryAttempts;
            this.ItemRetryDurationSeconds = settingsManager.ItemRetryDurationSeconds;
        }

        public async Task Start()
        {
            try
            {
                Trace.WriteLine("Starting the Service Processes");

                await Task.Run(() => 
                    {
                        this.ProcessItemsFromQueue(this.InternalExecutionCTS.Token);
                        this.ProcessItemsFromRetryQueue(this.InternalExecutionCTS.Token);
                    });

                this.IsRunning = true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }            
        }

        public async Task Stop()
        {
            this.InternalExecutionCTS.Cancel();
            this.ExternalInfluenceCTS.Cancel();

            this.IsRunning = false;

            Trace.WriteLine("Stopping the Service Processes");
        }

        public async Task Add(string filePath)
        {
            await this.Add(new List<string> { filePath }, this.ItemRetryAttempts);
        }
        public async Task Add(string filePath, int retryAttempts)
        {
            await this.Add(new List<string> { filePath }, retryAttempts);
        }
        public async Task Add(IEnumerable<string> filePaths)
        {
            await this.Add(filePaths, this.ItemRetryAttempts);
        }
        public async Task Add(IEnumerable<string> filePaths, int retryAttempts)
        {
            await Task.Run(async () =>
                {
                    if (filePaths != null)
                    {
                        foreach (var filePath in filePaths)
                        {
                            var activity = new ResolveNameActivity(filePath, retryAttempts);

                            await this.AddToCollection(this.Queue, activity, this.ExternalInfluenceCTS.Token);
                        }
                    }
                });
        }

        public async Task Remove(string filePath)
        {
            await this.Remove(new List<string> { filePath });
        }
        public async Task Remove(IEnumerable<string> filePaths)
        {
            var removeFromQueueTask = Task.Factory.StartNew((x) =>
            {
                var paths = x as IEnumerable<string>;

                if (paths != null)
                {
                    var mockActivities = paths.Select(p => new ResolveNameActivity(p, 0));

                    var activitiesToCancel = this.Queue.Intersect(mockActivities, new ActivityComparer());

                    foreach (var activity in activitiesToCancel)
                    {
                        activity.Cancel();
                    }
                }
            }, filePaths, this.ExternalInfluenceCTS.Token);

            var removeFromRetryQueueTask = Task.Factory.StartNew((x) =>
            {
                var paths = x as IEnumerable<string>;

                if (paths != null)
                {
                    var mockActivities = paths.Select(p => new ResolveNameActivity(p, 0));

                    var activitiesToCancel = this.RetryQueue.Intersect(mockActivities, new ActivityComparer());

                    foreach (var activity in activitiesToCancel)
                    {
                        activity.Cancel();
                    }
                }
            }, filePaths, this.ExternalInfluenceCTS.Token);

            await Task.WhenAll(removeFromQueueTask, removeFromRetryQueueTask);
        }


        private async Task ProcessItemsFromQueue(CancellationToken cancellationToken)
        {
            foreach (var activity in this.Queue.GetConsumingEnumerable(cancellationToken))
            {
                if (!activity.IsCancelled)
                {
                    IActivity result = await activity.Perform();

                    if (result != null)
                    {
                        await this.AddToCollection(this.RetryQueue, result, cancellationToken);
                    }
                }
            }
        }
        private async Task ProcessItemsFromRetryQueue(CancellationToken cancellationToken)
        {
            foreach (var activity in this.RetryQueue.GetConsumingEnumerable(cancellationToken))
            {
                if (!activity.IsCancelled)
                {
                    var delta = activity.CreatedDtm.AddSeconds(this.ItemRetryDurationSeconds).Subtract(DateTime.Now);
                    if (delta.TotalMilliseconds > 0)
                    {
                        Trace.WriteLine(string.Format("Delay of {0} started for {1} milliseconds", activity, delta.TotalMilliseconds.ToString()));
                        await Task.Delay((int)delta.TotalMilliseconds, cancellationToken);
                        Trace.WriteLine(string.Format("Delay of {0} finished", activity));
                    }

                    if (!activity.IsCancelled)
                    {
                        await this.AddToCollection(this.Queue, activity, cancellationToken);
                    }
                }
            }
        }


        private async Task AddToCollection(BlockingCollection<IActivity> collection, IActivity activity, CancellationToken cancellationToken)
        {
            if (collection != null)
            {
                await Task.Run(() =>
                    {
                        if (!collection.Any(a => a.Equals(activity)))
                        {
                            collection.Add(activity, cancellationToken);
                            Trace.WriteLine("Added to collection: " + activity);
                        }
                    });
            }
        }


        public bool IsRunning { get; private set; }
        public int ItemRetryDurationSeconds { get; private set; }
        public int ItemRetryAttempts { get; private set; }



        private CancellationTokenSource InternalExecutionCTS
        {
            get
            {
                if (this._internalExecutionCTS == null || this._internalExecutionCTS.IsCancellationRequested)
                {
                    this._internalExecutionCTS = new CancellationTokenSource();
                }
                return this._internalExecutionCTS;
            }
        }
        private CancellationTokenSource _internalExecutionCTS;

        private CancellationTokenSource ExternalInfluenceCTS
        {
            get
            {
                if (this._externalInfluenceCTS == null || this._externalInfluenceCTS.IsCancellationRequested)
                {
                    this._externalInfluenceCTS = new CancellationTokenSource();
                }
                return this._externalInfluenceCTS;
            }
        }
        private CancellationTokenSource _externalInfluenceCTS;



        private BlockingCollection<IActivity> Queue = new BlockingCollection<IActivity>();
        private BlockingCollection<IActivity> RetryQueue = new BlockingCollection<IActivity>();
    }
}
