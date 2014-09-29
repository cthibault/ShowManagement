using Microsoft.Practices.Unity;
using ShowManagement.CommonServiceProviders;
using ShowManagement.WindowsServices.NameResolver.Components.Activities;
using ShowManagement.Core.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShowManagement.WindowsServices.NameResolver.Diagnostics;

namespace ShowManagement.WindowsServices.NameResolver.Components
{
    public class NameResolverEngine : INameResolverEngine
    {
        public NameResolverEngine(SettingsManager settingsManager, IShowManagementServiceProvider showManagementServiceProvider)
        {
            if (settingsManager == null)
            {
                throw new ArgumentNullException("settingsManager");
            }

            this.ItemRetryAttempts = settingsManager.ItemRetryAttempts;
            this.ItemRetryDurationSeconds = settingsManager.ItemRetryDurationSeconds;

            this._showManagementServiceProvider = showManagementServiceProvider;
        }

        public async Task Start()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Components.NameResolverEngine.Start()");

            try
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Starting the Engine Processing Queues.");

                await Task.Run(() => 
                    {
                        this.ProcessItemsFromQueue(this.InternalExecutionCTS.Token);
                        this.ProcessItemsFromRetryQueue(this.InternalExecutionCTS.Token);
                    });

                this.IsRunning = true;
            }
            catch (OperationCanceledException ex)
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Error, 0, "Exception caught and rethrown in ShowManagement.WindowsServices.NameResolver.Components.NameResolverEngine.Start(): {0}", ex.ExtractExceptionMessage());
                throw;
            }

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Components.NameResolverEngine.Start()");
        }

        public async Task Stop()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Components.NameResolverEngine.Stop()");

            this.InternalExecutionCTS.Cancel();
            this.ExternalInfluenceCTS.Cancel();

            this.IsRunning = false;
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Stopped the Engine Processing Queues");

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Components.NameResolverEngine.Stop()");
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
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Components.NameResolverEngine.Add()");

            await Task.Run(async () =>
                {
                    if (filePaths != null)
                    {
                        TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Trying to add the {0} new file paths to Process Queue", filePaths.Count());
                        foreach (var filePath in filePaths)
                        {
                            var activity = new ResolveNameActivity(filePath, retryAttempts, this._showManagementServiceProvider);

                            await this.AddToCollection(this.ProcessQueue, activity, this.ExternalInfluenceCTS.Token);
                        }
                    }
                });

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Components.NameResolverEngine.Add()");
        }

        public async Task Remove(string filePath)
        {
            await this.Remove(new List<string> { filePath });
        }
        public async Task Remove(IEnumerable<string> filePaths)
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Components.NameResolverEngine.Remove()");

            var removeFromQueueTask = Task.Factory.StartNew((x) =>
            {
                var paths = x as IEnumerable<string>;

                if (paths != null)
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Trying to remove the {0} file paths from Process Queue", paths.Count());
                    var mockActivities = paths.Select(p => new ResolveNameActivity(p, 0, null));

                    var activitiesToCancel = this.ProcessQueue.Intersect(mockActivities, new ActivityComparer());

                    if (activitiesToCancel.Any())
                    {
                        TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "{0} items to remove from the Process Queue:\r\n\t", activitiesToCancel.Count(), string.Join("\r\n\t", activitiesToCancel));

                        foreach (var activity in activitiesToCancel)
                        {
                            activity.Cancel();
                        }
                    }
                    else
                    {
                        TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "0 items to remove from the Process Queue");
                    }
                }
            }, filePaths, this.ExternalInfluenceCTS.Token);

            var removeFromRetryQueueTask = Task.Factory.StartNew((x) =>
            {
                var paths = x as IEnumerable<string>;

                if (paths != null)
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Trying to remove the {0} file paths from Retry Queue", paths.Count());
                    var mockActivities = paths.Select(p => new ResolveNameActivity(p, 0, null));

                    var activitiesToCancel = this.RetryQueue.Intersect(mockActivities, new ActivityComparer());
                    
                    if (activitiesToCancel.Any())
                    {
                        TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "{0} items to remove from the Retry Queue:\r\n\t", activitiesToCancel.Count(), string.Join("\r\n\t", activitiesToCancel));

                        foreach (var activity in activitiesToCancel)
                        {
                            activity.Cancel();
                        }
                    }
                    else
                    {
                        TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "0 items to remove from the Retry Queue");
                    }
                }
            }, filePaths, this.ExternalInfluenceCTS.Token);

            await Task.WhenAll(removeFromQueueTask, removeFromRetryQueueTask);

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Components.NameResolverEngine.Remove()");
        }


        private async Task ProcessItemsFromQueue(CancellationToken cancellationToken)
        {
            foreach (var activity in this.ProcessQueue.GetConsumingEnumerable(cancellationToken))
            {
                if (!activity.IsCancelled)
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Perform Activity: {0}", activity);
                    IActivity result = await activity.Perform();

                    if (result != null)
                    {
                        TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Adding Activity to the Retry Queue: {0}", result);
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
                        TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Delay of {0} started for {1} milliseconds", activity, delta.TotalMilliseconds.ToString());
                        await Task.Delay((int)delta.TotalMilliseconds, cancellationToken);
                        TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Delay of {0} finished", activity);
                    }

                    if (!activity.IsCancelled)
                    {
                        TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Adding Activity to the Process Queue: {0}", activity);
                        await this.AddToCollection(this.ProcessQueue, activity, cancellationToken);
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
                            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Added to collection: {0}", activity);
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
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Creating new Internal Execution CancellationTokenSource");
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
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Creating new External Influence CancellationTokenSource");
                    this._externalInfluenceCTS = new CancellationTokenSource();
                }
                return this._externalInfluenceCTS;
            }
        }
        private CancellationTokenSource _externalInfluenceCTS;


        private BlockingCollection<IActivity> ProcessQueue = new BlockingCollection<IActivity>();
        private BlockingCollection<IActivity> RetryQueue = new BlockingCollection<IActivity>();

        private IShowManagementServiceProvider _showManagementServiceProvider;

    }
}
