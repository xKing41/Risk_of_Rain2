using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using HG;
using ModCommon;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace MoreSkills
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(R2API.ContentManagement.R2APIContentManager.PluginGUID)]
    public class MoreSkillsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "xking41.MoreSkills";
        public const string PluginName = "MoreSkills";
        public const string PluginVersion = "0.1.0";

        // Andere Mods (z. B. PlayableMonsters) können hierüber Skills für eigene Slots holen.
        public static readonly Dictionary<string, SkillDef> SkillDefs = new Dictionary<string, SkillDef>();

        private ConfigEntry<bool> addToAllSurvivors;
        private readonly Dictionary<SkillDef, Slot> slots = new Dictionary<SkillDef, Slot>();

        private void Awake()
        {
            addToAllSurvivors = Config.Bind("General", "AddToAllSurvivors", true,
                "Gibt jedem Survivor (auch aus anderen Mods) alle neuen Skills zur Auswahl.");

            SkillAssets.Load();
            CreateBerserkBuff();

            foreach (SkillEntry entry in SkillCatalogEntries.All)
            {
                if (!Config.Bind("Skills", entry.Name, true, $"Aktiviert: {entry.DisplayName}").Value)
                    continue;

                SkillDef def = CreateSkillDef(entry);
                SkillDefs[entry.Name] = def;
                slots[def] = entry.Slot;
            }

            if (addToAllSurvivors.Value)
                RoR2Application.onLoad += AddSkillsToSurvivors;

            Logger.LogInfo($"{PluginName} {PluginVersion}: {SkillDefs.Count} Skills registriert.");
        }

        private SkillDef CreateSkillDef(SkillEntry entry)
        {
            ContentAddition.AddEntityState(entry.State, out _);

            string token = "MORESKILLS_" + entry.Name.ToUpperInvariant();
            var def = ScriptableObject.CreateInstance<SkillDef>();
            ((ScriptableObject)def).name = "MoreSkills" + entry.Name;
            def.skillName = "MoreSkills" + entry.Name;
            def.skillNameToken = token + "_NAME";
            def.skillDescriptionToken = token + "_DESC";
            def.icon = ModAssets.LoadIcon(typeof(MoreSkillsPlugin).Assembly, entry.Name);
            def.activationState = new SerializableEntityStateType(entry.State);
            def.activationStateMachineName = entry.Machine;
            def.baseRechargeInterval = entry.Cooldown;
            def.baseMaxStock = entry.Stocks;
            def.rechargeStock = 1;
            def.requiredStock = 1;
            def.stockToConsume = entry.Slot == Slot.Primary ? 0 : 1;
            def.beginSkillCooldownOnSkillEnd = false;
            def.canceledFromSprinting = false;
            def.cancelSprintingOnActivation = entry.Slot != Slot.Utility;
            def.fullRestockOnAssign = true;
            def.interruptPriority = entry.Priority;
            def.isCombatSkill = entry.Slot != Slot.Utility;
            def.mustKeyPress = entry.Slot != Slot.Primary;
            def.resetCooldownTimerOnUse = false;

            LanguageAPI.Add(def.skillNameToken, entry.DisplayName);
            LanguageAPI.Add(def.skillDescriptionToken, entry.Description);
            ContentAddition.AddSkillDef(def);
            return def;
        }

        private void CreateBerserkBuff()
        {
            var buff = ScriptableObject.CreateInstance<BuffDef>();
            buff.name = "MoreSkillsBerserk";
            buff.buffColor = new Color(1f, 0.35f, 0.2f);
            buff.canStack = false;
            buff.isDebuff = false;
            buff.iconSprite = ModAssets.LoadIcon(typeof(MoreSkillsPlugin).Assembly, "Berserk");
            ContentAddition.AddBuffDef(buff);
            SkillAssets.BerserkBuff = buff;

            RecalculateStatsAPI.GetStatCoefficients += (body, args) =>
            {
                if (body.HasBuff(buff))
                {
                    args.attackSpeedMultAdd += 0.5f;
                    args.moveSpeedMultAdd += 0.3f;
                }
            };
        }

        // Läuft, wenn alle Mods geladen sind. Damit bekommen auch Survivors aus anderen Mods die Skills.
        private void AddSkillsToSurvivors()
        {
            var done = new HashSet<SkillFamily>();
            int survivors = 0;

            foreach (SurvivorDef survivor in SurvivorCatalog.allSurvivorDefs)
            {
                if (!survivor || !survivor.bodyPrefab)
                    continue;

                SkillLocator locator = survivor.bodyPrefab.GetComponent<SkillLocator>();
                if (!locator)
                    continue;

                var machines = new HashSet<string>(survivor.bodyPrefab.GetComponents<EntityStateMachine>().Select(m => m.customName));
                AddToFamily(locator.primary, Slot.Primary, machines, done);
                AddToFamily(locator.secondary, Slot.Secondary, machines, done);
                AddToFamily(locator.utility, Slot.Utility, machines, done);
                AddToFamily(locator.special, Slot.Special, machines, done);
                survivors++;
            }

            Logger.LogInfo($"Skills zu {survivors} Survivors hinzugefügt.");
        }

        private void AddToFamily(GenericSkill skill, Slot slot, HashSet<string> machines, HashSet<SkillFamily> done)
        {
            SkillFamily family = skill ? skill.skillFamily : null;
            if (!family || !done.Add(family))
                return;

            foreach (var pair in slots)
            {
                SkillDef def = pair.Key;
                if (pair.Value != slot || !machines.Contains(def.activationStateMachineName))
                    continue;
                if (family.variants.Any(v => v.skillDef == def))
                    continue;

                ArrayUtils.ArrayAppend(ref family.variants, new SkillFamily.Variant
                {
                    skillDef = def,
                    unlockableDef = null,
                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null),
                });
            }
        }
    }
}
