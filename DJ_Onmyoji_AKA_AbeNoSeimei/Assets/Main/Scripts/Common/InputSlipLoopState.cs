using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Main.Common
{
    /// <summary>
    /// スリップループの入力情報
    /// </summary>
    [System.Serializable]
    public struct InputSlipLoopState
    {
        /// <summary>拍の長さ</summary>
        public IReactiveProperty<int> beatLength;
        /// <summary>十字キー入力履歴（コントローラーのみ）</summary>
        public IReactiveCollection<Vector2> crossVectorHistory;
        /// <summary>十字キー入力情報（MIDIJackのみ）</summary>
        public IReactiveProperty<Vector2> crossVector;
        /// <summary>ループ中</summary>
        public IReactiveProperty<bool> IsLooping { get; set; }
        /// <summary>アクショントリガー</summary>
        public IReactiveProperty<bool> ActionTrigger { get; set; }
    }

    /// <summary>
    /// 拍の長さ
    /// </summary>
    public enum BeatLength
    {
        /// <summary>無効</summary>
        None,
        /// <summary>2拍</summary>
        TwoBeats,
        /// <summary>1拍</summary>
        OneBeat,
        /// <summary>1/2拍</summary>
        HalfBeat,
        /// <summary>1/4拍</summary>
        QuarterBeat,
    }

    /// <summary>
    /// 拍の情報を管理
    /// </summary>
    public class BeatLengthApp
    {
        /// <summary>
        /// 拍の長さを元に値を算出して取得
        /// 曲の長さ、五芒星の角度を扱う
        /// </summary>
        /// <param name="inputSlipLoopState">スリップループの入力情報</param>
        /// <param name="baseValue">ベース値</param>
        /// <returns>拍補正後の値</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">指定不可の条件</exception>
        public static float GetTotalReverse(InputSlipLoopState inputSlipLoopState, float baseValue)
        {
            return (BeatLength)inputSlipLoopState.beatLength.Value switch
            {
                BeatLength.TwoBeats => baseValue * 2f,
                BeatLength.OneBeat => baseValue * 1f,
                BeatLength.HalfBeat => baseValue * .5f,
                BeatLength.QuarterBeat => baseValue * .25f,
                _ => throw new System.ArgumentOutOfRangeException($"指定不可の条件:[{(BeatLength)inputSlipLoopState.beatLength.Value}]"),
            };
        }
    }
}
