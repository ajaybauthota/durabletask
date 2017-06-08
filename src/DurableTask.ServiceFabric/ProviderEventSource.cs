﻿//  ----------------------------------------------------------------------------------
//  Copyright Microsoft Corporation
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  ----------------------------------------------------------------------------------

namespace DurableTask.ServiceFabric
{
    using Microsoft.Diagnostics.Tracing;
    using System.Threading.Tasks;
    using System;

    [EventSource(Name = "DurableTask-ServiceFabricProvider")]
    internal sealed class ProviderEventSource : EventSource
    {
        public static readonly ProviderEventSource Log = new ProviderEventSource();

        static ProviderEventSource()
        {
            // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
            // This problem will be fixed in .NET Framework 4.6.2.
            Task.Run(() => { });
        }

        private ProviderEventSource() : base()
        {
        }

        public static class Keywords
        {
            public const EventKeywords Orchestration = (EventKeywords) 0x1L;
            public const EventKeywords Activity = (EventKeywords) 0x2L;
            public const EventKeywords Common = (EventKeywords)0x4L;
        }

        #region Informational 1-500
        [Event(1, Level = EventLevel.Informational, Message = "Orchestration with instanceId : '{0}' and executionId : '{1}' is Created.", Channel = EventChannel.Operational)]
        public void OrchestrationCreated(string instanceId, string executionId)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Orchestration))
            {
                WriteEvent(1, instanceId, executionId);
            }
        }

        [Event(2, Level = EventLevel.Informational, Message = "Orchestration {0} Finished with the status {1} and result {3} in {2} seconds.", Channel = EventChannel.Operational)]
        public void OrchestrationFinished(string instanceId, string terminalStatus, double runningTimeInSeconds, string result)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Orchestration))
            {
                WriteEvent(2, instanceId, terminalStatus, runningTimeInSeconds, result);
            }
        }

        [Event(4, Level = EventLevel.Informational, Message = "Current number of entries in store {0} : {1}", Channel = EventChannel.Operational)]
        public void LogStoreCount(string storeName, long count)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Common))
            {
                WriteEvent(4, storeName, count);
            }
        }

        [Event(8, Level = EventLevel.Informational, Message = "Time taken for {0} : {1} milli seconds.", Channel = EventChannel.Operational)]
        public void LogTimeTaken(string uniqueActionIdentifier, double elapsedMilliseconds)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Common))
            {
                WriteEvent(8, uniqueActionIdentifier, elapsedMilliseconds);
            }
        }

        [Event(9, Level = EventLevel.Informational, Message = "{0} : {1}", Channel = EventChannel.Operational)]
        public void ReliableStateManagement(string operationIdentifier, string operationData)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Common))
            {
                WriteEvent(9, operationIdentifier, operationData);
            }
        }
        #endregion

        #region Warnings 1001-1500
        [Event(1001, Level = EventLevel.Warning, Message = "Exception in the background job {0} : {1}", Channel = EventChannel.Operational)]
        public void ExceptionWhileRunningBackgroundJob(string operationIdentifier, string exception)
        {
            if (IsEnabled(EventLevel.Warning, Keywords.Common))
            {
                WriteEvent(1001, operationIdentifier, exception);
            }
        }

        [Event(1002, Level = EventLevel.Warning, Message = "Hint : {0}, Exception: {1}", Channel = EventChannel.Operational)]
        public void RetryableFabricException(string uniqueIdentifier, int attemptNumber, string exception)
        {
            if (IsEnabled(EventLevel.Warning, Keywords.Common))
            {
                WriteEvent(1002, uniqueIdentifier, exception);
            }
        }
        #endregion

        #region Errors 1501-2000
        [Event(1501, Level = EventLevel.Error, Message = "We are seeing something that we don't expect to see : {0}", Channel = EventChannel.Operational)]
        public void UnexpectedCodeCondition(string uniqueMessage)
        {
            if (IsEnabled(EventLevel.Error, Keywords.Common))
            {
                WriteEvent(1501, uniqueMessage);
            }

#if DEBUG
            // This is so that tests fail when this event happens.
            throw new Exception(uniqueMessage);
#endif
        }

        [Event(1502, Level = EventLevel.Error, Message = "Hint : {0}, Exception: {1}", Channel = EventChannel.Operational)]
        public void ExceptionInReliableCollectionOperations(string uniqueIdentifier, string exception)
        {
            if (IsEnabled(EventLevel.Error, Keywords.Common))
            {
                WriteEvent(1502, uniqueIdentifier, exception);
            }
        }
        #endregion

        #region Verbose Events 501-1000
        [Event(501, Level = EventLevel.Verbose, Message = "Trace Message for Session {0} : {1}", Channel = EventChannel.Debug)]
        public void TraceMessage(string instanceId, string message)
        {
#if DEBUG
            if (IsEnabled(EventLevel.Verbose, Keywords.Common))
            {
                WriteEvent(501, instanceId, message);
            }
#endif
        }

        [Event(502, Level = EventLevel.Verbose, Message = "Time taken for {0} : {1} milli seconds.", Channel = EventChannel.Analytic)]
        public void LogMeasurement(string uniqueActionIdentifier, long elapsedMilliseconds)
        {
#if DEBUG
            if (IsEnabled(EventLevel.Verbose, Keywords.Common))
            {
                WriteEvent(502, uniqueActionIdentifier, elapsedMilliseconds);
            }
#endif
        }

        [Event(503, Level = EventLevel.Verbose, Message = "Size of {0} : {1} bytes.", Channel = EventChannel.Analytic)]
        public void LogSizeMeasure(string uniqueObjectIdentifier, long sizeInBytes)
        {
#if DEBUG
            if (IsEnabled(EventLevel.Verbose, Keywords.Common))
            {
                WriteEvent(503, uniqueObjectIdentifier, sizeInBytes);
            }
#endif
        }
        #endregion
    }
}