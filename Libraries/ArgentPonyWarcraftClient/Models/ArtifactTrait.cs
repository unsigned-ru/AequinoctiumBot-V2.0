﻿using Newtonsoft.Json;

namespace AequinoctiumBot
{
    /// <summary>
    /// An artifact trait.
    /// </summary>
    public class ArtifactTrait
    {
        /// <summary>
        /// 
        /// 
        /// 
        /// s or sets the artifact trait ID.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the rank.
        /// </summary>
        [JsonProperty("rank")]
        public int Rank { get; set; }
    }
}
