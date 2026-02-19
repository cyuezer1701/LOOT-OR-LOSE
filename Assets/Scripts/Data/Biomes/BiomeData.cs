using System;
using UnityEngine;
using LootOrLose.Enums;

namespace LootOrLose.Data
{
    /// <summary>
    /// Defines a biome (dungeon theme) that determines the visual style,
    /// music, and hazard frequencies for a run.
    /// </summary>
    [Serializable]
    public class BiomeData
    {
        /// <summary>Unique identifier for this biome definition.</summary>
        public string id;

        /// <summary>Localization key for the biome's display name.</summary>
        public string nameKey;

        /// <summary>Localization key for the biome's description or flavor text.</summary>
        public string descriptionKey;

        /// <summary>The categorical type of this biome (e.g., Crypt, Volcano).</summary>
        public BiomeType type;

        /// <summary>Primary theme color as hex string (e.g., "#1a1a2e").</summary>
        public string primaryColor;

        /// <summary>Secondary theme color as hex string (e.g., "#2d2d44").</summary>
        public string secondaryColor;

        /// <summary>Whether the player has unlocked this biome.</summary>
        public bool isUnlocked = false;

        /// <summary>Localization key describing how to unlock this biome.</summary>
        public string unlockConditionKey;

        /// <summary>Minimum round required before this biome becomes available.</summary>
        public int minRound;

        /// <summary>Resource path to the biome's background artwork.</summary>
        public string backgroundPath;

        /// <summary>Resource path to the biome's icon sprite.</summary>
        public string iconPath;

        /// <summary>Resource path to the biome's ambient audio track.</summary>
        public string ambientTrack;

        /// <summary>
        /// Parses the primary hex color string into a Unity Color.
        /// Returns white if parsing fails.
        /// </summary>
        public Color GetPrimaryColor()
        {
            if (ColorUtility.TryParseHtmlString(primaryColor, out Color color))
                return color;
            return Color.white;
        }

        /// <summary>
        /// Parses the secondary hex color string into a Unity Color.
        /// Returns gray if parsing fails.
        /// </summary>
        public Color GetSecondaryColor()
        {
            if (ColorUtility.TryParseHtmlString(secondaryColor, out Color color))
                return color;
            return Color.gray;
        }
    }
}
