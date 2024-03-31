using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using Main.Utility;

namespace Main.Model
{
    /// <summary>
    /// ダンス
    /// モデル
    /// </summary>
    public class DanceTurretModel : TurretModel
    {
        protected override OnmyoBulletConfig InitializeOnmyoBulletConfig()
        {
            // メイン
            var onmyoBulletConfig = new OnmyoBulletConfig()
            {
                actionRate = _shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.ActionRate),
                attackPoint = (int)_shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.AttackPoint),
                bulletLifeTime = _shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.BulletLifeTime),
                range = _shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.Range),
                // 陰陽玉と発射角度が異なるため再設定
                moveSpeed = 0f,
                trackingOfAny = RectTransform,
            };
            // ノックバック
            onmyoBulletConfig.knockBack = _shikigamiUtility.GetSubSkillValue(_shikigamiInfo, SubSkillType.KnockBack);
            // 麻痺
            onmyoBulletConfig.paralysis = _shikigamiUtility.GetSubSkillValue(_shikigamiInfo, SubSkillType.Paralysis);
            // 広範囲
            var largeRange = _shikigamiUtility.GetSubSkillValue(_shikigamiInfo, SubSkillType.LargeRange);
            onmyoBulletConfig.largeRange = largeRange != null ? largeRange.Value : 1f;
            // 伝播
            onmyoBulletConfig.propagation = ((IShikigamiParameterUtilityOfInteger)_shikigamiUtility).GetSubSkillValue(_shikigamiInfo, SubSkillType.Propagation);

            return onmyoBulletConfig;
        }

        protected override OnmyoBulletConfig ReLoadOnmyoBulletConfig(OnmyoBulletConfig config)
        {
            config.actionRate = _shikigamiUtility.GetMainSkillValueAddValueBuffMax(_shikigamiInfo, MainSkillType.ActionRate);
            config.attackPoint = (int)_shikigamiUtility.GetMainSkillValueAddValueBuffMax(_shikigamiInfo, MainSkillType.AttackPoint);

            return config;
        }

        protected override bool ActionOfBullet(ObjectsPoolModel objectsPoolModel, OnmyoBulletConfig onmyoBulletConfig)
        {
            return _turretUtility.CallInitialize(objectsPoolModel.GetDanceHallModel(), RectTransform, onmyoBulletConfig);
        }

        public override bool UpdateTempoLvValue(float tempoLevel, ShikigamiType shikigamiType)
        {
            try
            {
                switch (shikigamiType)
                {
                    case ShikigamiType.Dance:
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
    }
}
