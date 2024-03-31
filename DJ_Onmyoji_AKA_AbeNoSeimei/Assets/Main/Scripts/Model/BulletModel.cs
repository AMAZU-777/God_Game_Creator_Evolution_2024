using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Common;
using Universal.Utility;
using Main.Common;
using Main.Utility;

namespace Main.Model
{
    /// <summary>
    /// 弾系モデル
    /// </summary>
    public class BulletModel : MobCharacter
    {
        /// <summary>移動方向</summary>
        protected Vector2 _moveDirection;
        /// <summary>移動速度</summary>
        protected float _moveSpeed;
        /// <summary>トランスフォーム</summary>
        private Transform _transform;
        /// <summary>トランスフォーム</summary>
        protected Transform Transform => _transform != null ? _transform : _transform = transform;
        /// <summary>停止するまでの時間</summary>
        protected float _disableTimeSec;
        /// <summary>設定</summary>
        [SerializeField] protected OnmyoBulletConfig onmyoBulletConfig;
        /// <summary>敵が攻撃範囲へ侵入した判定のトリガー</summary>
        [Tooltip("敵が攻撃範囲へ侵入した判定のトリガー")]
        [SerializeField] private SearchRangeOfEnemyCollider searchRangeOfEnemyCollider;
        /// <summary>攻撃を与える判定のトリガー</summary>
        [SerializeField] protected AttackColliderOfOnmyoBullet attackColliderOfOnmyoBullet;
        /// <summary>砲台系ユーティリティ</summary>
        protected TurretUtility _turretUtility = new TurretUtility();
        // /// <summary>連射</summary>
        // protected OnmyoBulletConfigOfHighEnd _continuousFire;
        

        protected override void Reset()
        {
            searchRangeOfEnemyCollider = GetComponentInChildren<SearchRangeOfEnemyCollider>();
            attackColliderOfOnmyoBullet = GetComponentInChildren<AttackColliderOfOnmyoBullet>();
            base.Reset();
        }

        protected override void Awake()
        {
            var adminDataSingleton = AdminDataSingleton.Instance != null ?
                AdminDataSingleton.Instance :
                new GameObject(Universal.Common.ConstGameObjectNames.GAMEOBJECT_NAME_ADMINDATA_SINGLETON).AddComponent<AdminDataSingleton>()
                    .GetComponent<AdminDataSingleton>();
            onmyoBulletConfig.moveDirection = adminDataSingleton.AdminBean.onmyoBulletModel.moveDirection;
            onmyoBulletConfig.moveSpeed = adminDataSingleton.AdminBean.onmyoBulletModel.moveSpeed;
            base.Awake();
        }

        protected virtual void OnEnable()
        {
            StartCoroutine(GeneralUtility.ActionsAfterDelay(_disableTimeSec, () => gameObject.SetActive(false)));
        }

        protected virtual void FixedUpdate()
        {
            _turretUtility.MoveBullet(searchRangeOfEnemyCollider.Target, _moveDirection, _moveSpeed, Transform);
        }
    }

    /// <summary>
    /// 弾系モデル
    /// インターフェース
    /// </summary>
    public interface IBulletModel
    {
        /// <summary>
        /// 初期設定
        /// </summary>
        /// <param name="position">生成位置</param>
        /// <param name="eulerAngles">初期角度</param>
        /// <param name="updateConf">更新後の設定</param>
        /// <returns>成功／失敗</returns>
        public bool Initialize(Vector2 position, Vector3 eulerAngles, OnmyoBulletConfig updateConf);
    }
}
