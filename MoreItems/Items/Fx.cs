using System.Collections.Generic;
using System.Linq;
using ModCommon;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace MoreItems.Items
{
    // Bausteine, aus denen die Item-Effekte zusammengesetzt sind. Alles hier läuft auf dem Server.
    public static class Fx
    {
        private static GameObject explosion, explosionQuick, fireExplosion, lightningImpact;

        public static void Load()
        {
            explosion = ModAssets.Load<GameObject>(AssetKeys.OmniExplosionVFX);
            explosionQuick = ModAssets.Load<GameObject>(AssetKeys.OmniExplosionVFXQuick);
            fireExplosion = ModAssets.Load<GameObject>(AssetKeys.IgniteExplosionVFX);
            lightningImpact = ModAssets.Load<GameObject>(AssetKeys.LightningStrikeImpact);
        }

        public static float Hyperbolic(float perStack, int count) => 1f - 1f / (1f + perStack * count);

        public static void Damage(CharacterBody attacker, CharacterBody victim, float damage, bool crit = false,
            DamageType type = DamageType.Generic)
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
                damageType = type,
            });
        }

        public static void Blast(CharacterBody attacker, Vector3 position, float radius, float damage, bool crit = false,
            bool fire = false, DamageType type = DamageType.Generic)
        {
            if (!attacker)
                return;

            GameObject effect = fire ? fireExplosion : radius > 8f ? explosion : explosionQuick;
            if (effect)
                EffectManager.SpawnEffect(effect, new EffectData { origin = position, scale = radius }, true);

            new BlastAttack
            {
                attacker = attacker.gameObject,
                inflictor = attacker.gameObject,
                teamIndex = attacker.teamComponent ? attacker.teamComponent.teamIndex : TeamIndex.Player,
                position = position,
                radius = radius,
                baseDamage = damage,
                baseForce = 600f,
                crit = crit,
                procCoefficient = 0f,
                damageColorIndex = DamageColorIndex.Item,
                falloffModel = BlastAttack.FalloffModel.None,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                damageType = type,
            }.Fire();
        }

        public static void Lightning(CharacterBody attacker, Vector3 origin, HurtBox target, float damage, int bounces = 0, bool crit = false)
        {
            if (!attacker || !target)
                return;

            OrbManager.instance.AddOrb(new LightningOrb
            {
                origin = origin,
                target = target,
                attacker = attacker.gameObject,
                damageValue = damage,
                isCrit = crit,
                teamIndex = attacker.teamComponent ? attacker.teamComponent.teamIndex : TeamIndex.Player,
                bouncesRemaining = bounces,
                range = 25f,
                procCoefficient = 0f,
                lightningType = LightningOrb.LightningType.Ukulele,
                damageColorIndex = DamageColorIndex.Item,
                bouncedObjects = new List<HealthComponent>(),
            });
        }

        public static void Missile(CharacterBody attacker, HurtBox target, float damage, bool crit = false)
        {
            if (!attacker || !target)
                return;

            OrbManager.instance.AddOrb(new MicroMissileOrb
            {
                origin = attacker.aimOrigin,
                target = target,
                attacker = attacker.gameObject,
                damageValue = damage,
                isCrit = crit,
                teamIndex = attacker.teamComponent ? attacker.teamComponent.teamIndex : TeamIndex.Player,
                procCoefficient = 0f,
                damageColorIndex = DamageColorIndex.Item,
            });
        }

        public static void LightningStrike(Vector3 position)
        {
            if (lightningImpact)
                EffectManager.SpawnEffect(lightningImpact, new EffectData { origin = position, scale = 2f }, true);
        }

        public static void Dot(CharacterBody attacker, CharacterBody victim, DotController.DotIndex dot, float duration, float multiplier = 1f)
        {
            if (!attacker || !victim)
                return;

            var info = new InflictDotInfo
            {
                attackerObject = attacker.gameObject,
                victimObject = victim.gameObject,
                dotIndex = dot,
                duration = duration,
                damageMultiplier = multiplier,
            };
            DotController.InflictDot(ref info);
        }

        public static HurtBox HurtBoxOf(CharacterBody body) => body && body.mainHurtBox ? body.mainHurtBox : null;

        public static List<HurtBox> Enemies(CharacterBody around, Vector3 position, float radius, int max = 99)
        {
            if (!around)
                return new List<HurtBox>();

            var search = new BullseyeSearch
            {
                searchOrigin = position,
                maxDistanceFilter = radius,
                teamMaskFilter = TeamMask.GetEnemyTeams(around.teamComponent ? around.teamComponent.teamIndex : TeamIndex.Player),
                sortMode = BullseyeSearch.SortMode.Distance,
                filterByLoS = false,
            };
            search.RefreshCandidates();
            search.FilterOutGameObject(around.gameObject);
            return search.GetResults()
                .Where(h => h && h.healthComponent && h.healthComponent.alive)
                .GroupBy(h => h.healthComponent)
                .Select(g => g.First())
                .Take(max)
                .ToList();
        }

        public static IEnumerable<CharacterBody> Allies(CharacterBody around, float radius)
        {
            if (!around || !around.teamComponent)
                yield break;

            foreach (TeamComponent member in TeamComponent.GetTeamMembers(around.teamComponent.teamIndex))
            {
                if (member && member.body && member.body.healthComponent && member.body.healthComponent.alive
                    && (member.body.corePosition - around.corePosition).sqrMagnitude <= radius * radius)
                    yield return member.body;
            }
        }

        public static void Heal(CharacterBody body, float amount)
        {
            if (body && body.healthComponent && amount > 0f)
                body.healthComponent.Heal(amount, default);
        }

        public static void HealFraction(CharacterBody body, float fraction)
        {
            if (body && body.healthComponent && fraction > 0f)
                body.healthComponent.HealFraction(fraction, default);
        }

        public static void Barrier(CharacterBody body, float amount)
        {
            if (body && body.healthComponent && amount > 0f)
                body.healthComponent.AddBarrier(amount);
        }

        // Gold skaliert wie vanilla mit der Zeit/Schwierigkeit des Runs.
        public static void Gold(CharacterBody body, float baseAmount)
        {
            if (body && body.master && Run.instance)
                body.master.GiveMoney((uint)Mathf.CeilToInt(baseAmount * Run.instance.difficultyCoefficient));
        }

        public static void Buff(CharacterBody body, BuffDef buff, float seconds)
        {
            if (body && buff)
                body.AddTimedBuff(buff, seconds);
        }
    }
}
