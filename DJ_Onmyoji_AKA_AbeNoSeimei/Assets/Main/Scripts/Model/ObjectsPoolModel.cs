using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Main.View;
using UniRx;
using UnityEngine;
using Universal.Common;

namespace Main.Model
{
    /// <summary>
    /// オブジェクトプール
    /// モデル
    /// </summary>
    public class ObjectsPoolModel : MonoBehaviour, IObjectsPoolModel
    {
        /// <summary>トランスフォーム</summary>
        private Transform _transform;
        /// <summary>プール数の上限</summary>
        [Tooltip("プール数の上限")]
        [SerializeField] private int countLimit;
        /// <summary>プール完了</summary>
        public IReactiveProperty<bool> IsCompleted { get; private set; } = new BoolReactiveProperty();
        /// <summary>魔力弾ステート</summary>
        private OnmyoBulletModelState[] _onmyoBulletModelStates;
        /// <summary>魔力弾ステート</summary>
        public OnmyoBulletModelState[] OnmyoBulletModelStates => _onmyoBulletModelStates;
        /// <summary>魔力弾のプレハブ</summary>
        [Tooltip("魔力弾のプレハブ")]
        [SerializeField] private Transform onmyoBulletPrefab;
        /// <summary>魔力弾配列</summary>
        private List<OnmyoBulletModel> _onmyoBulletModels = new List<OnmyoBulletModel>();
        /// <summary>魔力弾（ラップ用）のプレハブ</summary>
        [SerializeField] private Transform wrapBulletPrefab;
        /// <summary>魔力弾（ラップ用）配列</summary>
        private List<WrapBulletModel> _wrapBulletModels = new List<WrapBulletModel>();
        /// <summary>ダンスホールのプレハブ</summary>
        [SerializeField] private Transform danceHallPrefab;
        /// <summary>ダンスホール配列</summary>
        private List<DanceHallModel> _danceHallModels = new List<DanceHallModel>();
        /// <summary>魔力弾（グラフィティ用）のプレハブ</summary>
        [SerializeField] private Transform graffitiBulletPrefab;
        /// <summary>魔力弾（グラフィティ用）配列</summary>
        private List<GraffitiBulletModel> _graffitiBulletModels = new List<GraffitiBulletModel>();
        /// <summary>敵のスポーンテーブル配列</summary>
        [SerializeField] private EnemiesSpawnAssign[] enemiesSpawnAssigns;
        /// <summary>敵配列</summary>
        private List<EnemyModel> _enemyModels = new List<EnemyModel>();
        /// <summary>魂の経験値のプレハブ</summary>
        [SerializeField] private Transform soulMoneyPrefab;
        /// <summary>魂の経験値配列</summary>
        private List<SoulMoneyView> _soulMoneyViews = new List<SoulMoneyView>();

        public OnmyoBulletModel GetOnmyoBulletModel()
        {
            return GetInactiveComponent(_onmyoBulletModels, onmyoBulletPrefab, _transform);
        }

        public WrapBulletModel GetWrapBulletModel()
        {
            return GetInactiveComponent(_wrapBulletModels, wrapBulletPrefab, _transform);
        }

        public DanceHallModel GetDanceHallModel()
        {
            return GetInactiveComponent(_danceHallModels, danceHallPrefab, _transform);
        }

        public GraffitiBulletModel GetGraffitiBulletModel()
        {
            return GetInactiveComponent(_graffitiBulletModels, graffitiBulletPrefab, _transform);
        }

        public EnemyModel GetEnemyModel(EnemiesID enemiesID)
        {
            var enemies = enemiesSpawnAssigns.Where(q => q.enemiesID.Equals(enemiesID))
                .Select(q => q.enemyPrefab)
                .ToArray();
            if (enemies.Length < 1)
            {
                Debug.LogError($"存在しないIDを指定:[{enemiesID}]");
                return null;
            }

            return GetInactiveComponent(_enemyModels.Where(q => q.EnemiesID.Equals(enemiesID))
                .Select(q => q)
                .ToList(), enemies[0], _transform);
        }

        public SoulMoneyView GetSoulMoneyView()
        {
            return GetInactiveComponent(_soulMoneyViews, soulMoneyPrefab, _transform);
        }

