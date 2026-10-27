using System.Collections.Generic;
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
        public const string PluginVersion = "0.1.0";

        private void Awake()
        {
            var items = new List<ItemBase>();
            items.AddRange(StatItems.All());
            items.Add(new VampireFang());
            items.Add(new ThornVest());
            items.Add(new ProspectorsPick());
            items.Add(new SoulCollector());
            items.Add(new EchoBlade());
            items.Add(new ExecutionersAxe());

            int registered = 0;
            foreach (ItemBase item in items)
            {
                if (!Config.Bind("Items", item.Name, true, $"Aktiviert: {item.DisplayName}").Value)
                    continue;

                item.Register();
                registered++;
            }

            Logger.LogInfo($"{PluginName} {PluginVersion}: {registered} Items registriert.");
        }
    }
}
