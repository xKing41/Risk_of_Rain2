using ModCommon;
using RoR2;
using UnityEngine;

namespace MoreSkills
{
    internal static class SkillAssets
    {
        public static GameObject Firebolt, Lightningbolt, Grenade, BigFireball;
        public static GameObject TracerDefault, TracerSnipe, TracerGolem;
        public static GameObject Explosion, ExplosionQuick, BlinkEffect, JumpEffect, LightningImpact, GroundSlam, MeteorImpact, FireExplosion;
        public static BuffDef BerserkBuff;

        public static void Load()
        {
            Firebolt = ModAssets.Load<GameObject>(AssetKeys.MageFireboltBasic);
            Lightningbolt = ModAssets.Load<GameObject>(AssetKeys.MageLightningboltBasic);
            Grenade = ModAssets.Load<GameObject>(AssetKeys.CommandoGrenadeProjectile);
            BigFireball = ModAssets.Load<GameObject>(AssetKeys.LemurianBigFireball);

            TracerDefault = ModAssets.Load<GameObject>(AssetKeys.TracerCommandoDefault);
            TracerSnipe = ModAssets.Load<GameObject>(AssetKeys.TracerHuntressSnipe);
            TracerGolem = ModAssets.Load<GameObject>(AssetKeys.TracerGolem);

            Explosion = ModAssets.Load<GameObject>(AssetKeys.OmniExplosionVFX);
            ExplosionQuick = ModAssets.Load<GameObject>(AssetKeys.OmniExplosionVFXQuick);
            BlinkEffect = ModAssets.Load<GameObject>(AssetKeys.HuntressBlinkEffect);
            JumpEffect = ModAssets.Load<GameObject>(AssetKeys.BoostJumpEffect);
            LightningImpact = ModAssets.Load<GameObject>(AssetKeys.LightningStrikeImpact);
            GroundSlam = ModAssets.Load<GameObject>(AssetKeys.LoaderGroundSlam);
            MeteorImpact = ModAssets.Load<GameObject>(AssetKeys.MeteorStrikeImpact);
            FireExplosion = ModAssets.Load<GameObject>(AssetKeys.IgniteExplosionVFX);
        }
    }
}
