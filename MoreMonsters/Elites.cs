using System;
using System.Collections.Generic;
using System.Linq;
using ModCommon;
using R2API;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace MoreMonsters
{
    public class EliteEntry
    {
        public string Name;
        public string Prefix;
        public string Equip;
        public string Description;
        public Color Color;
        public BuffDef Buff;
        public EliteDef EliteDef;
    }

    // Neue Elite-Typen. Jeder Gegner kann als eine dieser Varianten spawnen.
    public static class Elites
    {
        public static readonly List<EliteEntry> All = new List<EliteEntry>
        {
            new EliteEntry { Name = "Vampiric", Equip = "Vampir", Prefix = "Vampirischer", Color = new Color(0.6f, 0.05f, 0.1f),
                Description = "Heilt sich um 40% des verursachten Schadens." },
            new EliteEntry { Name = "Storm", Equip = "Sturm", Prefix = "Stürmischer", Color = new Color(0.3f, 0.85f, 1f),
                Description = "Treffer lösen Kettenblitze aus." },
            new EliteEntry { Name = "Toxic", Equip = "Gift", Prefix = "Giftiger", Color = new Color(0.45f, 0.9f, 0.2f),
                Description = "Treffer vergiften." },
            new EliteEntry { Name = "Armored", Equip = "Panzer", Prefix = "Gepanzerter", Color = new Color(0.55f, 0.55f, 0.6f),
                Description = "Massive Rüstung, wirft Schaden zurück." },
            new EliteEntry { Name = "Frenzied", Equip = "Raserei", Prefix = "Rasender", Color = new Color(1f, 0.5f, 0.1f),
                Description = "Greift und bewegt sich viel schneller." },
            new EliteEntry { Name = "Volatile", Equip = "Instabilität", Prefix = "Instabiler", Color = new Color(1f, 0.85f, 0.2f),
                Description = "Explodiert beim Tod." },
        };

        private static GameObject explosion;

        public static void Register(Func<string, bool> enabled)
        {
            explosion = ModAssets.Load<GameObject>(AssetKeys.OmniExplosionVFX);
            var tiers = EliteAPI.GetEliteTierEnumerable(VanillaEliteTier.BaseTier1).Where(t => t != null).ToArray();

            foreach (EliteEntry entry in All.Where(e => enabled(e.Name)))
            {
                string token = "MOREMONSTERS_ELITE_" + entry.Name.ToUpperInvariant();
                Sprite icon = ModAssets.LoadIcon(typeof(Elites).Assembly, "Elite" + entry.Name);

                var buff = ScriptableObject.CreateInstance<BuffDef>();
                buff.name = "MoreMonstersAffix" + entry.Name;
                buff.buffColor = entry.Color;
                buff.iconSprite = icon;
                buff.canStack = false;
                buff.isDebuff = false;

                var equipment = ScriptableObject.CreateInstance<EquipmentDef>();
                equipment.name = "MoreMonstersAffix" + entry.Name;
                equipment.nameToken = token + "_EQUIP_NAME";
                equipment.pickupToken = token + "_EQUIP_PICKUP";
                equipment.descriptionToken = token + "_EQUIP_DESC";
                equipment.loreToken = token + "_EQUIP_LORE";
                equipment.pickupIconSprite = icon;
                equipment.pickupModelPrefab = ModAssets.Load<GameObject>(AssetKeys.PickupMystery);
                equipment.passiveBuffDef = buff;
                equipment.canDrop = false;
                equipment.dropOnDeathChance = 0.00025f;
                equipment.cooldown = 10f;
                equipment.colorIndex = ColorCatalog.ColorIndex.Equipment;

                var elite = new CustomElite("MoreMonsters" + entry.Name, equipment, entry.Color, token + "_MODIFIER", tiers, CreateRamp(entry.Color));
                elite.EliteDef.healthBoostCoefficient = 4f;
                elite.EliteDef.damageBoostCoefficient = 2f;
                buff.eliteDef = elite.EliteDef;

                LanguageAPI.Add(token + "_MODIFIER", entry.Prefix + " {0}");
                LanguageAPI.Add(equipment.nameToken, "Affix: " + entry.Equip);
                LanguageAPI.Add(equipment.pickupToken, entry.Description);
                LanguageAPI.Add(equipment.descriptionToken, "Werde zum Elite-Gegner. " + entry.Description);
                LanguageAPI.Add(equipment.loreToken, "");

                ContentAddition.AddBuffDef(buff);
                ItemAPI.Add(new CustomEquipment(equipment, new ItemDisplayRuleDict(null)));
                EliteAPI.Add(elite);

                entry.Buff = buff;
                entry.EliteDef = elite.EliteDef;
            }

            RecalculateStatsAPI.GetStatCoefficients += OnStats;
            GlobalEventManager.onServerDamageDealt += OnDamage;
            GlobalEventManager.onCharacterDeathGlobal += OnDeath;
        }

        // Farbverlauf, mit dem das Spiel Elite-Gegner einfärbt.
        private static Texture2D CreateRamp(Color color)
        {
            var ramp = new Texture2D(256, 8, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            for (int x = 0; x < 256; x++)
            {
                float t = x / 255f;
                Color c = t < 0.5f ? Color.Lerp(Color.black, color, t * 2f) : Color.Lerp(color, Color.white, (t - 0.5f) * 1.2f);
                for (int y = 0; y < 8; y++)
                    ramp.SetPixel(x, y, c);
            }
            ramp.Apply();
            return ramp;
        }

        private static bool Has(CharacterBody body, string name)
        {
            EliteEntry entry = All.FirstOrDefault(e => e.Name == name);
            return body && entry?.Buff && body.HasBuff(entry.Buff);
        }

        private static void OnStats(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (Has(body, "Armored"))
                args.armorAdd += 100f;
            if (Has(body, "Frenzied"))
            {
                args.attackSpeedMultAdd += 0.5f;
                args.moveSpeedMultAdd += 0.4f;
            }
        }

        private static void OnDamage(DamageReport report)
        {
            if (report.damageInfo.procCoefficient <= 0f)
                return;

            CharacterBody attacker = report.attackerBody;
            CharacterBody victim = report.victimBody;

            if (Has(attacker, "Vampiric") && attacker.healthComponent)
                attacker.healthComponent.Heal(report.damageDealt * 0.4f, default);

            if (Has(attacker, "Storm") && victim && victim.mainHurtBox)
            {
                OrbManager.instance.AddOrb(new LightningOrb
                {
                    origin = report.damageInfo.position,
                    target = victim.mainHurtBox,
                    attacker = attacker.gameObject,
                    damageValue = report.damageDealt * 0.5f,
                    teamIndex = attacker.teamComponent.teamIndex,
                    bouncesRemaining = 2,
                    range = 20f,
                    procCoefficient = 0f,
                    lightningType = LightningOrb.LightningType.Ukulele,
                    damageColorIndex = DamageColorIndex.Item,
                    bouncedObjects = new List<HealthComponent> { victim.healthComponent },
                });
            }

            if (Has(attacker, "Toxic") && victim)
            {
                var dot = new InflictDotInfo
                {
                    attackerObject = attacker.gameObject,
                    victimObject = victim.gameObject,
                    dotIndex = DotController.DotIndex.Poison,
                    duration = 2f,
                    damageMultiplier = 1f,
                };
                DotController.InflictDot(ref dot);
            }

            if (Has(victim, "Armored") && attacker && attacker != victim && attacker.healthComponent)
            {
                attacker.healthComponent.TakeDamage(new DamageInfo
                {
                    attacker = victim.gameObject,
                    inflictor = victim.gameObject,
                    damage = report.damageDealt * 0.1f,
                    position = attacker.corePosition,
                    procCoefficient = 0f,
                    damageColorIndex = DamageColorIndex.Item,
                });
            }
        }

        private static void OnDeath(DamageReport report)
        {
            CharacterBody victim = report.victimBody;
            if (!NetworkServer.active || !Has(victim, "Volatile"))
                return;

            Vector3 position = victim.corePosition;
            float radius = 6f + victim.radius;
            if (explosion)
                EffectManager.SpawnEffect(explosion, new EffectData { origin = position, scale = radius }, true);

            new BlastAttack
            {
                attacker = victim.gameObject,
                inflictor = victim.gameObject,
                teamIndex = victim.teamComponent ? victim.teamComponent.teamIndex : TeamIndex.Monster,
                position = position,
                radius = radius,
                baseDamage = victim.damage * 3f,
                baseForce = 1500f,
                procCoefficient = 0f,
                falloffModel = BlastAttack.FalloffModel.SweetSpot,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                damageColorIndex = DamageColorIndex.Item,
            }.Fire();
        }
    }
}
