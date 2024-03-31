using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using UniRx;

namespace Main.Model
{
    /// <summary>
    /// 魔力弾（グラフィティ用）
    /// モデル
    /// </summary>
    public class GraffitiBulletModel : BulletModel, IBulletModel
    {
        /// <summary>最小範囲</summary>
        private const float RANGE_MIN = 0f;
        /// <summary>最大範囲</summary>
        private float _rangeMax;
        /// <summary>経過時間</summary>
        IReactiveProperty<float> elapsedTime = new FloatReactiveProperty();

        public bool Initialize(Vector2 position, Vector3 eulerAngles, OnmyoBulletConfig updateConf)
        {
            try
            {
                // グラフティ
                //  ●持続、効果時間、レート、範囲
                _moveDirection = Quaternion.Euler(eulerAngles) * (!updateConf.moveDirection.Equals(Vector2.zero) ?
                    updateConf.moveDirection : onmyoBulletConfig.moveDirection);
                _moveSpeed = updateConf.moveSpeed != null ? updateConf.moveSpeed.Value : onmyoBulletConfig.moveSpeed.Value;
                _disableTimeSec = updateConf.bulletLifeTime;
                Transform.position = position;
                if (0f < updateConf.range)
                    _rangeMax = updateConf.range;
                // 被ダメージ増加
                // 継続ダメージ
                // 強化解除
                // ドレイン（回復）

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            elapsedTime.Value = 0f;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!_turretUtility.UpdateScale(elapsedTime, _disableTimeSec, RANGE_MIN, _rangeMax, attackColliderOfOnmyoBullet))
                Debug.LogError("UpdateScale");
        }
    }
}
