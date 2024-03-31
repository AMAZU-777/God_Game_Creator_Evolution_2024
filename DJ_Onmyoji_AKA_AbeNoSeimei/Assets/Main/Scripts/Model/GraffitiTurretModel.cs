using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using Main.Utility;

namespace Main.Model
{
    /// <summary>
    /// グラフィティ
    /// モデル
    /// </summary>
    public class GraffitiTurretModel : TurretModel, IWrapTurretModel
    {
        protected override OnmyoBulletConfig InitializeOnmyoBulletConfig()
        {
            // メイン
            var onmyoBulletConfig = new OnmyoBulletConfig()
            {
                actionRate = _shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.ActionRate),
                bulletLifeTime = _shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.BulletLifeTime),
                // 陰陽玉と発射角度が異なるため再設定
                range = _shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.Range),
                debuffEffectLifeTime = _shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.DebuffEffectLifeTime),
            };
            // 被ダメージ増加
            onmyoBulletConfig.increasedDamage = _shikigamiUtility.GetSubSkillValue(_shikigamiInfo, SubSkillType.IncreasedDamage);
            // 継続ダメージ
            onmyoBulletConfig.poison = _shikigamiUtility.GetSubSkillValue(_shikigamiInfo, SubSkillType.Poison);
            // 強化解除
            onmyoBulletConfig.cancelBuff = ((IShikigamiParameterUtilityOfBoolean)_shikigamiUtility).GetSubSkillValue(_shikigamiInfo, SubSkillType.CancelBuff);
            // ドレイン（回復）
            onmyoBulletConfig.drain = _shikigamiUtility.GetSubSkillValue(_shikigamiInfo, SubSkillType.Drain);

            return onmyoBulletConfig;
        }

        protected override OnmyoBulletConfig ReLoadOnmyoBulletConfig(OnmyoBulletConfig config)
        {
            config.actionRate = _shikigamiUtility.GetMainSkillValueAddValueBuffMax(_shikigamiInfo, MainSkillType.ActionRate);

            return _turretUtility.UpdateMoveDirection(_bulletCompass, config);
        }

        protected override bool ActionOfBullet(ObjectsPoolModel objectsPoolModel, OnmyoBulletConfig onmyoBulletConfig)
        {
            return _turretUtility.CallInitialize(objectsPoolModel.GetGraffitiBulletModel(), RectTransform, onmyoBulletConfig);
        }

        public override bool UpdateTempoLvValue(float tempoLevel, ShikigamiType shikigamiType)
        {
            try
            {
                switch (shikigamiType)
                {
                    case ShikigamiType.Graffiti:
                        if (_shikigamiInfo.state.tempoLevel != null)
                            _shikigamiInfo.state.tempoLevel.Value = tempoLevel;

                        break;
                    default:
                        break;
                }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool InitializeBulletCompass(Vector2 fromPosition, Vector2 danceVector)
        {
            return _turretUtility.InitializeBulletCompass(ref _bulletCompass,
                (new Vector2(RectTransform.position.x, RectTransform.position.y) - fromPosition).normalized,
                danceVector);
        }

        public bool SetBulletCompassType(BulletCompassType bulletCompassType)
        {
            return _turretUtility.SetBulletCompassType(ref _bulletCompass, bulletCompassType);
        }
    }
}
