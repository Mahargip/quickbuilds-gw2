using System;
using System.Collections.Generic;
using QuickBuilds.Models;

namespace QuickBuilds.Services
{
    public static class BuildChatCodeDecoder
    {
        private static readonly Dictionary<int, string> ProfessionNames = new Dictionary<int, string>
        {
            { 1, "Guardian" },
            { 2, "Warrior" },
            { 3, "Engineer" },
            { 4, "Ranger" },
            { 5, "Thief" },
            { 6, "Elementalist" },
            { 7, "Mesmer" },
            { 8, "Necromancer" },
            { 9, "Revenant" }
        };

        public static DecodedBuild? Decode(string? chatCode)
        {
            if (string.IsNullOrWhiteSpace(chatCode))
                return null;

            string cleaned = chatCode!.Trim();
            if (cleaned.StartsWith("[&") && cleaned.EndsWith("]"))
            {
                cleaned = cleaned.Substring(2, cleaned.Length - 3).Trim();
            }

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(cleaned);
            }
            catch
            {
                return null;
            }

            // Must have header 0x0D (13) and at least 8 bytes for header, prof, and 3 specs (2 bytes each)
            if (bytes.Length < 8 || bytes[0] != 0x0D)
            {
                return null;
            }

            int profId = bytes[1];
            string profName = ProfessionNames.TryGetValue(profId, out var name) ? name : "Unknown";

            var result = new DecodedBuild
            {
                ProfessionId = profId,
                ProfessionName = profName
            };

            for (int i = 0; i < 3; i++)
            {
                int specOffset = 2 + (i * 2);
                int specId = bytes[specOffset];
                byte traitsByte = bytes[specOffset + 1];

                int choice1 = traitsByte & 0x03;
                int choice2 = (traitsByte >> 2) & 0x03;
                int choice3 = (traitsByte >> 4) & 0x03;

                result.Specializations.Add(new DecodedSpecialization
                {
                    SpecializationId = specId,
                    TraitChoice1 = choice1,
                    TraitChoice2 = choice2,
                    TraitChoice3 = choice3
                });
            }

            return result;
        }
    }
}
