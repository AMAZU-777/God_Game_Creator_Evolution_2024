using System.Collections;
using System.Collections.Generic;
using Main.Common;
using UnityEngine;

namespace Main.Model
{
    /// <summary>
    /// ダンスホール
    /// モデル
    /// </summary>
    public class DanceHallModel : BulletModel, IBulletModel
    {
        /// <summary>ターゲット</summary>
        private RectTransform _target;

        public bool Initialize(Vector2 position, Vector3 eulerAngles, OnmyoBulletConfig updateConf)
        {
            try
            {
                // ダンス
                //  ●威力、レート、範囲
                _disableTimeSec = updateConf.bulletLifeTime;
                if (!attackColliderOfOnmyoBullet.SetAttackPoint(updateConf.attackPoint))
                    throw new System.Exception("SetAttackPoint");
                if (0f < updateConf.range)
                    if (!attackColliderOfOnmyoBullet.SetRadiosOfCircleCollier2D(updateConf.range))
                        throw new System.Exception("SetRadiosOfCircleCollier2D");
                _target = updateConf.trackingOfAny;
                // ノックバック
                // 麻痺
                // 広範囲
                // 伝播

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        protected override void FixedUpdate()
        {
            if (!_turretUtility.InitiateDance(_target, Transform))
                Debug.LogError("InitiateDance");
        }
    }
}