        private void Start()
        {
            var adminDataSingleton = AdminDataSingleton.Instance != null ?
                AdminDataSingleton.Instance :
                new GameObject(ConstGameObjectNames.GAMEOBJECT_NAME_ADMINDATA_SINGLETON).AddComponent<AdminDataSingleton>()
                    .GetComponent<AdminDataSingleton>();
            countLimit = adminDataSingleton.AdminBean.objectsPoolModel.countLimit;
            if (_transform == null)
                _transform = transform;
            Debug.Log("プール開始");
            for (int i = 0; i < countLimit; i++)
            {
                // プレハブを生成してプールする
                _onmyoBulletModels.Add(GetClone(onmyoBulletPrefab, _transform).GetComponent<OnmyoBulletModel>());
                _wrapBulletModels.Add(GetClone(wrapBulletPrefab, _transform).GetComponent<WrapBulletModel>());
                _danceHallModels.Add(GetClone(danceHallPrefab, _transform).GetComponent<DanceHallModel>());
                _graffitiBulletModels.Add(GetClone(graffitiBulletPrefab, _transform).GetComponent<GraffitiBulletModel>());
                foreach (var enemyPrefab in enemiesSpawnAssigns.Select(q => q.enemyPrefab))
                    _enemyModels.Add(GetClone(enemyPrefab, _transform).GetComponent<EnemyModel>());
                _soulMoneyViews.Add(GetClone(soulMoneyPrefab, _transform).GetComponent<SoulMoneyView>());
            }
            Debug.Log("プール完了");
            IsCompleted.Value = true;
        }

        /// <summary>
        /// プール内のクローンオブジェクトを取得
        /// </summary>
        /// <param name="cloneObject">プレハブ</param>
        /// <param name="parent">親</param>
        /// <returns>クローンオブジェクト</returns>
        private Transform GetClone(Transform cloneObject, Transform parent)
        {
            return Instantiate(cloneObject, parent);
        }

        private T GetInactiveComponent<T>(List<T> components, Transform prefab, Transform parent) where T : MonoBehaviour
        {
            var inactiveComponents = components.Where(q => !q.isActiveAndEnabled).ToArray();
            if (inactiveComponents.Length < 1)
            {
                Debug.LogWarning("プレハブ新規生成");
                var newComponent = GetClone(prefab, parent).GetComponent<T>();
                components.Add(newComponent);
                return newComponent;
            }
            else
            {
                return inactiveComponents[0];
            }
        }

        public bool KillEnemyModels(EnemiesID enemiesID)
        {
            try
            {
                foreach (var enemyModel in _enemyModels.Where(q => q.EnemiesID.Equals(enemiesID) &&
                    q.isActiveAndEnabled)
                    .Select(q => q))
                    if (!enemyModel.Kill())
                        throw new System.Exception("Kill");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }

    public interface IObjectsPoolModel
    {
        /// <summary>
        /// 魔力弾を取り出す
        /// </summary>
        /// <returns>魔力弾</returns>
        public OnmyoBulletModel GetOnmyoBulletModel();
        /// <summary>
        /// 魔力弾（ラップ用）を取り出す
        /// </summary>
        /// <returns>魔力弾（ラップ用）</returns>
        public WrapBulletModel GetWrapBulletModel();
        /// <summary>
        /// ダンスホールを取り出す
        /// </summary>
        /// <returns>ダンスホール</returns>
        public DanceHallModel GetDanceHallModel();
        /// <summary>
        /// 魔力弾（グラフィティ用）を取り出す
        /// </summary>
        /// <returns>魔力弾（グラフィティ用）</returns>
        public GraffitiBulletModel GetGraffitiBulletModel();
        /// <summary>
        /// 敵を取り出す
        /// </summary>
        /// <param name="enemiesID">敵ID</param>
        /// <returns>敵</returns>
        public EnemyModel GetEnemyModel(EnemiesID enemiesID);
        /// <summary>
        /// 魂の経験値を取り出す
        /// </summary>
        /// <returns>魂の経験値</returns>
        public SoulMoneyView GetSoulMoneyView();
        /// <summary>
        /// アクティブな敵をキルする
        /// </summary>
        /// <param name="enemiesID">敵ID</param>
        /// <returns>成功／失敗</returns>
        public bool KillEnemyModels(EnemiesID enemiesID);
    }
}
