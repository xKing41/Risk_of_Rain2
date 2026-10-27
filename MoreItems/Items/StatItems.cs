using System;
using System.Collections.Generic;
using R2API;
using RoR2;
using UnityEngine;

namespace MoreItems.Items
{
    // Item, das nur Werte verändert. Ein neues Stat-Item ist ein Eintrag in StatItems.All().
    public class StatItem : ItemBase
    {
        private readonly Action<RecalculateStatsAPI.StatHookEventArgs, int> apply;

        public override string Name { get; }
        public override string DisplayName { get; }
        public override string Pickup { get; }
        public override string Description { get; }
        public override ItemTier Tier { get; }
        public override ItemTag[] Tags { get; }

        public StatItem(string name, string displayName, ItemTier tier, ItemTag[] tags,
            string pickup, string description, Action<RecalculateStatsAPI.StatHookEventArgs, int> apply)
        {
            Name = name;
            DisplayName = displayName;
            Tier = tier;
            Tags = tags;
            Pickup = pickup;
            Description = description;
            this.apply = apply;
        }

        protected override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += (body, args) =>
            {
                int count = GetCount(body);
                if (count > 0)
                    apply(args, count);
            };
        }
    }

    public static class StatItems
    {
        private static readonly ItemTag[] Damage = { ItemTag.Damage };
        private static readonly ItemTag[] Utility = { ItemTag.Utility };
        private static readonly ItemTag[] Healing = { ItemTag.Healing };

        public static IEnumerable<StatItem> All() => new[]
        {
            // --- Gewöhnlich (weiß) ---
            new StatItem("RustyWhetstone", "Rostiger Wetzstein", ItemTier.Tier1, Damage,
                "Erhöht den Schaden.",
                "Erhöht den <style=cIsDamage>Basisschaden</style> um <style=cIsDamage>8%</style> <style=cStack>(+8% pro Stack)</style>.",
                (a, n) => a.damageMultAdd += 0.08f * n),

            new StatItem("FeatherBoots", "Federleichte Stiefel", ItemTier.Tier1, Utility,
                "Springe höher.",
                "Erhöht die <style=cIsUtility>Sprungkraft</style> um <style=cIsUtility>15%</style> <style=cStack>(+15% pro Stack)</style>.",
                (a, n) => a.jumpPowerMultAdd += 0.15f * n),

            new StatItem("CopperPlate", "Kupferplatte", ItemTier.Tier1, Utility,
                "Erhöht die Rüstung.",
                "Erhöht die <style=cIsHealing>Rüstung</style> um <style=cIsHealing>6</style> <style=cStack>(+6 pro Stack)</style>.",
                (a, n) => a.armorAdd += 6f * n),

            new StatItem("CoffeeBean", "Kaffeebohne", ItemTier.Tier1, Utility,
                "Etwas schneller angreifen und laufen.",
                "Erhöht die <style=cIsDamage>Angriffsgeschwindigkeit</style> um <style=cIsDamage>8%</style> <style=cStack>(+8% pro Stack)</style> und die <style=cIsUtility>Bewegungsgeschwindigkeit</style> um <style=cIsUtility>5%</style> <style=cStack>(+5% pro Stack)</style>.",
                (a, n) => { a.attackSpeedMultAdd += 0.08f * n; a.moveSpeedMultAdd += 0.05f * n; }),

            new StatItem("LuckyLadybug", "Glücksmarienkäfer", ItemTier.Tier1, Damage,
                "Erhöht die kritische Trefferchance.",
                "Erhöht die <style=cIsDamage>kritische Trefferchance</style> um <style=cIsDamage>5%</style> <style=cStack>(+5% pro Stack)</style>.",
                (a, n) => a.critAdd += 5f * n),

            new StatItem("HerbalTea", "Kräutertee", ItemTier.Tier1, Healing,
                "Regeneriere Leben.",
                "Erhöht die <style=cIsHealing>Lebensregeneration</style> um <style=cIsHealing>1.2 LP/s</style> <style=cStack>(+1.2 LP/s pro Stack)</style>.",
                (a, n) => a.baseRegenAdd += 1.2f * n),

            new StatItem("BandageRoll", "Verbandsrolle", ItemTier.Tier1, Healing,
                "Erhöht das maximale Leben.",
                "Erhöht das <style=cIsHealing>maximale Leben</style> um <style=cIsHealing>20</style> <style=cStack>(+20 pro Stack)</style>.",
                (a, n) => a.baseHealthAdd += 20f * n),

            // --- Selten (grün) ---
            new StatItem("HourglassShard", "Sanduhrsplitter", ItemTier.Tier2, Utility,
                "Verringert alle Abklingzeiten.",
                "Verringert alle <style=cIsUtility>Abklingzeiten</style> um <style=cIsUtility>8%</style> <style=cStack>(+8% pro Stack, multiplikativ)</style>.",
                (a, n) => a.cooldownMultAdd -= 1f - Mathf.Pow(0.92f, n)),

            new StatItem("ShieldBattery", "Schildbatterie", ItemTier.Tier2, Utility,
                "Erhalte einen aufladbaren Schild.",
                "Erhalte einen <style=cIsHealing>Schild</style> mit <style=cIsHealing>40</style> <style=cStack>(+40 pro Stack)</style> Punkten.",
                (a, n) => a.baseShieldAdd += 40f * n),

            new StatItem("BerserkerTooth", "Berserkerzahn", ItemTier.Tier2, Damage,
                "Mehr Schaden und Tempo, aber weniger Rüstung.",
                "Erhöht den <style=cIsDamage>Schaden</style> um <style=cIsDamage>15%</style> und die <style=cIsDamage>Angriffsgeschwindigkeit</style> um <style=cIsDamage>12%</style> <style=cStack>(je +15%/+12% pro Stack)</style>. Verringert die Rüstung um <style=cDeath>5</style> <style=cStack>(+5 pro Stack)</style>.",
                (a, n) => { a.damageMultAdd += 0.15f * n; a.attackSpeedMultAdd += 0.12f * n; a.armorAdd -= 5f * n; }),

            // --- Legendär (rot) ---
            new StatItem("TitansHeart", "Herz des Titanen", ItemTier.Tier3, Healing,
                "Massiv mehr Leben.",
                "Erhöht das <style=cIsHealing>maximale Leben</style> um <style=cIsHealing>60%</style> <style=cStack>(+60% pro Stack)</style>.",
                (a, n) => a.healthMultAdd += 0.6f * n),

            new StatItem("StormCrown", "Krone des Sturms", ItemTier.Tier3, Utility,
                "Massiv schneller angreifen und laufen.",
                "Erhöht die <style=cIsDamage>Angriffsgeschwindigkeit</style> und die <style=cIsUtility>Bewegungsgeschwindigkeit</style> um je <style=cIsUtility>30%</style> <style=cStack>(+30% pro Stack)</style>.",
                (a, n) => { a.attackSpeedMultAdd += 0.3f * n; a.moveSpeedMultAdd += 0.3f * n; }),
        };
    }
}
