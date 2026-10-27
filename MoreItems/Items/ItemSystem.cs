using System.Collections.Generic;
using System.Linq;
using ModCommon;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MoreItems.Items
{
    // Registriert alle Items und verteilt die Spiel-Events an sie.
    public static class ItemSystem
    {
        public static readonly List<ModItem> Registered = new List<ModItem>();

        private static List<ModItem> stat, onHit, onKill, onHurt, conditional, pulse;

        public static void Register(ModItem item)
        {
            string token = "MOREITEMS_" + item.Name.ToUpperInvariant();
            item.Icon = ModAssets.LoadIcon(typeof(ItemSystem).Assembly, item.Name)
                        ?? ModAssets.Load<Sprite>(AssetKeys.MysteryIcon);

            var def = ScriptableObject.CreateInstance<ItemDef>();
            def.name = "MoreItems" + item.Name;
            def.nameToken = token + "_NAME";
            def.pickupToken = token + "_PICKUP";
            def.descriptionToken = token + "_DESC";
            def.loreToken = token + "_LORE";
#pragma warning disable CS0618 // deprecatedTier ist der von R2API unterstützte Weg, die Seltenheit zu setzen
            def.deprecatedTier = item.Tier;
#pragma warning restore CS0618
            def.tags = item.Tags;
            def.canRemove = true;
            def.hidden = false;
            def.pickupIconSprite = item.Icon;
            def.pickupModelPrefab = ModAssets.Load<GameObject>(AssetKeys.PickupMystery);

            LanguageAPI.Add(def.nameToken, item.DisplayName);
            LanguageAPI.Add(def.pickupToken, item.Pickup);
            LanguageAPI.Add(def.descriptionToken, item.Description);
            LanguageAPI.Add(def.loreToken, "");

            ItemAPI.Add(new CustomItem(def, new ItemDisplayRuleDict(null)));
            item.ItemDef = def;

            if (item.Kind == Kind.Conditional)
            {
                var buff = ScriptableObject.CreateInstance<BuffDef>();
                buff.name = "MoreItems" + item.Name + "Active";
                buff.iconSprite = item.Icon;
                buff.buffColor = Color.white;
                buff.canStack = false;
                buff.isDebuff = false;
                ContentAddition.AddBuffDef(buff);
                item.ConditionBuff = buff;
            }

            Registered.Add(item);
        }

        public static void Hook()
        {
            stat = Registered.Where(i => i.Kind == Kind.Stat).ToList();
            onHit = Registered.Where(i => i.Kind == Kind.OnHit).ToList();
            onKill = Registered.Where(i => i.Kind == Kind.OnKill).ToList();
            onHurt = Registered.Where(i => i.Kind == Kind.OnHurt).ToList();
            conditional = Registered.Where(i => i.Kind == Kind.Conditional).ToList();
            pulse = Registered.Where(i => i.Kind == Kind.Pulse).ToList();

            RecalculateStatsAPI.GetStatCoefficients += OnStats;
            GlobalEventManager.onServerDamageDealt += OnDamageDealt;
            GlobalEventManager.onCharacterDeathGlobal += OnDeath;
            CharacterBody.onBodyStartGlobal += body =>
            {
                if (NetworkServer.active && (conditional.Count > 0 || pulse.Count > 0))
                    body.gameObject.AddComponent<ItemTracker>();
            };
        }

        private static void OnStats(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (!body.inventory)
                return;

            foreach (ModItem item in stat)
            {
                int count = item.Count(body);
                if (count > 0)
                    item.Stats(body, args, count);
            }

            foreach (ModItem item in conditional)
            {
                int count = item.Count(body);
                if (count > 0 && body.HasBuff(item.ConditionBuff))
                    item.Stats(body, args, count);
            }
        }

        private static void OnDamageDealt(DamageReport report)
        {
            // Zusatzschaden unserer Items hat procCoefficient 0 und löst daher keine weiteren Items aus.
            float proc = report.damageInfo.procCoefficient;
            if (proc <= 0f)
                return;

            CharacterBody attacker = report.attackerBody;
            CharacterBody victim = report.victimBody;

            if (attacker && attacker.inventory && victim)
            {
                foreach (ModItem item in onHit)
                {
                    int count = item.Count(attacker);
                    if (count > 0 && Util.CheckRoll(Mathf.Min(100f, item.Chance) * proc, attacker.master))
                        item.OnDamage(attacker, victim, report, count);
                }
            }

            if (victim && victim.inventory && attacker != victim)
            {
                foreach (ModItem item in onHurt)
                {
                    int count = item.Count(victim);
                    if (count > 0 && Util.CheckRoll(Mathf.Min(100f, item.Chance), victim.master))
                        item.OnDamage(attacker, victim, report, count);
                }
            }
        }

        private static void OnDeath(DamageReport report)
        {
            CharacterBody killer = report.attackerBody;
            if (!NetworkServer.active || !killer || !killer.inventory)
                return;

            foreach (ModItem item in onKill)
            {
                int count = item.Count(killer);
                if (count > 0 && Util.CheckRoll(Mathf.Min(100f, item.Chance), killer.master))
                    item.OnKill(killer, report, count);
            }
        }

        // Läuft auf dem Server an jedem Körper und kümmert sich um Bedingungs- und Intervall-Items.
        private class ItemTracker : MonoBehaviour
        {
            private CharacterBody body;
            private readonly Dictionary<ModItem, float> timers = new Dictionary<ModItem, float>();

            private void Awake() => body = GetComponent<CharacterBody>();

            private void FixedUpdate()
            {
                if (!body || !body.inventory || !body.healthComponent || !body.healthComponent.alive)
                    return;

                foreach (ModItem item in conditional)
                {
                    bool active = item.Count(body) > 0 && item.Condition(body);
                    bool has = body.HasBuff(item.ConditionBuff);
                    if (active && !has)
                        body.AddBuff(item.ConditionBuff);
                    else if (!active && has)
                        body.RemoveBuff(item.ConditionBuff);
                }

                foreach (ModItem item in pulse)
                {
                    int count = item.Count(body);
                    if (count <= 0)
                        continue;

                    timers.TryGetValue(item, out float timer);
                    timer += Time.fixedDeltaTime;
                    if (timer >= item.Interval)
                    {
                        timer = 0f;
                        item.OnPulse(body, count);
                    }
                    timers[item] = timer;
                }
            }
        }
    }
}
