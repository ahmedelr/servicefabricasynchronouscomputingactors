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

using Newtonsoft.Json;

#endregion

namespace Microsoft.AzureCat.Samples.Entities
{
    public class Result
    {
        /// <summary>
        ///     Gets or sets the message id.
        /// </summary>
        [JsonProperty(PropertyName = "messageId", Order = 1)]
        public string MessageId { get; set; }

        /// <summary>
        ///     Gets or sets the start time.
        /// </summary>
        [JsonProperty(PropertyName = "startTime", Order = 2)]
        public string StartTime { get; set; }

        /// <summary>
        ///     Gets or sets the endTime.
        /// </summary>
        [JsonProperty(PropertyName = "endTime", Order = 3)]
        public string EndTime { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        [JsonProperty(PropertyName = "status", Order = 4)]
        public string Status { get; set; }

        /// <summary>
        ///     Gets or sets the result.
        /// </summary>
        [JsonProperty(PropertyName = "returnValue", Order = 5)]
        public long ReturnValue { get; set; }
    }
}