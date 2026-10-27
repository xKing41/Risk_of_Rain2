using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace MoreSkills.States
{
    public class FragGrenade : ModSkillState
    {
        protected override float BaseDuration => 0.4f;

        public override void OnEnter()
        {
            base.OnEnter();
            FireProjectile(SkillAssets.Grenade, 6f, 800f);
        }
    }

    public class MissileVolley : ModSkillState
    {
        protected override float BaseDuration => 0.5f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (!NetworkServer.active)
                return;

            Ray ray = GetAimRay();
            var targets = FindEnemies(ray.origin, ray.direction, 80f, 40f);
            if (targets.Count == 0)
                return;

            bool crit = RollCrit();
            for (int i = 0; i < 4; i++)
            {
                OrbManager.instance.AddOrb(new MicroMissileOrb
                {
                    origin = ray.origin,
                    target = targets[i % targets.Count],
                    attacker = gameObject,
                    damageValue = damageStat * 2.5f,
                    isCrit = crit,
                    teamIndex = GetTeam(),
                    procCoefficient = 0.5f,
                    damageColorIndex = DamageColorIndex.Default,
                });
            }
        }
    }

    public class Shockwave : ModSkillState
    {
        protected override float BaseDuration => 0.4f;

        public override void OnEnter()
        {
            base.OnEnter();
            SpawnEffect(SkillAssets.GroundSlam, characterBody.footPosition, 12f);
            if (isAuthority)
                MakeBlast(characterBody.corePosition, 12f, 5f).Fire();
        }
    }

    public class FrostLance : ModSkillState
    {
        protected override float BaseDuration => 0.5f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (!isAuthority)
                return;

            BulletAttack bullet = MakeBullet(4f, SkillAssets.TracerGolem);
            bullet.damageType = DamageType.Freeze2s;
            bullet.radius = 1f;
            bullet.stopperMask = LayerIndex.world.mask;
            bullet.Fire();
        }
    }
}
