using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;

namespace Main.Model
{
    /// <summary>
    /// 陰陽玉（陰陽砲台）
    /// モデル
    /// </summary>
    public class OnmyoTurretModel : TurretModel, IOnmyoTurretModel, IWrapTurretModel
    {
        protected override OnmyoBulletConfig InitializeOnmyoBulletConfig()
        {
            return new OnmyoBulletConfig()
            {
                actionRate = _shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.ActionRate),
                attackPoint = (int)_shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.AttackPoint),
                bulletLifeTime = _shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.BulletLifeTime),
            };
        }

        protected override OnmyoBulletConfig ReLoadOnmyoBulletConfig(OnmyoBulletConfig config)
        {
            return _turretUtility.UpdateMoveDirection(_bulletCompass, config);
        }

        protected override bool ActionOfBullet(ObjectsPoolModel objectsPoolModel, OnmyoBulletConfig onmyoBulletConfig)
        {
            return _turretUtility.CallInitialize(objectsPoolModel.GetOnmyoBulletModel(), RectTransform, onmyoBulletConfig);
        }

        public override bool UpdateTempoLvValue(float tempoLevel, ShikigamiType shikigamiType)
        {
            // 陰陽玉はレベルアップなし
            return true;
        }

        public bool ActionOfBulletOfOnmyoTurretModel()
        {
            try
            {
                var config = InitializeOnmyoBulletConfig();
                // 一定間隔で弾を生成するための実装
                config = ReLoadOnmyoBulletConfig(config);
                if (!_spawnUtility.ManageBulletSpawn(
                    _poolModel,
                    config,
                    ActionOfBullet))
                    Debug.LogError("ManageBulletSpawn");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool SetActionRateNormal(bool isUnLoopNormalActionRate)
        {
            try
            {
                _isUnLoopNormalActionRate = isUnLoopNormalActionRate;

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

    /// <summary>
    /// 陰陽玉（陰陽砲台）
    /// モデル
    /// インターフェース
    /// </summary>
    public interface IOnmyoTurretModel
    {
        /// <summary>
        /// 魔力弾／円舞範囲／デバフ魔力弾の制御
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool ActionOfBulletOfOnmyoTurretModel();
        /// <summary>
        /// 通常攻撃の攻撃間隔を制御
        /// </summary>
        /// <param name="isUnLoopNormalActionRate">通常攻撃のループが有効か</param>
        /// <returns>成功／失敗</returns>
        public bool SetActionRateNormal(bool isUnLoopNormalActionRate);
    }
}
