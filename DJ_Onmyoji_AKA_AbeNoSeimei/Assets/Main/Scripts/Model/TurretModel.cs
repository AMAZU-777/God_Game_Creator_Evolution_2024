using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Utility;
using Main.Common;
using UniRx;
using Universal.Utility;
using UniRx.Triggers;

namespace Main.Model
{
    /// <summary>
    /// 砲台系
    /// モデル
    /// </summary>
    public abstract class TurretModel : SpawnModel, ITurretModel
    {
        /// <summary>トランスフォーム</summary>
        private Transform _transform;
        /// <summary>トランスフォーム</summary>
        private Transform Transform => _transform != null ? _transform : _transform = transform;
        /// <summary>Rectトランスフォーム</summary>
        protected RectTransform RectTransform => Transform as RectTransform;
        /// <summary>式神タイプ別パラメータ管理</summary>
        protected ShikigamiParameterUtility _shikigamiUtility = new ShikigamiParameterUtility();
        /// <summary>砲台系ユーティリティ</summary>
        protected TurretUtility _turretUtility = new TurretUtility();
        /// <summary>式神の情報</summary>
        protected ShikigamiInfo _shikigamiInfo;
        /// <summary>インスタンスID</summary>
        public int InstanceID { get; private set; }
        /// <summary>ジョッキーコマンドタイプ</summary>
        private JockeyCommandType _jockeyCommandType = JockeyCommandType.None;
        /// <summary>クローンオブジェクトを生成する時間間隔（秒）のバフ補正値</summary>
        [SerializeField] private float instanceRateTimeSecCorrection = 2f;
        /// <summary>通常攻撃のループが有効か</summary>
        protected bool _isUnLoopNormalActionRate;
        /// <summary>弾の角度を動的に管理</summary>
        protected BulletCompass _bulletCompass;
        /// <summary>共通のユーティリティ</summary>
        protected MainCommonUtility _mainCommonUtility = new MainCommonUtility();

        protected virtual void Awake()
        {
            InstanceID = GetInstanceID();
        }

        protected override void Start()
        {
            var obj = GameObject.Find(ConstGameObjectNames.GAMEOBJECT_NAME_PENTAGRAMTURNTABLE);
            if (obj != null)
            {
                var model = obj.GetComponent<PentagramTurnTableModel>();
                _shikigamiInfo = _shikigamiUtility.GetShikigamiInfo(model.PentagramTurnTableInfo, InstanceID);
            }
            _shikigamiInfo.state.tempoLevel = new FloatReactiveProperty();
            _bulletCompass.bulletCompassType = BulletCompassType.Default;
            base.Start();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <returns>魔力弾の設定</returns>
        protected abstract OnmyoBulletConfig InitializeOnmyoBulletConfig();
        /// <summary>
        /// 設定のリロード（テンポレベルによる可変効果）
        /// </summary>
        /// <returns>魔力弾の設定</returns>
        protected abstract OnmyoBulletConfig ReLoadOnmyoBulletConfig(OnmyoBulletConfig config);

        /// <summary>
        /// 魔力弾／円舞範囲／デバフ魔力弾の制御
        /// </summary>
        /// <param name="objectsPoolModel">オブジェクトプール</param>
        /// <returns>成功／失敗</returns>
        protected abstract bool ActionOfBullet(ObjectsPoolModel objectsPoolModel, OnmyoBulletConfig onmyoBulletConfig);

        protected override bool InstanceCloneObjects(float instanceRateTimeSec, ObjectsPoolModel objectsPoolModel)
        {
            try
            {
                var config = InitializeOnmyoBulletConfig();
                float elapsedTime = 0f;
                // 一定間隔で弾を生成するための実装
                this.UpdateAsObservable()
                    .Where(_ => !_isUnLoopNormalActionRate)
                    .Subscribe(_ =>
                    {
                        config = ReLoadOnmyoBulletConfig(config);
                        if (!_spawnUtility.ManageBulletSpawn(_jockeyCommandType,
                            instanceRateTimeSecCorrection,
                            objectsPoolModel,
                            config,
                            ref elapsedTime,
                            ActionOfBullet))
                            Debug.LogError("ManageBulletSpawn");
                    });

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool SetJockeyCommandType(JockeyCommandType jockeyCommandType)
        {
            try
            {
                _jockeyCommandType = jockeyCommandType;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// テンポレベルを更新
        /// </summary>
        /// <param name="tempoLevel">テンポレベル</param>
        /// <param name="shikigamiType">式神タイプ</param>
        /// <returns>成功／失敗</returns>
        public abstract bool UpdateTempoLvValue(float tempoLevel, ShikigamiType shikigamiType);
    }

    /// <summary>
    /// 砲台系
    /// モデル
    /// インターフェース
    /// </summary>
    public interface ITurretModel
    {
        /// <summary>
        /// ジョッキーコマンドタイプをセット
        /// </summary>
        /// <param name="jockeyCommandType">ジョッキーコマンドタイプ</param>
        /// <returns>成功／失敗</returns>
        public bool SetJockeyCommandType(JockeyCommandType jockeyCommandType);
    }

    /// <summary>
    /// 弾の角度を動的に管理
    /// </summary>
    public struct BulletCompass
    {
        /// <summary>移動方向（デフォルト）</summary>
        public Vector2 moveDirectionDefault;
        /// <summary>移動方向（中心から外側）</summary>
        public Vector2 moveDirectionCenterBetweenOutSide;
        /// <summary>移動方向（ダンスの前方）</summary>
        public Vector2 moveDirectionDanceForward;
        /// <summary>弾の角度タイプ</summary>
        public BulletCompassType bulletCompassType;
    }

    /// <summary>
    /// 弾の角度タイプ
    /// </summary>
    public enum BulletCompassType
    {
        /// <summary>デフォルト</summary>
        Default,
        /// <summary>中心から外側</summary>
        CenterBetweenOutSide,
        /// <summary>ダンスの前方</summary>
        DanceForward,
    }
}
