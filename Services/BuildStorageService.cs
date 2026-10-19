using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blish_HUD;
using Newtonsoft.Json;
using QuickBuilds.Models;

namespace QuickBuilds.Services
{
    public class BuildStorageService
    {
        private static readonly Logger Logger = Logger.GetLogger<BuildStorageService>();
        private readonly List<BuildTemplateEntry> _builds = new List<BuildTemplateEntry>();
        private string _filePath = string.Empty;

        public event EventHandler? BuildsChanged;

        public IReadOnlyList<BuildTemplateEntry> AllBuilds => _builds.AsReadOnly();

        public void Initialize(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                _filePath = Path.Combine(directoryPath, "builds.json");

                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    var loaded = JsonConvert.DeserializeObject<List<BuildTemplateEntry>>(json);
                    if (loaded != null && loaded.Count > 0)
                    {
                        _builds.Clear();
                        _builds.AddRange(loaded);
                        Logger.Info($"Loaded {_builds.Count} saved build templates.");
                        BuildsChanged?.Invoke(this, EventArgs.Empty);
                        return;
                    }
                }

                // If file didn't exist or was empty, provide initial popular builds
                PopulateDefaultBuilds();
                Save();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to initialize QuickBuilds storage.");
            }
        }

        public void Add(BuildTemplateEntry entry)
        {
            if (entry == null) return;

            entry.Id = string.IsNullOrEmpty(entry.Id) ? Guid.NewGuid().ToString() : entry.Id;
            entry.CreatedAt = DateTime.UtcNow;
            entry.UpdatedAt = DateTime.UtcNow;

            _builds.Insert(0, entry);
            Save();
            BuildsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Update(BuildTemplateEntry entry)
        {
            if (entry == null) return;

            int index = _builds.FindIndex(b => b.Id == entry.Id);
            if (index >= 0)
            {
                entry.UpdatedAt = DateTime.UtcNow;
                _builds[index] = entry;
                Save();
                BuildsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Delete(string id)
        {
            int removed = _builds.RemoveAll(b => b.Id == id);
            if (removed > 0)
            {
                Save();
                BuildsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public IReadOnlyList<BuildTemplateEntry> Filter(string? profession, string? category, string? searchQuery)
        {
            IEnumerable<BuildTemplateEntry> result = _builds;

            if (!string.IsNullOrWhiteSpace(profession) && !string.Equals(profession, "All", StringComparison.OrdinalIgnoreCase))
            {
                result = result.Where(b => b.Profession != null && b.Profession.Equals(profession, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(category) && !string.Equals(category, "All", StringComparison.OrdinalIgnoreCase))
            {
                result = result.Where(b => b.Category != null && b.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string query = searchQuery!.Trim();
                result = result.Where(b =>
                    (!string.IsNullOrEmpty(b.Title) && b.Title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(b.EliteSpec) && b.EliteSpec.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(b.Notes) && b.Notes.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(b.ChatCode) && b.ChatCode.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }

            return result.OrderByDescending(b => b.UpdatedAt).ToList();
        }

        public void Save()
        {
            try
            {
                if (string.IsNullOrEmpty(_filePath)) return;

                string json = JsonConvert.SerializeObject(_builds, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to save builds to disk.");
            }
        }

        public string ExportJson()
        {
            return JsonConvert.SerializeObject(_builds, Formatting.Indented);
        }

        public bool ImportJson(string json, bool replace = false)
        {
            try
            {
                var imported = JsonConvert.DeserializeObject<List<BuildTemplateEntry>>(json);
                if (imported == null || imported.Count == 0) return false;

                if (replace)
                {
                    _builds.Clear();
                }

                foreach (var item in imported)
                {
                    if (string.IsNullOrEmpty(item.Id) || _builds.Any(b => b.Id == item.Id))
                    {
                        item.Id = Guid.NewGuid().ToString();
                    }
                    _builds.Add(item);
                }

                Save();
                BuildsChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to import builds JSON.");
                return false;
            }
        }

        private void PopulateDefaultBuilds()
        {
            _builds.Clear();

            // Necromancer - Condi Scourge
            _builds.Add(new BuildTemplateEntry
            {
                Title = "Condi Scourge - DPS",
                Profession = "Necromancer",
                EliteSpec = "Scourge",
                Category = "Raid",
                ChatCode = "[&DQk1KTIlPCp+AAAA1gAAACgBAABHAQAAlwEAAAAAAAAAAAAAAAAAAAAAAAA=]",
                Notes = "Gear: Full Viper armor and trinkets.\nWeapons: Scepter / Torch + Pistol.\nRunes: Superior Rune of the Tormenting."
            });

            // Necromancer - Power Reaper
            _builds.Add(new BuildTemplateEntry
            {
                Title = "Power Reaper - Open World",
                Profession = "Necromancer",
                EliteSpec = "Reaper",
                Category = "Open World",
                ChatCode = "[&DQg1KTIlIhabAAAAvgAAAP4AAACbAAAAEgEAAAAAAAAAAAAAAAAAAAAAAAA=]",
                Notes = "Gear: Full Berserker with Scholar Runes.\nWeapons: Greatsword + Staff / Axe+Warhorn.\nHigh self-boon uptime (Might & Quickness in shroud)."
            });

            // Guardian - Quickness Firebrand
            _builds.Add(new BuildTemplateEntry
            {
                Title = "Condi Quick Firebrand",
                Profession = "Guardian",
                EliteSpec = "Firebrand",
                Category = "Fractal",
                ChatCode = "[&DQEQGg0bPi15AHkAcgBhABEBWgFLAVcBiwCLAAAAAAAAAAAAAAAAAAAAAAA=]",
                Notes = "Gear: Ritualist / Viper mix to reach 100% Quickness duration.\nWeapons: Axe / Torch + Staff."
            });

            // Engineer - Power Mechanist
            _builds.Add(new BuildTemplateEntry
            {
                Title = "Power Mechanist - Easy DPS",
                Profession = "Engineer",
                EliteSpec = "Mechanist",
                Category = "Open World",
                ChatCode = "[&DQQMGyYvRh4pDwAAvgEAABkBAABUAQAA8BUAAAAAAAAAAAAAAAAAAAAAAAA=]",
                Notes = "Gear: Full Berserker with Eagle or Scholar Runes.\nWeapons: Rifle.\nSimple low-intensity rotation with mech tanking."
            });

            // Revenant - Quickness Herald
            _builds.Add(new BuildTemplateEntry
            {
                Title = "Power Quickness Herald",
                Profession = "Revenant",
                EliteSpec = "Herald",
                Category = "Strike",
                ChatCode = "[&DQkDPg8aNDrcEdwRBhIGEtoS2hHUESsSEhISEgAAAAAAAAAAAAAAAAAAAAA=]",
                Notes = "Gear: Berserker + Diviner mix for boon duration.\nWeapons: Greatsword + Swords."
            });
        }
    }
}
