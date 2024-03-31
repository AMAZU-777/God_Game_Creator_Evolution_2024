using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Main.Model
{
    /// <summary>
    /// モブキャラクターをプール管理する
    /// モデル
    /// </summary>
    public class MobCharacter : MonoBehaviour, IMobCharacter
    {
        /// <summary>相手への攻撃ヒット判定のトリガー</summary>
        [Tooltip("相手への攻撃ヒット判定のトリガー")]
        [SerializeField] protected AttackColliderOfOnmyoBullet attackCollider;
        /// <summary>貫通か</summary>
        private bool _IsPenetrating;

        protected virtual void Reset()
        {
            attackCollider = GetComponentInChildren<AttackColliderOfOnmyoBullet>();
        }

        protected virtual void Awake()
        {
            gameObject.SetActive(false);
        }

        protected virtual void OnDisable()
        {
            _IsPenetrating = false;
        }

        protected virtual void Start()
        {
            attackCollider.IsHit.ObserveEveryValueChanged(x => x.Value)
                .Subscribe(x =>
                {
                    if (x &&
                        !_IsPenetrating)
                        gameObject.SetActive(false);
                });
        }

        public bool SetPenetrating(bool penetrating)
        {
            try
            {
                _IsPenetrating = penetrating;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }

    /// <summary>
    /// モブキャラクターをプール管理する
    /// モデル
    /// インターフェース
    /// </summary>
    public interface IMobCharacter
    {
        /// <summary>
        /// 貫通をセット
        /// </summary>
        /// <param name="penetrating">貫通か</param>
        /// <returns>成功／失敗</returns>
        public bool SetPenetrating(bool penetrating);
    }
}
