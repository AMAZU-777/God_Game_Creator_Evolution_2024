using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Main.Common;
using UniRx;
using UnityEngine;
using Universal.Utility;

namespace Main.Model
{
    /// <summary>
    /// 魔力弾
    /// モデル
    /// </summary>
    public class OnmyoBulletModel : BulletModel, IBulletModel
    {
        /// <summary>魔力弾ステート</summary>
        private OnmyoBulletModelState _onmyoBulletModelState = new OnmyoBulletModelState()
        {
            IsExplosion = new BoolReactiveProperty(),
        };
        /// <summary>魔力弾ステート</summary>
        public OnmyoBulletModelState OnmyoBulletModelState => _onmyoBulletModelState;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_onmyoBulletModelState.IsExplosion.Value)
                _onmyoBulletModelState.IsExplosion.Value = false;
        }

        public bool Initialize(Vector2 position, Vector3 eulerAngles, OnmyoBulletConfig updateConf)
        {
            try
            {
                // 陰陽玉／ラップ
                //  ●威力、レート、持続
                _moveDirection = Quaternion.Euler(eulerAngles) * (!updateConf.moveDirection.Equals(Vector2.zero) ?
                    updateConf.moveDirection : onmyoBulletConfig.moveDirection);
                _moveSpeed = updateConf.moveSpeed != null ? updateConf.moveSpeed.Value : onmyoBulletConfig.moveSpeed.Value;
                _disableTimeSec = updateConf.bulletLifeTime;
                Transform.position = position;
                if (!attackColliderOfOnmyoBullet.SetAttackPoint(updateConf.attackPoint))
                    throw new System.Exception("SetAttackPoint");
                // 広範囲（爆発）
                if (updateConf.onmyoBulletConfigOfExplosion.explosionDuration != null &&
                    updateConf.onmyoBulletConfigOfExplosion.explosionRange != null)
                {
                    DOVirtual.DelayedCall(updateConf.onmyoBulletConfigOfExplosion.explosionDuration.Value, () =>
                    {
                        _moveSpeed = 0f;
                        if (!attackColliderOfOnmyoBullet.SetRadiosOfCircleCollier2D(updateConf.onmyoBulletConfigOfExplosion.explosionRange.Value))
                            throw new System.Exception("SetRadiosOfCircleCollier2D");
                        _onmyoBulletModelState.IsExplosion.Value = true;
                    });
                }
                // 貫通
                if (!SetPenetrating(updateConf.penetrating))
                    throw new System.Exception("SetPenetrating");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }

    /// <summary>
    /// 魔力弾
    /// ステート
    /// </summary>
    public struct OnmyoBulletModelState
    {
        /// <summary>爆発したか</summary>
        public IReactiveProperty<bool> IsExplosion;
    }
}
