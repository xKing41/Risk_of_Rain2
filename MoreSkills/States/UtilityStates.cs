using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MoreSkills.States
{
    public class Dash : ModSkillState
    {
        protected override float BaseDuration => 0.25f;
        protected override bool ScalesWithAttackSpeed => false;
        protected override InterruptPriority MinimumPriority => InterruptPriority.PrioritySkill;

        private Vector3 direction;

        public override void OnEnter()
        {
            base.OnEnter();
            Vector3 forward = characterDirection ? characterDirection.forward : transform.forward;
            direction = inputBank && inputBank.moveVector != Vector3.zero ? inputBank.moveVector : forward;
            direction.y = 0f;
            direction.Normalize();

            SpawnEffect(SkillAssets.JumpEffect, characterBody.footPosition);
            if (NetworkServer.active)
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.3f);
        }

        public override void FixedUpdate()
        {
            if (isAuthority && characterMotor)
            {
                characterMotor.rootMotion += direction * (moveSpeedStat * 6f * Time.fixedDeltaTime);
                if (characterDirection)
                    characterDirection.forward = direction;
            }
            base.FixedUpdate();
        }
    }

    public class Blink : ModSkillState
    {
        protected override float BaseDuration => 0.1f;
        protected override bool ScalesWithAttackSpeed => false;
        protected override InterruptPriority MinimumPriority => InterruptPriority.PrioritySkill;

        public override void OnEnter()
        {
            base.OnEnter();
            SpawnEffect(SkillAssets.BlinkEffect, characterBody.corePosition);
            if (!isAuthority)
                return;

            Vector3 direction = GetAimRay().direction;
            float distance = 20f;
            if (Physics.SphereCast(characterBody.corePosition, 1f, direction, out RaycastHit hit, distance, LayerIndex.world.mask))
                distance = Mathf.Max(0f, hit.distance - 1f);

            Vector3 target = characterBody.footPosition + direction * distance;
            TeleportHelper.TeleportBody(characterBody, target);
            if (characterMotor)
                characterMotor.velocity = Vector3.zero;
        }

        public override void OnExit()
        {
            SpawnEffect(SkillAssets.BlinkEffect, characterBody.corePosition);
            base.OnExit();
        }
    }

    public class GroundSlam : ModSkillState
    {
        protected override float BaseDuration => 4f;
        protected override bool ScalesWithAttackSpeed => false;
        protected override InterruptPriority MinimumPriority => InterruptPriority.PrioritySkill;

        private bool slammed;

        public override void OnEnter()
        {
            base.OnEnter();
            SpawnEffect(SkillAssets.JumpEffect, characterBody.footPosition);
            if (isAuthority && characterMotor)
            {
                characterMotor.Motor.ForceUnground();
                Vector3 forward = GetAimRay().direction;
                forward.y = 0f;
                characterMotor.velocity = forward.normalized * moveSpeedStat * 2f + Vector3.up * 22f;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isAuthority || slammed || !characterMotor)
                return;

            // Nach dem Scheitelpunkt nach unten beschleunigen, bei Landung Explosion.
            if (fixedAge > 0.35f)
                characterMotor.velocity.y = Mathf.Min(characterMotor.velocity.y, -40f);

            if (fixedAge > 0.2f && characterMotor.isGrounded)
            {
                slammed = true;
                SpawnEffect(SkillAssets.GroundSlam, characterBody.footPosition, 10f);
                MakeBlast(characterBody.footPosition, 10f, 6f).Fire();
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (characterMotor)
                characterMotor.velocity = Vector3.zero;
            base.OnExit();
        }
    }

    public class Barrier : ModSkillState
    {
        protected override float BaseDuration => 0.2f;
        protected override bool ScalesWithAttackSpeed => false;

        public override void OnEnter()
        {
            base.OnEnter();
            SpawnEffect(SkillAssets.ExplosionQuick, characterBody.corePosition, 3f);
            if (NetworkServer.active && healthComponent)
                healthComponent.AddBarrier(healthComponent.fullCombinedHealth * 0.3f);
        }
    }
}
