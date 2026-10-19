using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace QuickBuilds.Models
{
    public class BuildTemplateEntry
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("profession")]
        public string Profession { get; set; } = "Necromancer";

        [JsonProperty("elite_spec")]
        public string EliteSpec { get; set; } = string.Empty;

        [JsonProperty("chat_code")]
        public string ChatCode { get; set; } = string.Empty;

        [JsonProperty("category")]
        public string Category { get; set; } = "Open World";

        [JsonProperty("notes")]
        public string Notes { get; set; } = string.Empty;

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Color GetProfessionColor()
        {
            switch (Profession?.ToLowerInvariant())
            {
                case "guardian":
                    return new Color(80, 160, 230);
                case "warrior":
                    return new Color(255, 160, 40);
                case "engineer":
                    return new Color(180, 130, 80);
                case "ranger":
                    return new Color(110, 200, 70);
                case "thief":
                    return new Color(200, 100, 130);
                case "elementalist":
                    return new Color(235, 75, 75);
                case "mesmer":
                    return new Color(175, 95, 215);
                case "necromancer":
                    return new Color(80, 185, 100);
                case "revenant":
                    return new Color(185, 55, 70);
                default:
                    return Color.Gold;
            }
        }
    }
}
