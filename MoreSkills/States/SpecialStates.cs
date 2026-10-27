using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace MoreSkills.States
{
    public class Thunderstorm : ModSkillState
    {
        protected override float BaseDuration => 0.6f;

        public override void OnEnter()
        {
            base.OnEnter();
            SpawnEffect(SkillAssets.LightningImpact, characterBody.corePosition, 5f);
            if (!NetworkServer.active)
                return;

            var targets = FindEnemies(characterBody.corePosition, Vector3.up, 40f, 180f);
            bool crit = RollCrit();
            for (int i = 0; i < Mathf.Min(10, targets.Count); i++)
            {
                OrbManager.instance.AddOrb(new LightningOrb
                {
                    origin = characterBody.corePosition + Vector3.up * 8f,
                    target = targets[i],
                    attacker = gameObject,
                    damageValue = damageStat * 6f,
                    isCrit = crit,
                    teamIndex = GetTeam(),
                    bouncesRemaining = 0,
                    procCoefficient = 1f,
                    lightningType = LightningOrb.LightningType.Ukulele,
                    damageColorIndex = DamageColorIndex.Default,
                });
            }
        }
    }

    public class HealingNova : ModSkillState
    {
        protected override float BaseDuration => 0.4f;
        protected override bool ScalesWithAttackSpeed => false;

        public override void OnEnter()
        {
            base.OnEnter();
            SpawnEffect(SkillAssets.ExplosionQuick, characterBody.corePosition, 25f);
            if (!NetworkServer.active)
                return;

            foreach (TeamComponent member in TeamComponent.GetTeamMembers(GetTeam()))
            {
                if (member && member.body && member.body.healthComponent
                    && (member.body.corePosition - characterBody.corePosition).sqrMagnitude <= 25f * 25f)
                {
                    member.body.healthComponent.HealFraction(0.35f, default);
                }
            }
        }
    }

    public class Berserk : ModSkillState
    {
        protected override float BaseDuration => 0.2f;
        protected override bool ScalesWithAttackSpeed => false;

        public override void OnEnter()
        {
            base.OnEnter();
            SpawnEffect(SkillAssets.FireExplosion, characterBody.corePosition, 4f);
            if (NetworkServer.active && SkillAssets.BerserkBuff)
                characterBody.AddTimedBuff(SkillAssets.BerserkBuff, 8f);
        }
    }

    public class Meteor : ModSkillState
    {
        protected override float BaseDuration => 0.5f;

        public override void OnEnter()
        {
            base.OnEnter();
            Vector3 point = AimPoint(100f);
            SpawnEffect(SkillAssets.MeteorImpact, point, 10f);
            if (isAuthority)
                MakeBlast(point, 10f, 12f).Fire();
        }
    }
}
