using System.Collections;
using System.Collections.Generic;
using Effect.Model;
using Effect.Utility;
using Effect.Common;
using UnityEngine;
using UniRx;

namespace Main.View
{
    /// <summary>
    /// ラップ
    /// ビュー
    /// </summary>
    public class WrapTurretView : MonoBehaviour, IWrapTurretView
    {
        /// <summary>エフェクトプール</summary>
        private EffectsPoolModel _effectsPoolModel;
        /// <summary>エフェクトユーティリティ</summary>
        private EffectUtility _effectUtility = new EffectUtility();
        /// <summary>エフェクトプレハブ</summary>
        [SerializeField] private Transform effecctsPoolPrefab;

        private void Start()
        {
            _effectsPoolModel = _effectUtility.FindOrInstantiateForGetEffectsPoolModel(effecctsPoolPrefab);
        }

        public IEnumerator PlayEffectExplosion(System.IObserver<bool> observer, int index)
        {
            var particleSystem = _effectsPoolModel.GetShikigamiWrapExplosion();

            // パーティクルシステムの再生完了を監視し、完了時にコールバックを実行
            particleSystem.PlayAsync()
                .Subscribe(_ => observer.OnNext(true))
                .AddTo(gameObject);

            yield return null;
        }
    }

    /// <summary>
    /// ラップ
    /// ビュー
    /// インターフェース
    /// </summary>
    public interface IWrapTurretView
    {
        /// <summary>
        /// 爆発エフェクトを再生
        /// </summary>
        /// <param name="observer">バインド</param>
        /// <returns>コルーチン</returns>
        public IEnumerator PlayEffectExplosion(System.IObserver<bool> observer, int index);
    }
}
