using System.Collections;
using System.Collections.Generic;
using Main.Common;
using Main.Utility;
using UniRx;
using UnityEngine;

namespace Main.Model
{
    /// <summary>
    /// スポーンの抽象クラス
    /// </summary>
    public abstract class SpawnModel : MonoBehaviour
    {
        /// <summary>オブジェクトプール</summary>
        [Tooltip("オブジェクトプール")]
        [SerializeField] protected Transform objectsPoolPrefab;
        /// <summary>クローンオブジェクトを生成する時間間隔（秒）</summary>
        [Tooltip("クローンオブジェクトを生成する時間間隔（秒）")]
        [SerializeField] protected float instanceRateTimeSec = .5f;
        /// <summary>スポーンのユーティリティ</summary>
        protected SpawnUtility _spawnUtility = new SpawnUtility();
        /// <summary>オブジェクトプール</summary>
        protected ObjectsPoolModel _poolModel;
        /// <summary>魔力弾ステート</summary>
        public OnmyoBulletModelState[] OnmyoBulletModelStates => _poolModel.OnmyoBulletModelStates;

        protected virtual void Start()
        {
            _poolModel = _spawnUtility.FindOrInstantiateForGetObjectsPoolModel(objectsPoolPrefab);
            _poolModel.IsCompleted.ObserveEveryValueChanged(x => x.Value)
                .Subscribe(x =>
                {
                    if (x)
                    {
                        if (!InstanceCloneObjects(instanceRateTimeSec, _poolModel))
                            Debug.LogError("InstanceCloneObjects");
                    }
                });
        }

        /// <summary>
        /// オブジェクトを生成
        /// </summary>
        /// <param name="instanceRateTimeSec">オブジェクトを生成する時間間隔（秒）</param>
        /// <param name="objectsPoolModel">オブジェクトプール</param>
        /// <returns>成功／失敗</returns>
        protected abstract bool InstanceCloneObjects(float instanceRateTimeSec, ObjectsPoolModel objectsPoolModel);
    }
}
