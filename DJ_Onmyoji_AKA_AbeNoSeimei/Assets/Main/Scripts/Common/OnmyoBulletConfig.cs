using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Common
{
    /// <summary>
    /// 魔力弾の設定
    /// </summary>
    [System.Serializable]
    public struct OnmyoBulletConfig
    {
        /// <summary>移動方向</summary>
        [Tooltip("移動方向")]
        public Vector2 moveDirection;
        /// <summary>移動速度</summary>
        [Tooltip("移動速度")]
        public float? moveSpeed;
        /// <summary>行動間隔</summary>
        public float actionRate;
        /// <summary>停止するまでの時間</summary>
        public float bulletLifeTime;
        /// <summary>攻撃力</summary>
        public int attackPoint;
        /// <summary>攻撃範囲</summary>
        public float range;
        /// <summary>トラッキング対象</summary>
        public RectTransform trackingOfAny;
        /// <summary>デバフ効果時間</summary>
        public float debuffEffectLifeTime;
        /// <summary>爆発</summary>
        public OnmyoBulletConfigOfExplosion onmyoBulletConfigOfExplosion;
        /// <summary>連射</summary>
        public OnmyoBulletConfigOfHighEnd continuousFire;
        /// <summary>貫通</summary>
        public bool penetrating;
        /// <summary>拡散</summary>
        public OnmyoBulletConfigOfHighEnd spreading;
        /// <summary>ノックバック</summary>
        public float? knockBack;
        /// <summary>麻痺</summary>
        public float? paralysis;
        /// <summary>広範囲</summary>
        public float largeRange;
        /// <summary>伝播</summary>
        public int? propagation;
        /// <summary>被ダメージ増加</summary>
        public float? increasedDamage;
        /// <summary>継続ダメージ</summary>
        public float? poison;
        /// <summary>強化解除</summary>
        public bool cancelBuff;
        /// <summary>ドレイン（回復）</summary>
        public float? drain;
    }

    /// <summary>
    /// 魔力弾の設定
    /// 爆発
    /// </summary>
    [System.Serializable]
    public struct OnmyoBulletConfigOfExplosion
    {
        /// <summary>爆発までの時間</summary>
        public float? explosionDuration;
        /// <summary>爆発範囲</summary>
        public float? explosionRange;
    }

    /// <summary>
    /// 魔力弾の設定
    /// ハイエンド
    /// </summary>
    [System.Serializable]
    public struct OnmyoBulletConfigOfHighEnd
    {
        /// <summary>行動間隔</summary>
        public float? actionRate;
        /// <summary>生成数最大</summary>
        public int? instanceMax;
        /// <summary>拡散角度</summary>
        public float? spreadingAngle;
    }
}
