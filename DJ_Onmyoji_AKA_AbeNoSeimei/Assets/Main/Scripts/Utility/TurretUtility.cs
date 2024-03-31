using System.Collections;
using System.Collections.Generic;
using Main.Common;
using Main.Model;
using UniRx;
using UnityEngine;

namespace Main.Utility
{
    /// <summary>
    /// 砲台系
    /// ユーティリティ
    /// </summary>
    public class TurretUtility : ITurretUtility
    {
        public bool CallInitialize<T>(T component, RectTransform rectTransform, OnmyoBulletConfig onmyoBulletConfig) where T : MonoBehaviour
        {
            try
            {
                if (!((IBulletModel)component).Initialize(CalibrationFromTarget(rectTransform),
                    rectTransform.parent.eulerAngles,
                    onmyoBulletConfig))
                    Debug.LogError("Initialize");
                if (!component.isActiveAndEnabled)
                    component.gameObject.SetActive(true);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool MoveBullet(Transform target, Vector2 moveDirection, float moveSpeed, Transform transform)
        {
            try
            {
                if (target != null)
                {
                    var targetDirection = target.position - transform.position;
                    moveDirection = targetDirection.normalized;
                }
                // 指定された方向と速度に弾を移動させる
                transform.position += (Vector3)moveDirection * moveSpeed * Time.fixedDeltaTime;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool InitiateDance(RectTransform target, Transform toTransform)
        {
            try
            {
                if (target != null &&
                    toTransform != null)
                    toTransform.position = CalibrationFromTarget(target);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// ターゲット位置を元に調整（From）
        /// </summary>
        /// <param name="rectTransform">UIターゲット情報</param>
        /// <returns>成功／失敗</returns>
        private Vector3 CalibrationFromTarget(RectTransform rectTransform)
        {
            // UI要素のローカル座標を取得
            Vector3 localPosition = rectTransform.localPosition;
            // ローカル座標をワールド座標に変換
            Vector3 worldPosition = rectTransform.parent.TransformPoint(localPosition);
            return worldPosition;
        }

        public bool UpdateScale(IReactiveProperty<float> elapsedTime, float disableTimeSec, float rangeMin, float rangeMax, AttackColliderOfOnmyoBullet attackColliderOfOnmyoBullet)
        {
            try
            {
                if (0f < rangeMax)
                {
                    if (elapsedTime.Value < disableTimeSec)
                    {
                        float newRadius = Mathf.Lerp(rangeMin, rangeMax, elapsedTime.Value / disableTimeSec);
                        if (!attackColliderOfOnmyoBullet.SetRadiosOfCircleCollier2D(newRadius))
                            throw new System.Exception("SetRadiosOfCircleCollier2D");
                        elapsedTime.Value += Time.fixedDeltaTime;
                    }
                }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool InitializeBulletCompass(ref BulletCompass bulletCompass, Vector2 outSideVector, Vector2 danceVector)
        {
            try
            {
                bulletCompass.moveDirectionDefault = outSideVector;
                bulletCompass.moveDirectionCenterBetweenOutSide = outSideVector;
                bulletCompass.moveDirectionDanceForward = danceVector;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool SetBulletCompassType(ref BulletCompass bulletCompass, BulletCompassType bulletCompassType)
        {
            try
            {
                bulletCompass.bulletCompassType = bulletCompassType;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public OnmyoBulletConfig UpdateMoveDirection(BulletCompass bulletCompass, OnmyoBulletConfig config)
        {
            switch (bulletCompass.bulletCompassType)
            {
                case BulletCompassType.Default:
                    config.moveDirection = bulletCompass.moveDirectionDefault;

                    break;
                case BulletCompassType.CenterBetweenOutSide:
                    config.moveDirection = bulletCompass.moveDirectionCenterBetweenOutSide;

                    break;
                case BulletCompassType.DanceForward:
                    config.moveDirection = bulletCompass.moveDirectionDanceForward;

                    break;
                default:
                    break;
            }

            return config;
        }
    }

    /// <summary>
    /// 砲台系
    /// ユーティリティ
    /// インターフェース
    /// </summary>
    public interface ITurretUtility
    {
        /// <summary>
        /// 初期設定を呼び出す
        /// </summary>
        /// <typeparam name="T">弾系のデータ型</typeparam>
        /// <param name="component">コンポーネント</param>
        /// <returns>成功／失敗</returns>
        public bool CallInitialize<T>(T component, RectTransform rectTransform, OnmyoBulletConfig onmyoBulletConfig) where T : MonoBehaviour;
        /// <summary>
        /// 弾の角度を動的にセット初期化
        /// </summary>
        /// <param name="bulletCompass">弾の角度を動的に管理</param>
        /// <param name="outSideVector">中央から外側の向き</param>
        /// <param name="danceVector">ダンスの向き</param>
        /// <returns>成功／失敗</returns>
        public bool InitializeBulletCompass(ref BulletCompass bulletCompass, Vector2 outSideVector, Vector2 danceVector);
        /// <summary>
        /// 弾の角度タイプをセット
        /// </summary>
        /// <param name="bulletCompass">弾の角度を動的に管理</param>
        /// <param name="bulletCompassType">弾の角度タイプ</param>
        /// <returns>成功／失敗</returns>
        public bool SetBulletCompassType(ref BulletCompass bulletCompass, BulletCompassType bulletCompassType);

        /// <summary>
        /// 弾を移動させる
        /// </summary>
        /// <param name="target">ターゲット</param>
        /// <param name="moveDirection">移動方向</param>
        /// <param name="moveSpeed">移動速度</param>
        /// <param name="transform">トランスフォーム</param>
        public bool MoveBullet(Transform target, Vector2 moveDirection, float moveSpeed, Transform transform);

        /// <summary>
        /// ダンスを開始
        /// </summary>
        /// <param name="target">ターゲット</param>
        /// <param name="toTransform">自分トランスフォーム</param>
        /// <returns>成功／失敗</returns>
        public bool InitiateDance(RectTransform target, Transform toTransform);

        /// <summary>
        /// 大きさを調整
        /// </summary>
        /// <param name="elapsedTime">経過時間</param>
        /// <param name="disableTimeSec">停止するまでの時間</param>
        /// <param name="rangeMin">最小範囲</param>
        /// <param name="rangeMax">最大範囲</param>
        /// <param name="attackColliderOfOnmyoBullet">攻撃を与える判定のトリガー</param>
        /// <returns>成功／失敗</returns>
        public bool UpdateScale(IReactiveProperty<float> elapsedTime, float disableTimeSec, float rangeMin, float rangeMax, AttackColliderOfOnmyoBullet attackColliderOfOnmyoBullet);
        /// <summary>
        /// 魔力弾の設定を更新
        /// </summary>
        /// <param name="bulletCompass">弾の角度を動的に管理</param>
        /// <param name="config">魔力弾の設定</param>
        /// <returns>更新後の魔力弾の設定</returns>
        public OnmyoBulletConfig UpdateMoveDirection(BulletCompass bulletCompass, OnmyoBulletConfig config);
    }
}
