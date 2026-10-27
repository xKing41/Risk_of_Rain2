using System.Collections.Generic;
using System.Linq;
using EntityStates;
using ModCommon;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace MoreSkills.States
{
    // Basis für alle neuen Skills. Sie kommen ohne eigene Animationen aus,
    // damit sie auf jedem Survivor funktionieren, auch auf modded.
    public abstract class ModSkillState : BaseSkillState
    {
        protected virtual float BaseDuration => 0.5f;
        protected virtual bool ScalesWithAttackSpeed => true;
        protected virtual InterruptPriority MinimumPriority => InterruptPriority.Skill;

        protected float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = ScalesWithAttackSpeed ? BaseDuration / attackSpeedStat : BaseDuration;
            if (characterBody)
                characterBody.SetAimTimer(2f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && fixedAge >= duration)
                outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority() => MinimumPriority;

        // Nur der Besitzer spawnt Effekte; transmit verteilt sie an alle Spieler.
        protected void SpawnEffect(GameObject prefab, Vector3 position, float scale = 1f)
        {
            if (prefab && isAuthority)
                EffectManager.SpawnEffect(prefab, new EffectData { origin = position, scale = scale }, true);
        }

        protected void FireProjectile(GameObject prefab, float damageCoefficient, float force = 200f, float spreadDegrees = 0f)
        {
            if (!isAuthority || !prefab)
                return;

            Ray ray = GetAimRay();
            Vector3 direction = spreadDegrees > 0f
                ? Quaternion.Euler(Random.Range(-spreadDegrees, spreadDegrees), Random.Range(-spreadDegrees, spreadDegrees), 0f) * ray.direction
                : ray.direction;

            ProjectileManager.instance.FireProjectile(new FireProjectileInfo
            {
                projectilePrefab = prefab,
                position = ray.origin,
                rotation = Util.QuaternionSafeLookRotation(direction),
                owner = gameObject,
                damage = damageStat * damageCoefficient,
                force = force,
                crit = RollCrit(),
            });
        }

        protected BulletAttack MakeBullet(float damageCoefficient, GameObject tracer)
        {
            Ray ray = GetAimRay();
            return new BulletAttack
            {
                owner = gameObject,
                weapon = gameObject,
                origin = ray.origin,
                aimVector = ray.direction,
                minSpread = 0f,
                maxSpread = 0f,
                bulletCount = 1,
                damage = damageStat * damageCoefficient,
                force = 300f,
                tracerEffectPrefab = tracer,
                isCrit = RollCrit(),
                radius = 0.4f,
                smartCollision = true,
                maxDistance = 250f,
                procCoefficient = 1f,
                falloffModel = BulletAttack.FalloffModel.None,
            };
        }

        protected BlastAttack MakeBlast(Vector3 position, float radius, float damageCoefficient)
        {
            return new BlastAttack
            {
                attacker = gameObject,
                inflictor = gameObject,
                teamIndex = GetTeam(),
                position = position,
                radius = radius,
                baseDamage = damageStat * damageCoefficient,
                baseForce = 1500f,
                bonusForce = Vector3.up * 800f,
                crit = RollCrit(),
                procCoefficient = 1f,
                falloffModel = BlastAttack.FalloffModel.None,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
            };
        }

        // Gegner im Sichtkegel bzw. Umkreis, nächster zuerst.
        protected List<HurtBox> FindEnemies(Vector3 origin, Vector3 direction, float maxDistance, float maxAngle)
        {
            var search = new BullseyeSearch
            {
                searchOrigin = origin,
                searchDirection = direction,
                maxDistanceFilter = maxDistance,
                maxAngleFilter = maxAngle,
                teamMaskFilter = TeamMask.GetEnemyTeams(GetTeam()),
                sortMode = BullseyeSearch.SortMode.Distance,
                filterByLoS = true,
            };
            search.RefreshCandidates();
            search.FilterOutGameObject(gameObject);
            return search.GetResults().Where(h => h && h.healthComponent && h.healthComponent.alive).ToList();
        }

        // Zielpunkt, auf den der Spieler zielt (Boden/Wand), sonst maxDistance voraus.
        protected Vector3 AimPoint(float maxDistance)
        {
            Ray ray = GetAimRay();
            return Physics.Raycast(ray, out RaycastHit hit, maxDistance, LayerIndex.world.mask)
                ? hit.point
                : ray.GetPoint(maxDistance);
        }
    }
}
