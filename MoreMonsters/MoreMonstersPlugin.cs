using BepInEx;
using R2API;

namespace MoreMonsters
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(DirectorAPI.PluginGUID)]
    [BepInDependency(EliteAPI.PluginGUID)]
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    public class MoreMonstersPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "xking41.MoreMonsters";
        public const string PluginName = "MoreMonsters";
        public const string PluginVersion = "0.1.0";

        private void Awake()
        {
            Elites.Register(name => Config.Bind("Elites", name, true, $"Aktiviert den Elite-Typ {name}").Value);
            Variants.Register(name => Config.Bind("Monsters", name, true, $"Aktiviert {name}").Value, Logger);
            Logger.LogInfo($"{PluginName} {PluginVersion}: {Elites.All.Count} Elite-Typen, {Variants.All.Count} Gegner/Bosse.");
        }
    }
}
