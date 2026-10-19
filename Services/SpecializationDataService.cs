using System;
using System.Collections.Generic;
using System.IO;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickBuilds.Models;

namespace QuickBuilds.Services
{
    public class SpecializationDataService
    {
        private static readonly Logger Logger = Logger.GetLogger<SpecializationDataService>();

        private readonly Dictionary<int, SpecializationDto> _specializations = new Dictionary<int, SpecializationDto>();
        private readonly Dictionary<int, TraitDto> _traits = new Dictionary<int, TraitDto>();
        private bool _isLoaded;

        public bool IsLoaded => _isLoaded;

        public void LoadData(ContentsManager? contentsManager, string moduleDir)
        {
            if (_isLoaded)
                return;

            try
            {
                string json = string.Empty;

                // 1. Try embedded resource (guaranteed to be present in the DLL)
                try
                {
                    using (var resStream = typeof(SpecializationDataService).Assembly.GetManifestResourceStream("QuickBuilds.trait_trees.json"))
                    {
                        if (resStream != null)
                        {
                            using (var reader = new StreamReader(resStream))
                            {
                                json = reader.ReadToEnd();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Could not load trait_trees.json from EmbeddedResource, trying ContentsManager.");
                }

                // 2. Try ContentsManager
                if (string.IsNullOrEmpty(json) && contentsManager != null)
                {
                    try
                    {
                        using (var stream = contentsManager.GetFileStream("trait_trees.json"))
                        {
                            if (stream != null)
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    json = reader.ReadToEnd();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Could not load trait_trees.json from ContentsManager, trying local file fallback.");
                    }
                }

                // 3. Try local file fallbacks
                if (string.IsNullOrEmpty(json))
                {
                    string localPath = Path.Combine(moduleDir, "trait_trees.json");
                    if (File.Exists(localPath))
                    {
                        json = File.ReadAllText(localPath);
                    }
                    else
                    {
                        string currentDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "trait_trees.json");
                        if (File.Exists(currentDirPath))
                        {
                            json = File.ReadAllText(currentDirPath);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(json))
                {
                    var root = JObject.Parse(json);

                    var specsArray = root["specializations"] as JArray;
                    if (specsArray != null)
                    {
                        foreach (var token in specsArray)
                        {
                            var spec = token.ToObject<SpecializationDto>();
                            if (spec != null)
                            {
                                _specializations[spec.Id] = spec;
                            }
                        }
                    }

                    var traitsObj = root["traits"] as JObject;
                    if (traitsObj != null)
                    {
                        foreach (var prop in traitsObj.Properties())
                        {
                            if (int.TryParse(prop.Name, out int traitId))
                            {
                                var trait = prop.Value.ToObject<TraitDto>();
                                if (trait != null)
                                {
                                    trait.Id = traitId;
                                    _traits[traitId] = trait;
                                }
                            }
                        }
                    }

                    _isLoaded = true;
                    Logger.Info($"Loaded {_specializations.Count} specializations and {_traits.Count} traits for QuickBuilds.");
                }
                else
                {
                    Logger.Warn("Could not locate trait_trees.json for QuickBuilds trait preview.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error loading specialization trait trees in QuickBuilds.");
            }
        }

        public SpecializationDto? GetSpecialization(int id)
        {
            _specializations.TryGetValue(id, out var spec);
            return spec;
        }

        public TraitDto? GetTrait(int id)
        {
            _traits.TryGetValue(id, out var trait);
            return trait;
        }
    }
}
