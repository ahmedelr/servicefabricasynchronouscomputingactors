#region Copyright

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

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace Microsoft.AzureCat.Samples.Entities
{
    /// <summary>
    ///     Represents messages elaborated by the worker and processor actors.
    /// </summary>
    public class Q2Message
    {
        /// <summary>
        ///     Gets or sets the message id.
        /// </summary>
        [JsonProperty(PropertyName = "MessageVersion", Order = 1)]
        public string MessageVersion { get; set; }

        /// <summary>
        ///     Gets or sets the message id.
        /// </summary>
        [JsonProperty(PropertyName = "messageId", Order = 2)]
        public string MessageId { get; set; }

        /// <summary>
        ///     Gets or sets the message body.
        /// </summary>
        [JsonProperty(PropertyName = "body", Order = 3)]
        public string Body { get; set; }

        /// <summary>
        ///     Gets or sets the message properties
        /// </summary>
        [JsonProperty(PropertyName = "properties", Order = 4)]
        public IDictionary<string, object> Properties { get; set; }

        /// <summary>
        ///     Gets or sets the message ResultInfo
        /// </summary>
        [JsonProperty(PropertyName = "resultInfo", Order = 5)]
        public ResultInfo ResultInfo { get; set; }

        /// <summary>
        ///     Gets or sets the message ResultingStatus
        /// </summary>
        [JsonProperty(PropertyName = "resultingStatus", Order = 6)]
        public ResultingStatus ResultingStatus { get; set; }

        /// <summary>
        ///     Gets or sets the studyInfo.
        /// </summary>
        [JsonProperty(PropertyName = "studyInfo", Order = 7)]
        public string StudyInfo { get; set; }

       public Q2Message CopyForDelta()
        {
            var newMessage = new Q2Message
            {
                MessageVersion = this.MessageVersion,
                MessageId = this.MessageId + "delta",
                Body = this.Body,
                Properties = this.Properties,
                ResultInfo = this.ResultInfo,
                ResultingStatus = this.ResultingStatus,
                StudyInfo = this.StudyInfo
            };
            
            return newMessage;
        }
    }
}