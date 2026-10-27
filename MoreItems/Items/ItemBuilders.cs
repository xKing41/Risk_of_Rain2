using System;
using R2API;
using RoR2;

namespace MoreItems.Items
{
    // Kurzschreibweisen, damit ein Item in ItemList nur ein paar Zeilen braucht.
    public static class B
    {
        public const ItemTier T1 = ItemTier.Tier1, T2 = ItemTier.Tier2, T3 = ItemTier.Tier3, Boss = ItemTier.Boss, Lunar = ItemTier.Lunar;

        public static string D(object s) => $"<style=cIsDamage>{s}</style>";
        public static string U(object s) => $"<style=cIsUtility>{s}</style>";
        public static string H(object s) => $"<style=cIsHealing>{s}</style>";
        public static string S(object s) => $"<style=cStack>({s})</style>";
        public static string X(object s) => $"<style=cDeath>{s}</style>";

        private static ModItem Base(string name, string display, ItemTier tier, string symbol, string pickup, string desc, Kind kind, ItemTag[] tags)
            => new ModItem { Name = name, DisplayName = display, Tier = tier, Symbol = symbol, Pickup = pickup, Description = desc, Kind = kind, Tags = tags };

        public static ModItem Stat(string name, string display, ItemTier tier, string symbol, string pickup, string desc,
            Action<RecalculateStatsAPI.StatHookEventArgs, int> stats, params ItemTag[] tags)
        {
            ModItem item = Base(name, display, tier, symbol, pickup, desc, Kind.Stat, tags);
            item.Stats = (body, args, n) => stats(args, n);
            return item;
        }

        public static ModItem StatB(string name, string display, ItemTier tier, string symbol, string pickup, string desc,
            Action<CharacterBody, RecalculateStatsAPI.StatHookEventArgs, int> stats, params ItemTag[] tags)
        {
            ModItem item = Base(name, display, tier, symbol, pickup, desc, Kind.Stat, tags);
            item.Stats = stats;
            return item;
        }

        public static ModItem Hit(string name, string display, ItemTier tier, string symbol, string pickup, string desc, float chance,
            Action<CharacterBody, CharacterBody, DamageReport, int> onHit, params ItemTag[] tags)
        {
            ModItem item = Base(name, display, tier, symbol, pickup, desc, Kind.OnHit, tags);
            item.Chance = chance;
            item.OnDamage = onHit;
            return item;
        }

        public static ModItem Kill(string name, string display, ItemTier tier, string symbol, string pickup, string desc, float chance,
            Action<CharacterBody, DamageReport, int> onKill, params ItemTag[] tags)
        {
            ModItem item = Base(name, display, tier, symbol, pickup, desc, Kind.OnKill, tags);
            item.Chance = chance;
            item.OnKill = onKill;
            return item;
        }

        public static ModItem Hurt(string name, string display, ItemTier tier, string symbol, string pickup, string desc, float chance,
            Action<CharacterBody, CharacterBody, DamageReport, int> onHurt, params ItemTag[] tags)
        {
            ModItem item = Base(name, display, tier, symbol, pickup, desc, Kind.OnHurt, tags);
            item.Chance = chance;
            item.OnDamage = onHurt;
            return item;
        }

        public static ModItem Cond(string name, string display, ItemTier tier, string symbol, string pickup, string desc,
            Func<CharacterBody, bool> condition, Action<RecalculateStatsAPI.StatHookEventArgs, int> stats, params ItemTag[] tags)
        {
            ModItem item = Base(name, display, tier, symbol, pickup, desc, Kind.Conditional, tags);
            item.Condition = condition;
            item.Stats = (body, args, n) => stats(args, n);
            return item;
        }

        public static ModItem Pulse(string name, string display, ItemTier tier, string symbol, string pickup, string desc, float interval,
            Action<CharacterBody, int> onPulse, params ItemTag[] tags)
        {
            ModItem item = Base(name, display, tier, symbol, pickup, desc, Kind.Pulse, tags);
            item.Interval = interval;
            item.OnPulse = onPulse;
            return item;
        }

        // Multiplikatives Stapeln für Nachteile: 0.5 → 50%, 25%, 12.5% ...
        public static float Mult(float perStack, int n) => (float)Math.Pow(perStack, n);
    }
}
