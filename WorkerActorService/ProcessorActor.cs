﻿#region Copyright

//=======================================================================================
// Microsoft Azure Customer Advisory Team  
//
// This sample is supplemental to the technical guidance published on the community
// blog at http://blogs.msdn.com/b/paolos/. 
// 
// Author: Paolo Salvatori
//=======================================================================================
// Copyright © 2016 Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
// EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
// MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. YOU BEAR THE RISK OF USING IT.
//=======================================================================================

#endregion

#region Using Directives

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AzureCat.Samples.Entities;
using Microsoft.AzureCat.Samples.Framework;
using Microsoft.AzureCat.Samples.Framework.Interfaces;
using Microsoft.AzureCat.Samples.WorkerActorService.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;

#endregion

namespace Microsoft.AzureCat.Samples.WorkerActorService
{
    /// <remarks>
    ///     This actor can be used to start, stop and monitor long running processes.
    /// </remarks>
    [ActorService(Name = "ProcessorActorService")]
    [StatePersistence(StatePersistence.Persisted)]
    internal class ProcessorActor : Actor, IProcessorActor, IRemindable
    {
        #region Private Constants

        //************************************
        // Constants
        //************************************
        private const string DelayProperty = "delay";
        private const string StepsProperty = "steps";
        private const string WorkerId = "workerId";
        private const string Message = "message";

        #endregion

        #region Private Fields

        private Uri queueActorServiceUri;
        private Uri workerActorServiceUri;

        #endregion

        #region Public Constructor

        /// <summary>
        ///     Initializes a new instance of ProcessorActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ProcessorActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        #endregion

        #region IRemindable Methods

