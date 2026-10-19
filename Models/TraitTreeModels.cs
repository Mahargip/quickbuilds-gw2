using System.Collections.Generic;
using Newtonsoft.Json;

namespace QuickBuilds.Models
{
    public class SpecializationDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("profession")]
        public string Profession { get; set; } = string.Empty;

        [JsonProperty("elite")]
        public bool Elite { get; set; }

        [JsonProperty("minor_traits")]
        public List<int> MinorTraits { get; set; } = new List<int>();

        [JsonProperty("major_traits")]
        public List<int> MajorTraits { get; set; } = new List<int>();

        [JsonProperty("icon")]
        public string Icon { get; set; } = string.Empty;

        [JsonProperty("background")]
        public string Background { get; set; } = string.Empty;
    }

    public class TraitDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("icon")]
        public string Icon { get; set; } = string.Empty;

        [JsonProperty("tier")]
        public int Tier { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("slot")]
        public string Slot { get; set; } = "Major";
    }

    public class DecodedSpecialization
    {
        public int SpecializationId { get; set; }
        public int TraitChoice1 { get; set; } // 1=Top, 2=Mid, 3=Bot (0=none)
        public int TraitChoice2 { get; set; }
        public int TraitChoice3 { get; set; }
    }

    public class DecodedBuild
    {
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; } = string.Empty;
        public List<DecodedSpecialization> Specializations { get; set; } = new List<DecodedSpecialization>();
        public bool IsValid => Specializations != null && Specializations.Count == 3;
    }
}
