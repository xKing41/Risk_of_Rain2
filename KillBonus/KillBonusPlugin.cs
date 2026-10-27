using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace KillBonus
{
    // Gibt Spielern beim Töten von Gegnern eine Chance auf Bonus-Gold.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class KillBonusPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "xking41.KillBonus";
        public const string PluginName = "KillBonus";
        public const string PluginVersion = "0.1.0";

        private ConfigEntry<float> bonusChance;
        private ConfigEntry<int> baseGold;

        private void Awake()
        {
            bonusChance = Config.Bind("General", "BonusChance", 10f,
                "Chance in Prozent (0-100), dass ein Kill Bonus-Gold gibt. Wird von Glück (Clover) beeinflusst.");
            baseGold = Config.Bind("General", "BaseGold", 5,
                "Basis-Goldmenge pro Bonus. Skaliert mit der Schwierigkeit des Runs.");

            Logger.LogInfo($"{PluginName} {PluginVersion} geladen.");
        }

        private void OnEnable() => GlobalEventManager.onCharacterDeathGlobal += OnCharacterDeath;

        private void OnDisable() => GlobalEventManager.onCharacterDeathGlobal -= OnCharacterDeath;

        private void OnCharacterDeath(DamageReport report)
        {
            // Gold wird nur vom Host vergeben, sonst bekommen Clients es doppelt.
            if (!NetworkServer.active || Run.instance == null)
                return;

            CharacterMaster attacker = report.attackerMaster;
            if (attacker == null || attacker.playerCharacterMasterController == null)
                return;

            if (!Util.CheckRoll(bonusChance.Value, attacker))
                return;

            uint gold = (uint)Mathf.CeilToInt(baseGold.Value * Run.instance.difficultyCoefficient);
            attacker.GiveMoney(gold);
        }
    }
}
