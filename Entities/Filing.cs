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
    public class Filing
    {
        /// <summary>
        ///     Gets or sets the starttime.
        /// </summary>
        [JsonProperty(PropertyName = "starttime", Order = 1)]
        public string StartTime { get; set; }

        /// <summary>
        ///     Gets or sets the endtime.
        /// </summary>
        [JsonProperty(PropertyName = "endtime", Order = 2)]
        public string EndTime { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        [JsonProperty(PropertyName = "status", Order = 3)]
        public string Status { get; set; }

        /// <summary>
        ///     Gets or sets the errorcode
        /// </summary>
        [JsonProperty(PropertyName = "errorcode", Order = 4)]
        public string ErrorCode { get; set; }

        /// <summary>
        ///     Gets or sets the errormessage
        /// </summary>
        [JsonProperty(PropertyName = "errormessage", Order = 5)]
        public string ErrorMessage { get; set; }
    }
}