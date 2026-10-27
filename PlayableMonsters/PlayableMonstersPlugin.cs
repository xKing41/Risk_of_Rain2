using System.Linq;
using System.Reflection;
using BepInEx;
using EntityStates;
using HG;
using ModCommon;
using MoreSkills;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace PlayableMonsters
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(MoreSkillsPlugin.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    public class PlayableMonstersPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "xking41.PlayableMonsters";
        public const string PluginName = "PlayableMonsters";
        public const string PluginVersion = "0.1.0";

        private static readonly FieldInfo SkillFamilyField = typeof(GenericSkill).GetField("_skillFamily", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo StateMachinesField = typeof(NetworkStateMachine).GetField("stateMachines", BindingFlags.Instance | BindingFlags.NonPublic);

        private GameObject commandoBody;

        private void Awake()
        {
            commandoBody = ModAssets.Load<GameObject>("64ee0a4463fdfdc41ac7a06c8f5f2f0f");

            int sort = 200;
            int created = 0;
            foreach (MonsterDef def in MonsterDefs.All)
            {
                if (!Config.Bind("Survivors", def.Name, true, $"Aktiviert: {def.DisplayName}").Value)
                    continue;

                try
                {
                    if (CreateSurvivor(def, sort++))
                        created++;
                }
                catch (System.Exception e)
                {
                    Logger.LogError($"{def.Name} konnte nicht erstellt werden: {e}");
                }
            }

            Logger.LogInfo($"{PluginName} {PluginVersion}: {created} Survivors erstellt.");
        }

        private bool CreateSurvivor(MonsterDef def, int sortPosition)
        {
            GameObject original = ModAssets.Load<GameObject>(def.BodyKey);
            if (!original)
            {
                Logger.LogWarning($"{def.Name}: Body nicht gefunden, übersprungen.");
                return false;
            }

            GameObject body = original.InstantiateClone("XKPlayable" + def.Name + "Body", true);
            string token = "PLAYABLEMONSTERS_" + def.Name.ToUpperInvariant();

            SetupBody(body, def, token);
            SetupPlayerComponents(body, def);
            SetupSkills(body, def);

            GameObject display = CreateDisplay(body, def);

            var survivor = ScriptableObject.CreateInstance<SurvivorDef>();
            ((ScriptableObject)survivor).name = "XKPlayable" + def.Name;
            survivor.cachedName = "XKPlayable" + def.Name;
            survivor.bodyPrefab = body;
            survivor.displayPrefab = display;
            survivor.displayNameToken = token + "_NAME";
            survivor.descriptionToken = token + "_DESCRIPTION";
            survivor.outroFlavorToken = token + "_OUTRO";
            survivor.mainEndingEscapeFailureFlavorToken = token + "_FAIL";
            survivor.primaryColor = def.Color;
            survivor.desiredSortPosition = sortPosition;
            survivor.hidden = false;

            LanguageAPI.Add(token + "_NAME", def.DisplayName);
            LanguageAPI.Add(token + "_SUBTITLE", def.Subtitle);
            LanguageAPI.Add(token + "_DESCRIPTION", def.Description);
            LanguageAPI.Add(token + "_OUTRO", "...und so verschwand das Monster, das ein Held wurde.");
            LanguageAPI.Add(token + "_FAIL", "...und so wurde es wieder zu dem, was es immer war: Beute.");

            ContentAddition.AddBody(body);
            ContentAddition.AddSurvivorDef(survivor);
            return true;
        }

        private void SetupBody(GameObject body, MonsterDef def, string token)
        {
            CharacterBody cb = body.GetComponent<CharacterBody>();
            cb.baseNameToken = token + "_NAME";
            cb.subtitleNameToken = token + "_SUBTITLE";
            cb.bodyColor = def.Color;

            // Werte auf Survivor-Niveau, damit Monster nicht über- oder unterlegen sind.
            cb.baseMaxHealth = def.Health;
            cb.levelMaxHealth = def.Health * 0.3f;
            cb.baseDamage = def.Damage;
            cb.levelDamage = def.Damage * 0.2f;
            cb.baseArmor = def.Armor;
            cb.levelArmor = 0f;
            cb.baseRegen = def.Regen;
            cb.levelRegen = def.Regen * 0.2f;
            cb.baseMoveSpeed = def.MoveSpeed;
            cb.levelMoveSpeed = 0f;
            cb.baseAcceleration = 80f;
            cb.baseJumpPower = def.JumpPower;
            cb.baseJumpCount = 1;
            cb.sprintingSpeedMultiplier = 1.45f;
            cb.baseAttackSpeed = 1f;
            cb.baseCrit = 1f;
            cb.baseMaxShield = 0f;
            cb.hideCrosshair = false;

            CharacterBody commando = commandoBody ? commandoBody.GetComponent<CharacterBody>() : null;
            if (commando)
            {
                cb.preferredPodPrefab = commando.preferredPodPrefab;
                cb._defaultCrosshairPrefab = commando._defaultCrosshairPrefab;
            }
        }

        private void SetupPlayerComponents(GameObject body, MonsterDef def)
        {
            // Spieler können sonst keine Truhen öffnen, keine Equipment benutzen usw.
            if (!body.GetComponent<Interactor>())
                body.AddComponent<Interactor>().maxInteractionDistance = 3f * Mathf.Max(1f, def.CameraScale * 0.75f);
            if (!body.GetComponent<InteractionDriver>())
                body.AddComponent<InteractionDriver>();
            if (!body.GetComponent<EquipmentSlot>())
                body.AddComponent<EquipmentSlot>();

            // Kein Gold/XP für die Gegner, wenn ein Spieler-Monster stirbt.
            DeathRewards rewards = body.GetComponent<DeathRewards>();
            if (rewards)
                DestroyImmediate(rewards);

            CameraTargetParams camera = body.GetComponent<CameraTargetParams>();
            CameraTargetParams commandoCamera = commandoBody ? commandoBody.GetComponent<CameraTargetParams>() : null;
            if (camera && commandoCamera && commandoCamera.cameraParams)
            {
                CharacterCameraParams cameraParams = Instantiate(commandoCamera.cameraParams);
                cameraParams.name = body.name + "CameraParams";
                cameraParams.data.idealLocalCameraPos.value *= def.CameraScale;
                cameraParams.data.pivotVerticalOffset.value *= def.CameraScale;
                camera.cameraParams = cameraParams;
            }

            // Viele Skills laufen auf der "Weapon"-Statemachine. Fehlt sie, legen wir sie an.
            if (!body.GetComponents<EntityStateMachine>().Any(m => m.customName == "Weapon"))
            {
                var weapon = body.AddComponent<EntityStateMachine>();
                weapon.customName = "Weapon";
                weapon.initialStateType = new SerializableEntityStateType(typeof(Idle));
                weapon.mainStateType = new SerializableEntityStateType(typeof(Idle));

                NetworkStateMachine network = body.GetComponent<NetworkStateMachine>();
                if (network && StateMachinesField != null)
                {
                    var machines = (EntityStateMachine[])StateMachinesField.GetValue(network) ?? new EntityStateMachine[0];
                    ArrayUtils.ArrayAppend(ref machines, weapon);
                    StateMachinesField.SetValue(network, machines);
                }
            }
        }

        private void SetupSkills(GameObject body, MonsterDef def)
        {
            SkillLocator locator = body.GetComponent<SkillLocator>();
            locator.primary = PrepareSlot(body, locator.primary, "Primary", def.Primary);
            locator.secondary = PrepareSlot(body, locator.secondary, "Secondary", def.Secondary);
            locator.utility = PrepareSlot(body, locator.utility, "Utility", def.Utility);
            locator.special = PrepareSlot(body, locator.special, "Special", def.Special);
        }

        // Jeder Slot bekommt eine eigene SkillFamily, damit die Gegner-Version unverändert bleibt.
        // Vorhandene Monster-Skills bleiben die Standardauswahl, MoreSkills hängt später alle weiteren an.
        private GenericSkill PrepareSlot(GameObject body, GenericSkill skill, string slotName, string fallbackSkill)
        {
            SkillDef[] existing = skill && skill.skillFamily
                ? skill.skillFamily.variants.Select(v => v.skillDef).Where(d => d).ToArray()
                : new SkillDef[0];

            if (existing.Length == 0 && MoreSkillsPlugin.SkillDefs.TryGetValue(fallbackSkill, out SkillDef fallback))
                existing = new[] { fallback };
            if (existing.Length == 0)
                return skill;

            if (!skill)
            {
                skill = body.AddComponent<GenericSkill>();
                skill.skillName = slotName;
            }

            var family = ScriptableObject.CreateInstance<SkillFamily>();
            ((ScriptableObject)family).name = body.name + slotName + "Family";
            family.variants = existing.Select(d => new SkillFamily.Variant
            {
                skillDef = d,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(d.skillNameToken, false, null),
            }).ToArray();

            SkillFamilyField.SetValue(skill, family);
            ContentAddition.AddSkillFamily(family);
            return skill;
        }

        private GameObject CreateDisplay(GameObject body, MonsterDef def)
        {
            ModelLocator modelLocator = body.GetComponent<ModelLocator>();
            if (!modelLocator || !modelLocator.modelTransform)
                return null;

            GameObject display = modelLocator.modelTransform.gameObject.InstantiateClone("XKPlayable" + def.Name + "Display", false);
            // Große Monster im Auswahlbildschirm verkleinern, damit sie ins Bild passen.
            display.transform.localScale *= 1f / Mathf.Max(1f, def.CameraScale * 0.8f);
            return display;
        }
    }
}
