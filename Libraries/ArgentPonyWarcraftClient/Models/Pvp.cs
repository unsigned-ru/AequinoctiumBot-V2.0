﻿using Newtonsoft.Json;

namespace AequinoctiumBot
{
    /// <summary>
    /// PvP information for a character.
    /// </summary>
    public class Pvp
    {
        /// <summary>
        /// Gets or sets the PvP brackets.
        /// </summary>
        [JsonProperty("brackets")]
        public PvpBrackets Brackets { get; set; }
    }
}
