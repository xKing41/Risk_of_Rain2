using EntityStates;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace MoreSkills.States
{
    public class ArcaneBolt : ModSkillState
    {
        protected override float BaseDuration => 0.45f;
        protected override InterruptPriority MinimumPriority => InterruptPriority.Any;

        public override void OnEnter()
        {
            base.OnEnter();
            FireProjectile(SkillAssets.Firebolt, 2.4f);
        }
    }

    public class ChainLightning : ModSkillState
    {
        protected override float BaseDuration => 0.4f;
        protected override InterruptPriority MinimumPriority => InterruptPriority.Any;

        public override void OnEnter()
        {
            base.OnEnter();
            if (!NetworkServer.active)
                return;

            Ray ray = GetAimRay();
            var targets = FindEnemies(ray.origin, ray.direction, 60f, 15f);
            if (targets.Count == 0)
                return;

            OrbManager.instance.AddOrb(new LightningOrb
            {
                origin = ray.origin,
                target = targets[0],
                attacker = gameObject,
                damageValue = damageStat * 1.6f,
                isCrit = RollCrit(),
                teamIndex = GetTeam(),
                bouncesRemaining = 3,
                range = 25f,
                procCoefficient = 0.8f,
                lightningType = LightningOrb.LightningType.Ukulele,
                damageColorIndex = DamageColorIndex.Default,
                bouncedObjects = new System.Collections.Generic.List<HealthComponent>(),
            });
        }
    }

    public class PrecisionShot : ModSkillState
    {
        protected override float BaseDuration => 0.7f;
        protected override InterruptPriority MinimumPriority => InterruptPriority.Any;

        public override void OnEnter()
        {
            base.OnEnter();
            if (!isAuthority)
                return;

            BulletAttack bullet = MakeBullet(3.2f, SkillAssets.TracerSnipe);
            bullet.radius = 0.2f;
            bullet.Fire();
        }
    }

    public class Scattershot : ModSkillState
    {
        protected override float BaseDuration => 0.8f;
        protected override InterruptPriority MinimumPriority => InterruptPriority.Any;

        public override void OnEnter()
        {
            base.OnEnter();
            if (!isAuthority)
                return;

            BulletAttack bullet = MakeBullet(0.9f, SkillAssets.TracerDefault);
            bullet.bulletCount = 6;
            bullet.maxSpread = 5f;
            bullet.procCoefficient = 0.6f;
            bullet.maxDistance = 60f;
            bullet.Fire();
        }
    }
}
