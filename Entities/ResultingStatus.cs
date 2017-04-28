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
    public class ResultingStatus
    {
        /// <summary>
        ///     Gets or sets the filing.
        /// </summary>
        [JsonProperty(PropertyName = "filing", Order = 1)]
        public Filing Filing { get; set; }

        /// <summary>
        ///     Gets or sets the referenceRange.
        /// </summary>
        [JsonProperty(PropertyName = "referenceRange", Order = 2)]
        public ReferenceRange ReferenceRange { get; set; }

        /// <summary>
        ///     Gets or sets the delta.
        /// </summary>
        [JsonProperty(PropertyName = "delta", Order = 3)]
        public Delta Delta { get; set; }

        /// <summary>
        ///     Gets or sets the customRule
        /// </summary>
        [JsonProperty(PropertyName = "customRule", Order = 4)]
        public CustomRule CustomRule { get; set; }

        /// <summary>
        ///     Gets or sets the errormessage
        /// </summary>
        [JsonProperty(PropertyName = "errormessage", Order = 5)]
        public string ErrorMessage { get; set; }
    }
}