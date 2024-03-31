using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using Main.Utility;
using UniRx;
using Main.Test.Driver;

namespace Main.Model
{
    /// <summary>
    /// ラップ
    /// モデル
    /// </summary>
    public class WrapTurretModel : TurretModel, IWrapTurretModel, IWrapTurretModelTest
    {
        protected override OnmyoBulletConfig InitializeOnmyoBulletConfig()
        {
            // メイン
            var onmyoBulletConfig = new OnmyoBulletConfig()
            {
                actionRate = _shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.ActionRate),
                attackPoint = (int)_shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.AttackPoint),
                bulletLifeTime = _shikigamiUtility.GetMainSkillValue(_shikigamiInfo, MainSkillType.BulletLifeTime),
            };
            // ホーミング性能↑
            var homing = _shikigamiUtility.GetSubSkillValue(_shikigamiInfo, SubSkillType.Homing);
            if (homing != null)
                onmyoBulletConfig.range = homing.Value;
            // 広範囲（爆発）
            onmyoBulletConfig.onmyoBulletConfigOfExplosion = ((IShikigamiParameterUtilityOfExplosion)_shikigamiUtility).GetSubSkillValue(_shikigamiInfo, SubSkillType.Explosion);
            // 連射
            onmyoBulletConfig.continuousFire = ((IShikigamiParameterUtilityOfHighEnd)_shikigamiUtility).GetSubSkillValue(_shikigamiInfo, SubSkillType.ContinuousFire);
            // 貫通
            onmyoBulletConfig.penetrating = ((IShikigamiParameterUtilityOfBoolean)_shikigamiUtility).GetSubSkillValue(_shikigamiInfo, SubSkillType.Penetrating);
            // 拡散
            onmyoBulletConfig.spreading = ((IShikigamiParameterUtilityOfHighEnd)_shikigamiUtility).GetSubSkillValue(_shikigamiInfo, SubSkillType.Spreading);

            return onmyoBulletConfig;
        }

        protected override OnmyoBulletConfig ReLoadOnmyoBulletConfig(OnmyoBulletConfig config)
        {
            config.actionRate = _shikigamiUtility.GetMainSkillValueAddValueBuffMax(_shikigamiInfo, MainSkillType.ActionRate);
            config.attackPoint = (int)_shikigamiUtility.GetMainSkillValueAddValueBuffMax(_shikigamiInfo, MainSkillType.AttackPoint);

            return _turretUtility.UpdateMoveDirection(_bulletCompass, config);
        }

        protected override bool ActionOfBullet(ObjectsPoolModel objectsPoolModel, OnmyoBulletConfig onmyoBulletConfig)
        {
            // 連射
            if (onmyoBulletConfig.continuousFire.actionRate != null &&
                onmyoBulletConfig.continuousFire.instanceMax != null &&
                onmyoBulletConfig.continuousFire.spreadingAngle != null &&
                onmyoBulletConfig.spreading.actionRate == null &&
                onmyoBulletConfig.spreading.instanceMax == null &&
                onmyoBulletConfig.spreading.spreadingAngle == null)
            {
                Observable.FromCoroutine<bool>(observer => ActionHighEndOfBullet(observer, onmyoBulletConfig.continuousFire, objectsPoolModel, onmyoBulletConfig))
                    .Subscribe(_ => {})
                    .AddTo(gameObject);

                return true;
            }
            // 拡散
            else if (onmyoBulletConfig.spreading.actionRate != null &&
                onmyoBulletConfig.spreading.instanceMax != null &&
                onmyoBulletConfig.spreading.spreadingAngle != null)
            {
                Observable.FromCoroutine<bool>(observer => ActionHighEndOfBullet(observer, onmyoBulletConfig.spreading, objectsPoolModel, onmyoBulletConfig))
                    .Subscribe(_ => {})
                    .AddTo(gameObject);

                return true;
            }
            else
                return _turretUtility.CallInitialize(objectsPoolModel.GetWrapBulletModel(), RectTransform, onmyoBulletConfig);
        }

        /// <summary>
        /// 連射、拡散の弾の制御
        /// </summary>
        /// <param name="observer">バインド</param>
        /// <param name="onmyoBulletConfigOfHighEnd">ハイエンドバレットの設定</param>
        /// <param name="objectsPoolModel">オブジェクトプールモデル</param>
        /// <param name="onmyoBulletConfig">弾の設定</param>
        /// <returns>コルーチン</returns>
        private IEnumerator ActionHighEndOfBullet(System.IObserver<bool> observer, OnmyoBulletConfigOfHighEnd onmyoBulletConfigOfHighEnd, ObjectsPoolModel objectsPoolModel, OnmyoBulletConfig onmyoBulletConfig)
        {
            for (int i = 0; i < onmyoBulletConfigOfHighEnd.instanceMax; i++)
            {
                var angleOffset = onmyoBulletConfigOfHighEnd.spreadingAngle.Value / onmyoBulletConfigOfHighEnd.instanceMax.Value * i - onmyoBulletConfigOfHighEnd.spreadingAngle.Value / 2;
                var rotation = Quaternion.Euler(0, 0, angleOffset);
                var direction = rotation * onmyoBulletConfig.moveDirection.normalized;
                // var bullet = Instantiate(onmyoBulletPrefab, transform.position, Quaternion.identity);
                onmyoBulletConfig.moveDirection = direction;
                _turretUtility.CallInitialize(objectsPoolModel.GetWrapBulletModel(), RectTransform, onmyoBulletConfig);
                // bullet.Initialize(direction, onmyoBulletConfig);
                if (0f < onmyoBulletConfigOfHighEnd.actionRate.Value)
                    yield return new WaitForSeconds(onmyoBulletConfigOfHighEnd.actionRate.Value);
                else
                    yield return null;
            }
            observer.OnNext(true);
        }

        public override bool UpdateTempoLvValue(float tempoLevel, ShikigamiType shikigamiType)
        {
            try
            {
                switch (shikigamiType)
                {
                    case ShikigamiType.Wrap:
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

        OnmyoBulletConfig IWrapTurretModelTest.InitializeOnmyoBulletConfig()
        {
            return InitializeOnmyoBulletConfig();
        }

        bool IWrapTurretModelTest.ActionOfBullet(ObjectsPoolModel objectsPoolModel, OnmyoBulletConfig onmyoBulletConfig)
        {
            return ActionOfBullet(objectsPoolModel, onmyoBulletConfig);
        }
    }

    /// <summary>
    /// ラップ
    /// モデル
    /// インターフェース
    /// </summary>
    public interface IWrapTurretModel
    {
        /// <summary>
        /// 弾の角度を動的にセット初期化
        /// </summary>
        /// <param name="fromPosition">中央位置</param>
        /// <param name="danceVector">ダンスの向き</param>
        /// <returns>成功／失敗</returns>
        public bool InitializeBulletCompass(Vector2 fromPosition, Vector2 danceVector);
        /// <summary>
        /// 弾の角度タイプをセット
        /// </summary>
        /// <param name="bulletCompassType">弾の角度タイプ</param>
        /// <returns>成功／失敗</returns>
        public bool SetBulletCompassType(BulletCompassType bulletCompassType);
    }
}
