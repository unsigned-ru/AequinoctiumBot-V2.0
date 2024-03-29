﻿using Newtonsoft.Json;

namespace AequinoctiumBot
{
    /// <summary>
    /// A guild member.
    /// </summary>
    public class GuildMember
    {
        /// <summary>
        /// Gets or sets the character.
        /// </summary>
        [JsonProperty("character")]
        public GuildCharacter Character { get; set; }

        /// <summary>
        /// Gets or sets the character's rank in the guild.
        /// </summary>
        [JsonProperty("rank")]
        public int Rank { get; set; }
    }
}
