using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MoreItems.Items
{
    public class ProspectorsPick : ItemBase
    {
        public override string Name => "ProspectorsPick";
        public override string DisplayName => "Goldgräberhacke";
        public override string Pickup => "Kills geben zusätzliches Gold.";
        public override string Description => "Jeder Kill gibt <style=cIsUtility>2 Gold</style> <style=cStack>(+2 pro Stack)</style>. Skaliert mit der Zeit.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] Tags => new[] { ItemTag.Utility, ItemTag.OnKillEffect };

        protected override void Hooks()
        {
            GlobalEventManager.onCharacterDeathGlobal += report =>
            {
                if (!NetworkServer.active || Run.instance == null || !report.attackerMaster)
                    return;

                int count = GetCount(report.attackerBody);
                if (count > 0)
                    report.attackerMaster.GiveMoney((uint)Mathf.CeilToInt(2f * count * Run.instance.difficultyCoefficient));
            };
        }
    }

    public class SoulCollector : ItemBase
    {
        public override string Name => "SoulCollector";
        public override string DisplayName => "Seelensammler";
        public override string Pickup => "Kills heilen dich.";
        public override string Description => "Jeder Kill <style=cIsHealing>heilt</style> dich um <style=cIsHealing>4%</style> <style=cStack>(+4% pro Stack)</style> deines maximalen Lebens.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] Tags => new[] { ItemTag.Healing, ItemTag.OnKillEffect };

        protected override void Hooks()
        {
            GlobalEventManager.onCharacterDeathGlobal += report =>
            {
                if (!NetworkServer.active)
                    return;

                CharacterBody attacker = report.attackerBody;
                int count = GetCount(attacker);
                if (count > 0 && attacker.healthComponent)
                    attacker.healthComponent.HealFraction(0.04f * count, default);
            };
        }
    }
}
