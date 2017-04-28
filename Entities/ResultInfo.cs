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
    public class ResultInfo
    {
        /// <summary>
        ///     Gets or sets the result.
        /// </summary>
        [JsonProperty(PropertyName = "result", Order = 1)]
        public string Result { get; set; }

        /// <summary>
        ///     Gets or sets the accession.
        /// </summary>
        [JsonProperty(PropertyName = "accession", Order = 2)]
        public string Accession { get; set; }

        /// <summary>
        ///     Gets or sets the analyte.
        /// </summary>
        [JsonProperty(PropertyName = "analyte", Order = 3)]
        public string Analyte { get; set; }

        /// <summary>
        ///     Gets or sets the unit.
        /// </summary>
        [JsonProperty(PropertyName = "unit", Order = 4)]
        public string Unit { get; set; }

        /// <summary>
        ///     Gets or sets the resultedDate.
        /// </summary>
        [JsonProperty(PropertyName = "resultedDate", Order = 5)]
        public string ResultedDate { get; set; }

        /// <summary>
        ///     Gets or sets the resultingUser.
        /// </summary>
        [JsonProperty(PropertyName = "resultingUser", Order = 6)]
        public string ResultingUser { get; set; }

        /// <summary>
        ///     Gets or sets the releasedDate.
        /// </summary>
        [JsonProperty(PropertyName = "releasedDate", Order = 7)]
        public string ReleasedDate { get; set; }

        /// <summary>
        ///     Gets or sets the releasingUser.
        /// </summary>
        [JsonProperty(PropertyName = "releasingUser", Order = 8)]
        public string ReleasingUser { get; set; }

        /// <summary>
        ///     Gets or sets the comment.
        /// </summary>
        [JsonProperty(PropertyName = "comment", Order = 9)]
        public string Comment { get; set; }

        /// <summary>
        ///     Gets or sets the facility.
        /// </summary>
        [JsonProperty(PropertyName = "facility", Order = 10)]
        public string Facility { get; set; }

        /// <summary>
        ///     Gets or sets the studyId.
        /// </summary>
        [JsonProperty(PropertyName = "studyId", Order = 11)]
        public string StudyId { get; set; }
    }
}