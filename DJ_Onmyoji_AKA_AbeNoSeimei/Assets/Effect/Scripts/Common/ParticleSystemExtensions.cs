using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Effect.Common
{
    /// <summary>
    /// 拡張メソッド
    /// パーティクルシステムの停止を監視する
    /// </summary>
    public static class ParticleSystemExtensions
    {
        public static System.IObservable<bool> PlayAsync(this ParticleSystem particleSystem)
        {
            return Observable.Create<bool>(observer =>
            {
                if (!particleSystem.gameObject.activeSelf)
                {
                    particleSystem.gameObject.SetActive(true);
                }
                // パーティクルシステムの再生を開始
                particleSystem.Play();

                // パーティクルシステムの再生が完了するのを監視するコルーチンを開始
                MainThreadDispatcher.StartCoroutine(WaitForCompletion(particleSystem, observer));

                // オブザーバーの購読解除時の処理
                return Disposable.Create(() => particleSystem.Stop());
            });
        }

        private static IEnumerator WaitForCompletion(ParticleSystem particleSystem, System.IObserver<bool> observer)
        {
            // パーティクルシステムの再生が完了するまで待機
            while (particleSystem.isPlaying)
            {
                yield return null;
            }

            // 再生が完了したことをオブザーバーに通知
            observer.OnNext(true);
            observer.OnCompleted();
            // オブジェクトを無効にする
            particleSystem.gameObject.SetActive(false);
        }
    }
}