        public async Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            try
            {
                // Unregister reminder
                var reminder = GetReminder(reminderName);
                if (reminder != null)
                {
                    await UnregisterReminderAsync(reminder);
                }

                // Gets the message id from the context
                var messageId = Id.GetStringId();

                // Retrieves the worker id
                var workerIdResult = await StateManager.TryGetStateAsync<string>(WorkerId);

                // Retrieves the message
                var messageResult = await StateManager.TryGetStateAsync<Q2Message>(Message);

                // Retrieves the cancellation token source from the actor state
                var cancellationTokenResult = await StateManager.TryGetStateAsync<CancellationToken>(messageId);

                if (workerIdResult.HasValue &&
                    messageResult.HasValue &&
                    cancellationTokenResult.HasValue)
                {
                    var workerId = workerIdResult.Value;
                    var message = messageResult.Value;
                    var cancellationToken = cancellationTokenResult.Value;

                    // Tries to start the processor. If the processor is already running, the task will timeout after 1 second.
                    var taskList = new List<Task>
                    {
                        InternalProcessParallelMessageAsync(workerId, message, cancellationToken),
                        Task.Delay(TimeSpan.FromSeconds(3), cancellationToken)
                    };
                    await Task.WhenAny(taskList);
                }
            }
            catch (Exception ex)
            {
                ActorEventSource.Current.Error(ex);
            }
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        ///     Process a messages.
        /// </summary>
        /// <param name="workerId">The worker id.</param>
        /// <param name="message">The message to process</param>
        /// <param name="cancellationToken">The cancellation token to interrupt message processing.</param>
        /// <returns>The object at the beginning of the circular queue.</returns>
        protected virtual async Task InternalProcessParallelMessageAsync(string workerId, Q2Message message, CancellationToken cancellationToken)
        {
            try
            {
                // Message validation
                if (string.IsNullOrWhiteSpace(message.MessageId) ||
                    string.IsNullOrWhiteSpace(message.Body))
                    return;

                // Create delay variable and assign 1 second as default value
                var delay = TimeSpan.FromSeconds(1);

                // Create steps variable and assign 10 as default value
                var steps = 10;

                if (message.Properties != null)
                {
                    // Checks if the message Properties collection contains the delay property
                    if (message.Properties.ContainsKey(DelayProperty))
                        if (message.Properties[DelayProperty] is TimeSpan)
                        {
                            // Assigns the property value to the delay variable
                            delay = (TimeSpan) message.Properties[DelayProperty];
                        }
                        else
                        {
                            var value = message.Properties[DelayProperty] as string;
                            if (value != null)
                            {
                                TimeSpan temp;
                                if (TimeSpan.TryParse(value, out temp))
                                    delay = temp;
                            }
                        }

                    // Checks if the message Properties collection contains the steps property
                    if (message.Properties.ContainsKey(StepsProperty))
                    {
                        if (message.Properties[StepsProperty] is int)
                            steps = (int) message.Properties[StepsProperty];
                        if (message.Properties[StepsProperty] is long)
                        {
                            // Assigns the property value to the steps variable
                            steps = (int) (long) message.Properties[StepsProperty];
                        }
                        else
                        {
                            var value = message.Properties[StepsProperty] as string;
                            if (value != null)
                            {
                                int temp;
                                if (int.TryParse(value, out temp))
                                    steps = temp;
                            }
                        }
                    }
                }

                // NOTE!!!! This section should be replaced by some real computation
                string stage = message.MessageId.EndsWith("delta") ? "delta" : "referenceRange";

                ActorEventSource.Current.Message(
                    $"MessageId=[{message.MessageId}] Body=[{message.Body}] ProcessStep=[{stage}]");

                var startTime = DateTime.Now.ToShortTimeString();
                var endTime = DateTime.Now.AddMinutes(1).ToShortTimeString();

                // Update message based on stage
                switch (stage)
                {
                    case "referenceRange":
                        message.ResultingStatus.ReferenceRange.StartTime = startTime;
                        // Call processing method here
                        message.ResultingStatus.ReferenceRange.EndTime = endTime;
                        message.ResultingStatus.ReferenceRange.Status = stage;
                        break;
                    case "delta":
                        message.ResultingStatus.Delta.StartTime = startTime;
                        // Call processing method here
                        message.ResultingStatus.Delta.EndTime = endTime;
                        message.ResultingStatus.Delta.Status = stage;
                        break;
                    default:
                        break;
                }

                // Processing when using steps. Currently not using this.
                //for (var i = 0; i < steps; i++)
                //{
                //    ActorEventSource.Current.Message($"MessageId=[{message.MessageId}] Body=[{message.Body}] ProcessStep=[{i + 1}]");
                //    try
                //    {
                //        await Task.Delay(delay, cancellationToken);
                //    }
                //    catch (TaskCanceledException)
                //    {
                //    }

                //    if (!cancellationToken.IsCancellationRequested)
                //    {
                //        continue;
                //    }
                //    // NOTE: If message processing has been cancelled, 
                //    // the method returns immediately without any result
                //    ActorEventSource.Current.Message($"MessageId=[{message.MessageId}] elaboration has been canceled and parallel message processing stopped.");
                //    return;
                //}

                ActorEventSource.Current.Message($"MessageId=[{message.MessageId}] has been successfully processed.");

                var workerActorProxy = ActorProxy.Create<IWorkerActor>(new ActorId(workerId), workerActorServiceUri);

                //for (var n = 1; n <= 10; n++)
                //{
                    try
                    {
                        // Simulates a return value between 1 and 100
                        var random = new Random();
                        var returnValue = random.Next(1, 101);

                        // Stops the current processing task: it removes the corresponding state from the worker actor
                        var ok = await workerActorProxy.ReturnParallelProcessingAsync(message, message.MessageId, returnValue);
                        if (ok)
                        {
                            ActorEventSource.Current.Message($"Parallel processing of MessageId=[{message.MessageId}] successfully stopped.");
                        }
                        return;
                    }
                    catch (FabricTransientException ex)
                    {
                        ActorEventSource.Current.Message(ex.Message);
                    }
                    catch (AggregateException ex)
                    {
                        foreach (var e in ex.InnerExceptions)
                        {
                            ActorEventSource.Current.Message(e.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        ActorEventSource.Current.Message(ex.Message);
                        throw;
                    }
                    Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).Wait(cancellationToken);
                //}
                throw new TimeoutException();
            }
            catch (Exception ex)
            {
                ActorEventSource.Current.Error(ex);
            }
        }

        #endregion

        #region Actor Overridden Methods

        protected override Task OnActivateAsync()
        {
            base.OnActivateAsync();
            queueActorServiceUri = new Uri($"{ApplicationName}/QueueActorService");
            workerActorServiceUri = new Uri($"{ApplicationName}/WorkerActorService");
            ActorEventSource.Current.Message($"Worker Actor [{Id}] activated.");
            return Task.FromResult(0);
        }

        #endregion

        #region IProcessorActor Methods

        /// <summary>
        ///     Starts processing messages from the work queue in a sequential order.
        /// </summary>
        /// <param name="cancellationToken">This CancellationToken is used to stop message processing.</param>
        /// <returns></returns>
        public async Task ProcessSequentialMessagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                Q2Message message;

                // Creates the proxy to call the queue actor
                var queueActorProxy = ActorProxy.Create<ICircularQueueActor>(new ActorId(Id.ToString()),
                    queueActorServiceUri);

                // Creates the proxy to call the worker actor
                var workerActorProxy = ActorProxy.Create<IWorkerActor>(new ActorId(Id.ToString()), workerActorServiceUri);

                // The method keeps processing messages from the queue, until the queue is empty
                while ((message = await queueActorProxy.DequeueAsync()) != null)
                    try
                    {
                        // Message validation
                        if (string.IsNullOrWhiteSpace(message.MessageId) ||
                            string.IsNullOrWhiteSpace(message.Body))
                        {
                            ActorEventSource.Current.Message("Message Invalid.");
                            continue;
                        }

                        // Create delay variable and assign 1 second as default value
                        var delay = TimeSpan.FromSeconds(1);

                        string stage = "";

                        // Get current status, if exists
                        if (message.ResultingStatus.Filing.Status == "")
                        {
                            stage = "filing";
                        }
                        else if (message.ResultingStatus.ReferenceRange.Status == "")
                        {
                            stage = "referenceRange";
                        }
                        else if (message.ResultingStatus.Delta.Status == "")
                        {
                            stage = "delta";
                        }
                        else if (message.ResultingStatus.CustomRule.Status == "")
                        {
                            stage = "customRule";
                        };

                        // Create steps variable and assign 10 as default value
                        var steps = 3;

                        if (message.Properties != null)
                        {
                            // Checks if the message Properties collection contains the delay property
                            if (message.Properties.ContainsKey(DelayProperty))
                                if (message.Properties[DelayProperty] is TimeSpan)
                                {
                                    // Assigns the property value to the delay variable
                                    delay = (TimeSpan) message.Properties[DelayProperty];
                                }
                                else
                                {
                                    var value = message.Properties[DelayProperty] as string;
                                    if (value != null)
                                    {
                                        TimeSpan temp;
                                        if (TimeSpan.TryParse(value, out temp))
                                            delay = temp;
                                    }
                                }

                            // Checks if the message Properties collection contains the steps property
                            if (message.Properties.ContainsKey(StepsProperty))
                            {
                                if (message.Properties[StepsProperty] is int)
                                    steps = (int) message.Properties[StepsProperty];
                                if (message.Properties[StepsProperty] is long)
                                {
                                    // Assigns the property value to the steps variable
                                    steps = (int) (long) message.Properties[StepsProperty];
                                }
                                else
                                {
                                    var value = message.Properties[StepsProperty] as string;
                                    if (value != null)
                                    {
                                        int temp;
                                        if (int.TryParse(value, out temp))
                                            steps = temp;
                                    }
                                }
                            }
                        }

                        // NOTE!!!! This section should be replaced by some real computation
                        ActorEventSource.Current.Message(
                                $"MessageId=[{message.MessageId}] Body=[{message.Body}] ProcessStep=[{stage}]");

                        var startTime = DateTime.Now.ToShortTimeString();
                        var endTime = DateTime.Now.AddMinutes(1).ToShortTimeString();
                        
                        // Update message based on stage
                        switch (stage)
                        {
                            case "filing":
                                message.ResultingStatus.Filing.StartTime = startTime;
                                // Call processing method here
                                message.ResultingStatus.Filing.EndTime = endTime;
                                message.ResultingStatus.Filing.Status = stage;
                                // Enqueues the message for next stage
                                await queueActorProxy.EnqueueAsync(message);
                                break;
                            case "referenceRange":
                                message.ResultingStatus.ReferenceRange.StartTime = startTime;
                                // Call processing method here
                                message.ResultingStatus.ReferenceRange.EndTime = endTime;
                                message.ResultingStatus.ReferenceRange.Status = stage;

                                // Going to use Worker to handle parallel processing initiation, so creating an external task here to do processing
                                // using a separate ProcessorActor per message.
                                var taskList = new List<Task>();
                                Func<string, Task> waitHandler = async messageId =>
                                {
                                    try
                                    {
                                        while (await workerActorProxy.IsParallelProcessingRunningAsync(messageId))
                                        {
                                            Console.WriteLine($" - [{DateTime.Now.ToLocalTime()}] Waiting for [{messageId}] parallel processing task completion...");
                                            await Task.Delay(TimeSpan.FromSeconds(1));
                                        }
                                        ActorEventSource.Current.Message($" - [{DateTime.Now.ToLocalTime()}] [{messageId}] Parallel message processing task completed.");
                                    }
                                    catch (Exception ex)
                                    {
                                        ActorEventSource.Current.Message(ex.Message);
                                    }
                                };

                                // Create two Q2Message instances and pas them into StartParallelProcessingAsync
                                var deltaMessage = message.CopyForDelta();
                                message.MessageId = message.MessageId + "RR"; // Using the same message, but changing the ID so as not to conflict with the incoming state manager

                                List<Q2Message> messageList = new List<Q2Message>();
                                messageList.Add(message);
                                messageList.Add(deltaMessage);

                                // Start parallel processing
                                foreach (var parallelMessage in messageList)
                                {
                                    if (!workerActorProxy.StartParallelProcessingAsync(parallelMessage).Result)
                                    {
                                        continue;
                                    }
                                    taskList.Add(waitHandler(message.MessageId));
                                    ActorEventSource.Current.Message($" - [{DateTime.Now.ToLocalTime()}] Message [{JsonSerializerHelper.Serialize(parallelMessage)}] sent.");
                                }

                                // Wait for message processing completion
                                Task.WaitAll(taskList.ToArray());

                                // Get the final combined message from the WorkerActor
                                message = workerActorProxy.GetParallelProcessingResultAsync().Result;

                                // Enqueues the message for next stage
                                await queueActorProxy.EnqueueAsync(message);
                                break;
                            case "customRule":
                                message.ResultingStatus.CustomRule.StartTime = startTime;
                                // Call processing method here
                                message.ResultingStatus.CustomRule.EndTime = endTime;
                                message.ResultingStatus.CustomRule.Status = stage;

                                // Final stage, return back to worker
                                try
                                {
                                    // Simulates a return value between 1 and 100
                                    var random = new Random();
                                    var returnValue = random.Next(1, 101);

                                    // Returns result to worker actor
                                    await workerActorProxy.ReturnSequentialProcessingAsync(message, message.MessageId, startTime, endTime, stage, returnValue);

                                    //Logs event
                                    ActorEventSource.Current.Message(
                                        $"Sequential processing of MessageId=[{message.MessageId}] ReturnValue=[{returnValue}] successfully returned.");
                                    break;
                                }
                                catch (FabricTransientException ex)
                                {
                                    ActorEventSource.Current.Message(ex.Message);
                                }
                                catch (AggregateException ex)
                                {
                                    foreach (var e in ex.InnerExceptions)
                                        ActorEventSource.Current.Message(e.Message);
                                }
                                catch (Exception ex)
                                {
                                    ActorEventSource.Current.Message(ex.Message);
                                }
                                break;
                            default:
                                break;
                        }

                        // Processing when steps are used. Currently not using this.
                        //
                        //for (var i = 0; i < steps; i++)
                        //{
                        //    ActorEventSource.Current.Message(
                        //        $"MessageId=[{message.MessageId}] Body=[{message.Body}] ProcessStep=[{i + 1}]");
                        //    try
                        //    {
                        //        await Task.Delay(delay, cancellationToken);
                        //    }
                        //    catch (TaskCanceledException)
                        //    {
                        //    }

                        //    if (!cancellationToken.IsCancellationRequested)
                        //        continue;
                        //    // NOTE: If message processing has been cancelled, 
                        //    // the method returns immediately without any result
                        //    ActorEventSource.Current.Message(
                        //        $"MessageId=[{message.MessageId}] elaboration has been canceled and sequential message processing stopped.");
                        //    return;
                        //}

                        ActorEventSource.Current.Message(
                            $"MessageId=[{message.MessageId}] has been successfully processed.");

                        //for (var n = 1; n <= 3; n++)
                        //{
                        //    try
                        //    {
                        //        // Simulates a return value between 1 and 100
                        //        var random = new Random();
                        //        var returnValue = random.Next(1, 101);

                        //        // Returns result to worker actor
                        //        await workerActorProxy.ReturnSequentialProcessingAsync(message, message.MessageId, startTime, endTime, stage, returnValue);

                        //        //Logs event
                        //        ActorEventSource.Current.Message(
                        //            $"Sequential processing of MessageId=[{message.MessageId}] ReturnValue=[{returnValue}] successfully returned.");
                        //        break;
                        //    }
                        //    catch (FabricTransientException ex)
                        //    {
                        //        ActorEventSource.Current.Message(ex.Message);
                        //    }
                        //    catch (AggregateException ex)
                        //    {
                        //        foreach (var e in ex.InnerExceptions)
                        //            ActorEventSource.Current.Message(e.Message);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        ActorEventSource.Current.Message(ex.Message);
                        //    }
                        //    Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).Wait(cancellationToken);
                        //}
                    }
                    catch (Exception ex)
                    {
                        ActorEventSource.Current.Error(ex);
                    }

                for (var n = 1; n <= 3; n++)
                {
                    try
                    {
                        // Sets the sequential processing state to false
                        await workerActorProxy.CloseSequentialProcessingAsync(false);
                        ActorEventSource.Current.Message("Closed sequential processing.");
                        return;
                    }
                    catch (FabricTransientException ex)
                    {
                        ActorEventSource.Current.Message(ex.Message);
                    }
                    catch (AggregateException ex)
                    {
                        foreach (var e in ex.InnerExceptions)
                            ActorEventSource.Current.Message(e.Message);
                    }
                    catch (Exception ex)
                    {
                        ActorEventSource.Current.Message(ex.Message);
                        throw;
                    }
                    Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).Wait(cancellationToken);
                }
                throw new TimeoutException();
            }
            catch (Exception ex)
            {
                ActorEventSource.Current.Error(ex);
            }
        }

        public async Task ProcessParallelMessagesAsync(string workerId, Q2Message message,
            CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(workerId) || (message == null))
                    return;

                await StateManager.TryAddStateAsync(WorkerId,
                    workerId,
                    cancellationToken);

                await StateManager.TryAddStateAsync(Message,
                    message,
                    cancellationToken);

                await StateManager.TryAddStateAsync(Id.GetStringId(),
                    cancellationToken,
                    cancellationToken);

                await RegisterReminderAsync(message.MessageId,
                    null,
                    TimeSpan.FromMilliseconds(10),
                    TimeSpan.FromMilliseconds(-1));
            }
            catch (Exception ex)
            {
                ActorEventSource.Current.Error(ex);
            }
        }

        #endregion
    }
}