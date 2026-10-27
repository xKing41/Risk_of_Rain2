using BepInEx;
using MoreItems.Items;
using R2API;

namespace MoreItems
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    public class MoreItemsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "xking41.MoreItems";
        public const string PluginName = "MoreItems";
        public const string PluginVersion = "0.2.0";

        private void Awake()
        {
            Fx.Load();

            foreach (ModItem item in ItemList.All())
            {
                if (Config.Bind("Items", item.Name, true, $"Aktiviert: {item.DisplayName}").Value)
                    ItemSystem.Register(item);
            }

            ItemSystem.Hook();
            ItemList.HookDrawbacks();
            Logger.LogInfo($"{PluginName} {PluginVersion}: {ItemSystem.Registered.Count} Items registriert.");
        }
    }
}
