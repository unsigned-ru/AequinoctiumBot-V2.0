﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace AequinoctiumBot
{
    /// <summary>
    /// Information about a character's pets.
    /// </summary>
    public class CharacterPets
    {
        /// <summary>
        /// Gets or sets the number of pets this character has collected.
        /// </summary>
        [JsonProperty("numCollected")]
        public int NumCollected { get; set; }

        /// <summary>
        /// Gets or sets the number of pets this character has not collected.
        /// </summary>
        [JsonProperty("numNotCollected")]
        public int NumNotCollected { get; set; }

        /// <summary>
        /// Gets or sets the pets this character has collected.
        /// </summary>
        [JsonProperty("collected")]
        public IList<ChararcterPet> Collected { get; set; }
    }
}
