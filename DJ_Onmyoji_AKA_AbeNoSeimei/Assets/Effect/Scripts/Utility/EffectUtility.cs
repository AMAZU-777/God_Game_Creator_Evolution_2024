using System.Collections;
using System.Collections.Generic;
using Effect.Common;
using Effect.Model;
using UnityEngine;

namespace Effect.Utility
{
    /// <summary>
    /// エフェクトユーティリティ
    /// </summary>
    public class EffectUtility : IEffectUtility
    {
        public EffectsPoolModel FindOrInstantiateForGetEffectsPoolModel(Transform effectsPoolPrefab)
        {
            var pool = GameObject.FindGameObjectWithTag(ConstTagNames.TAG_NAME_EFFECTS_POOL);
            EffectsPoolModel poolModel;
            if (pool == null)
                poolModel = Object.Instantiate(effectsPoolPrefab).GetComponent<EffectsPoolModel>();
            else
                poolModel = pool.GetComponent<EffectsPoolModel>();

            return poolModel;
        }
    }

    /// <summary>
    /// エフェクトユーティリティ
    /// インターフェース
    /// </summary>
    public interface IEffectUtility
    {
        /// <summary>
        /// エフェクトプールモデルを取得するために
        /// 対象エフェクトを検索または生成
        /// </summary>
        /// <param name="effectsPoolPrefab">エフェクトプール</param>
        /// <returns>エフェクトプールモデル</returns>
        public EffectsPoolModel FindOrInstantiateForGetEffectsPoolModel(Transform effectsPoolPrefab);
    }
}
