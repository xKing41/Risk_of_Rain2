using RoR2;
using UnityEngine;

namespace MoreItems.Items
{
    // Hinweis: Zusatzschaden von unseren Items hat procCoefficient 0. Deshalb lösen
    // sie sich nicht gegenseitig aus, und es entstehen keine Endlosschleifen.
    public static class DamageHelper
    {
        public static void DealExtraDamage(CharacterBody attacker, CharacterBody victim, float damage, bool crit)
        {
            if (!victim || !victim.healthComponent || !victim.healthComponent.alive || damage <= 0f)
                return;

            victim.healthComponent.TakeDamage(new DamageInfo
            {
                attacker = attacker ? attacker.gameObject : null,
                inflictor = attacker ? attacker.gameObject : null,
                damage = damage,
                crit = crit,
                position = victim.corePosition,
                procCoefficient = 0f,
                damageColorIndex = DamageColorIndex.Item,
            });
        }
    }

    public class VampireFang : ItemBase
    {
        public override string Name => "VampireFang";
        public override string DisplayName => "Vampirzahn";
        public override string Pickup => "Heile dich um einen Teil des verursachten Schadens.";
        public override string Description => "Treffer <style=cIsHealing>heilen</style> dich um <style=cIsHealing>1.5%</style> <style=cStack>(+1.5% pro Stack)</style> des verursachten Schadens.";
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] Tags => new[] { ItemTag.Healing };

        protected override void Hooks()
        {
            GlobalEventManager.onServerDamageDealt += report =>
            {
                CharacterBody attacker = report.attackerBody;
                int count = GetCount(attacker);
                if (count <= 0 || report.damageInfo.procCoefficient <= 0f || !attacker.healthComponent)
                    return;

                float heal = report.damageDealt * 0.015f * count * report.damageInfo.procCoefficient;
                attacker.healthComponent.Heal(heal, report.damageInfo.procChainMask);
            };
        }
    }

    public class ThornVest : ItemBase
    {
        public override string Name => "ThornVest";
        public override string DisplayName => "Dornenweste";
        public override string Pickup => "Wirf einen Teil des erlittenen Schadens zurück.";
        public override string Description => "Wenn du getroffen wirst, erleidet der Angreifer <style=cIsDamage>15%</style> <style=cStack>(+15% pro Stack)</style> des Schadens.";
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] Tags => new[] { ItemTag.Damage };

        protected override void Hooks()
        {
            GlobalEventManager.onServerDamageDealt += report =>
            {
                CharacterBody victim = report.victimBody;
                CharacterBody attacker = report.attackerBody;
                int count = GetCount(victim);
                if (count <= 0 || !attacker || attacker == victim || report.damageInfo.procCoefficient <= 0f)
                    return;

                DamageHelper.DealExtraDamage(victim, attacker, report.damageDealt * 0.15f * count, false);
            };
        }
    }

    public class EchoBlade : ItemBase
    {
        public override string Name => "EchoBlade";
        public override string DisplayName => "Echoklinge";
        public override string Pickup => "Chance, Treffer zu wiederholen.";
        public override string Description => "<style=cIsDamage>10%</style> <style=cStack>(+10% pro Stack)</style> Chance, dass ein Treffer seinen Schaden <style=cIsDamage>noch einmal</style> verursacht.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] Tags => new[] { ItemTag.Damage };

        protected override void Hooks()
        {
            GlobalEventManager.onServerDamageDealt += report =>
            {
                CharacterBody attacker = report.attackerBody;
                int count = GetCount(attacker);
                if (count <= 0 || report.damageInfo.procCoefficient <= 0f)
                    return;

                float chance = Mathf.Min(100f, 10f * count) * report.damageInfo.procCoefficient;
                if (Util.CheckRoll(chance, attacker.master))
                    DamageHelper.DealExtraDamage(attacker, report.victimBody, report.damageDealt, report.damageInfo.crit);
            };
        }
    }

    public class ExecutionersAxe : ItemBase
    {
        public override string Name => "ExecutionersAxe";
        public override string DisplayName => "Henkersbeil";
        public override string Pickup => "Richte geschwächte Gegner sofort hin.";
        public override string Description => "Treffer <style=cIsDamage>töten</style> Gegner (keine Bosse) mit weniger als <style=cIsDamage>15%</style> <style=cStack>(+15% pro Stack, hyperbolisch)</style> Leben sofort.";
        public override ItemTier Tier => ItemTier.Tier3;
        public override ItemTag[] Tags => new[] { ItemTag.Damage };

        protected override void Hooks()
        {
            GlobalEventManager.onServerDamageDealt += report =>
            {
                CharacterBody attacker = report.attackerBody;
                CharacterBody victim = report.victimBody;
                int count = GetCount(attacker);
                if (count <= 0 || !victim || victim.isBoss || report.damageInfo.procCoefficient <= 0f)
                    return;

                HealthComponent health = victim.healthComponent;
                if (health && health.alive && health.combinedHealthFraction < Hyperbolic(0.1765f, count))
                    DamageHelper.DealExtraDamage(attacker, victim, health.fullCombinedHealth, false);
            };
        }
    }
}
