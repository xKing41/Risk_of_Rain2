using System;
using System.Collections.Generic;
using System.Linq;
using ModCommon;
using R2API;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace MoreMonsters
{
    public class VariantDef
    {
        public string Name;
        public string DisplayName;
        public string Subtitle;
        public string Lore;
        public string BodyKey;
        public string MasterKey;
        public DirectorAPI.MonsterCategory Category;
        public float Scale = 1f;
        public float Health = 1f;
        public float Damage = 1f;
        public float Speed = 1f;
        public float AttackSpeed = 1f;
        public float Armor;
        public int Cost;
        public HullClassification Hull = HullClassification.Human;
        public int MinStage;

        public BodyIndex BodyIndex = BodyIndex.None;
    }

    // Neue Gegner und Bosse auf Basis vorhandener Monster: andere Größe, Werte und Rolle.
    public static class Variants
    {
        public static readonly List<VariantDef> All = new List<VariantDef>
        {
            // --- Bosse (Teleporter) ---
            new VariantDef { Name = "BeetleKing", DisplayName = "Käferkönig", Subtitle = "Herrscher des Schwarms",
                BodyKey = "e6d4f1c72713ae2479198584bfa637a4", MasterKey = "5459e8ded89cd0f4d84219750a7e10ac",
                Category = DirectorAPI.MonsterCategory.Champions, Scale = 2f, Health = 5f, Damage = 1.5f, Armor = 20f, Cost = 650, Hull = HullClassification.BeetleQueen },
            new VariantDef { Name = "AncientGolem", DisplayName = "Urgolem", Subtitle = "Wächter der ersten Welt",
                BodyKey = "f21a0264f74389246acbbc5e281092b1", MasterKey = "2da68637a35da9446b9cc3a09ab166b8",
                Category = DirectorAPI.MonsterCategory.Champions, Scale = 2.5f, Health = 7f, Damage = 1.8f, Speed = 0.9f, Armor = 30f, Cost = 700, Hull = HullClassification.BeetleQueen },
            new VariantDef { Name = "LemurianPatriarch", DisplayName = "Lemurianer-Patriarch", Subtitle = "Uralte Flamme",
                BodyKey = "aeb788f5fc372714a8827b167aee0594", MasterKey = "56ef7f71f643ea54892b80c0f4772529",
                Category = DirectorAPI.MonsterCategory.Champions, Scale = 1.6f, Health = 3.5f, Damage = 1.5f, Armor = 20f, Cost = 700, Hull = HullClassification.BeetleQueen, MinStage = 1 },
            new VariantDef { Name = "VoidTitan", DisplayName = "Leerentitan", Subtitle = "Das Ende aller Dinge",
                BodyKey = "4ac56d0b387fe344bba43f086ba72fe7", MasterKey = "aa843e29169d78e4e82c35028eb4459b",
                Category = DirectorAPI.MonsterCategory.Champions, Scale = 1.8f, Health = 3f, Damage = 1.5f, Armor = 20f, Cost = 900, Hull = HullClassification.BeetleQueen, MinStage = 2 },
            new VariantDef { Name = "ClayColossus", DisplayName = "Lehmkoloss", Subtitle = "Teer und Zorn",
                BodyKey = "a59e219f5c78ea949a7103a148d1ba0e", MasterKey = "b070af0233be17e4fa55293aff750bbe",
                Category = DirectorAPI.MonsterCategory.Champions, Scale = 2f, Health = 4.5f, Damage = 1.6f, Armor = 25f, Cost = 750, Hull = HullClassification.BeetleQueen, MinStage = 1 },

            // --- Mini-Bosse ---
            new VariantDef { Name = "GiantBeetle", DisplayName = "Riesenkäfer",
                BodyKey = "f2ae1aaca82b21c4e8a55edcf1312bc8", MasterKey = "42495d629c3f86d488591f2adcba8869",
                Category = DirectorAPI.MonsterCategory.Minibosses, Scale = 2.2f, Health = 6f, Damage = 2f, Speed = 0.9f, Armor = 15f, Cost = 110, Hull = HullClassification.Golem },
            new VariantDef { Name = "HellImp", DisplayName = "Höllenimp",
                BodyKey = "3e09a3664cb1cef47aad9e234268a20d", MasterKey = "134ac6c6522da1b41b12cef6bbabb293",
                Category = DirectorAPI.MonsterCategory.Minibosses, Scale = 1.5f, Health = 3f, Damage = 1.6f, Speed = 1.2f, AttackSpeed = 1.3f, Cost = 140, Hull = HullClassification.Golem, MinStage = 1 },
            new VariantDef { Name = "BisonAlpha", DisplayName = "Bison-Leittier",
                BodyKey = "93e318ca0f6b60840881c5aedaf03bca", MasterKey = "48248270686d92b4a9243ad0e14c4533",
                Category = DirectorAPI.MonsterCategory.Minibosses, Scale = 1.5f, Health = 2.5f, Damage = 1.5f, Speed = 1.2f, Armor = 20f, Cost = 120, Hull = HullClassification.Golem },
            new VariantDef { Name = "MushrumElder", DisplayName = "Pilzältester",
                BodyKey = "5744f0917b24e9d4bb5c894cec04b5a6", MasterKey = "2cad4d68df4291d45a9ba517ec953eb5",
                Category = DirectorAPI.MonsterCategory.Minibosses, Scale = 1.8f, Health = 3f, Damage = 1.5f, Armor = 15f, Cost = 110, Hull = HullClassification.Golem },

            // --- Normale Gegner ---
            new VariantDef { Name = "Pebblet", DisplayName = "Kieselgolem",
                BodyKey = "f21a0264f74389246acbbc5e281092b1", MasterKey = "2da68637a35da9446b9cc3a09ab166b8",
                Category = DirectorAPI.MonsterCategory.BasicMonsters, Scale = 0.5f, Health = 0.25f, Damage = 0.5f, Speed = 1.4f, AttackSpeed = 1.3f, Cost = 15, Hull = HullClassification.Human },
            new VariantDef { Name = "LemurianHatchling", DisplayName = "Lemurianer-Jungtier",
                BodyKey = "7dbddf7b5091ed5498164334ac1d43c7", MasterKey = "d224a5c371e9a444fada3230fde7a1c4",
                Category = DirectorAPI.MonsterCategory.BasicMonsters, Scale = 0.6f, Health = 0.5f, Damage = 0.6f, Speed = 1.3f, Cost = 8, Hull = HullClassification.Human },
            new VariantDef { Name = "ImpScout", DisplayName = "Imp-Späher",
                BodyKey = "3e09a3664cb1cef47aad9e234268a20d", MasterKey = "134ac6c6522da1b41b12cef6bbabb293",
                Category = DirectorAPI.MonsterCategory.BasicMonsters, Scale = 0.7f, Health = 0.5f, Damage = 0.7f, Speed = 1.5f, AttackSpeed = 1.2f, Cost = 20, Hull = HullClassification.Human, MinStage = 1 },
        };

        private static readonly Dictionary<BodyIndex, VariantDef> byIndex = new Dictionary<BodyIndex, VariantDef>();
        private static readonly Dictionary<string, VariantDef> byBodyName = new Dictionary<string, VariantDef>();

        public static void Register(Func<string, bool> enabled, BepInEx.Logging.ManualLogSource log)
        {
            foreach (VariantDef def in All.Where(d => enabled(d.Name)))
            {
                try
                {
                    Create(def);
                }
                catch (Exception e)
                {
                    log.LogError($"{def.Name} konnte nicht erstellt werden: {e}");
                }
            }

            // Größe über das Stat-System des Spiels; passt Modell und Kollision gemeinsam an.
            RecalculateStatsAPI.GetStatCoefficients += (body, args) =>
            {
                if (byIndex.TryGetValue(body.bodyIndex, out VariantDef def))
                {
                    args.desiredScaleAdd += def.Scale - 1f;
                    args.attackSpeedMultAdd += def.AttackSpeed - 1f;
                }
            };
            RoR2Application.onLoad += () =>
            {
                foreach (var pair in byBodyName)
                {
                    BodyIndex index = BodyCatalog.FindBodyIndex(pair.Key);
                    if (index != BodyIndex.None)
                        byIndex[index] = pair.Value;
                }
            };
        }

        private static void Create(VariantDef def)
        {
            GameObject originalBody = ModAssets.Load<GameObject>(def.BodyKey);
            GameObject originalMaster = ModAssets.Load<GameObject>(def.MasterKey);
            if (!originalBody || !originalMaster)
                throw new Exception("Vorlage nicht gefunden");

            string token = "MOREMONSTERS_" + def.Name.ToUpperInvariant();
            GameObject body = originalBody.InstantiateClone("MoreMonsters" + def.Name + "Body", true);
            GameObject master = originalMaster.InstantiateClone("MoreMonsters" + def.Name + "Master", true);
            master.GetComponent<CharacterMaster>().bodyPrefab = body;

            CharacterBody cb = body.GetComponent<CharacterBody>();
            cb.baseNameToken = token + "_NAME";
            cb.subtitleNameToken = token + "_SUBTITLE";
            cb.baseMaxHealth *= def.Health;
            cb.levelMaxHealth *= def.Health;
            cb.baseDamage *= def.Damage;
            cb.levelDamage *= def.Damage;
            cb.baseMoveSpeed *= def.Speed;
            cb.baseArmor += def.Armor;
            cb.isChampion = def.Category == DirectorAPI.MonsterCategory.Champions;

            LanguageAPI.Add(token + "_NAME", def.DisplayName);
            LanguageAPI.Add(token + "_SUBTITLE", def.Subtitle ?? "");

            ContentAddition.AddBody(body);
            ContentAddition.AddMaster(master);
            byBodyName[body.name] = def;

            var spawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            spawnCard.name = "cscMoreMonsters" + def.Name;
            spawnCard.prefab = master;
            spawnCard.sendOverNetwork = true;
            spawnCard.hullSize = def.Hull;
            spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
            spawnCard.requiredFlags = NodeFlags.None;
            spawnCard.forbiddenFlags = NodeFlags.NoCharacterSpawn;
            spawnCard.directorCreditCost = def.Cost;
            spawnCard.occupyPosition = false;

            var card = new DirectorCard
            {
                spawnCard = spawnCard,
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = def.MinStage,
            };
            DirectorAPI.Helpers.AddNewMonster(card, def.Category);
        }
    }
}
