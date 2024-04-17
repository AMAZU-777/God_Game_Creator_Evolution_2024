using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Main.Common;
using Main.InputSystem;
using Main.Model;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Main.Utility
{
    /// <summary>
    /// ジョッキーコマンドのユーティリティクラス
    /// </summary>
    public class JockeyCommandUtility : IJockeyCommandUtility
    {
        public bool UpdateJockeyCommandType(IReactiveProperty<float> inputValue, InputBackSpinState inputBackSpinState, InputSlipLoopState inputSlipLoopState, IReactiveProperty<int> jockeyCommandType, float autoSpinSpeed, int inputHistoriesLimit)
        {
            try
            {
                if (!SetScratch(inputValue, jockeyCommandType, autoSpinSpeed, inputHistoriesLimit))
                    throw new System.Exception("SetScratch");
                if (!SetBackSpin(inputBackSpinState, jockeyCommandType))
                    throw new System.Exception("SetBackSpin");
                if (!SetSlipLoop(inputSlipLoopState, jockeyCommandType))
                    throw new System.Exception("SetSlipLoop");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// スクラッチ、ホールドをセット
        /// </summary>
        /// <param name="inputValue">入力角度</param>
        /// <param name="jockeyCommandType">ジョッキーコマンドタイプ</param>
        /// <param name="autoSpinSpeed">自動回転の速度</param>
        /// <param name="inputHistoriesLimit">コマンドの入力数</param>
        /// <returns>成功／失敗</returns>
        private bool SetScratch(IReactiveProperty<float> inputValue, IReactiveProperty<int> jockeyCommandType, float autoSpinSpeed, int inputHistoriesLimit)
        {
            try
            {
                // 入力履歴を保持するリスト
                List<float> inputHistory = new List<float>();

                // inputValueの変更を購読
                inputValue.ObserveEveryValueChanged(x => x.Value)
                    .Subscribe(value => 
                    {
                        // 入力履歴が10を超えた場合、最初の要素を削除
                        if (inputHistory.Count >= inputHistoriesLimit)
                            inputHistory.RemoveAt(0);

                        // 最後の入力と異なる場合のみ履歴に追加
                        if (inputHistory.Count == 0 ||
                            inputHistory[inputHistory.Count - 1] != value)
                            inputHistory.Add(value);

                        // 最後の入力が0の場合、jockeyCommandTypeをHoldに設定
                        if (value == 0 ||
                            isScratch(inputHistory, autoSpinSpeed))
                        {
                            if (value == 0)
                                jockeyCommandType.Value = (int)JockeyCommandType.Hold;
                            if (isScratch(inputHistory, autoSpinSpeed))
                            {
                                jockeyCommandType.Value = (int)JockeyCommandType.Scratch;
                                if (0 < inputHistory.Count)
                                    inputHistory.Clear();
                            }
                        }
                        else if (value == autoSpinSpeed)
                        {
                            jockeyCommandType.Value = (int)JockeyCommandType.None;
                            if (0 < inputHistory.Count)
                                inputHistory.Clear();
                        }
                    });
                
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// スクラッチ判定
        /// </summary>
        /// <param name="inputHistory">入力履歴</param>
        /// <param name="autoSpinSpeed">自動回転の速度</param>
        /// <returns>スクラッチ状態か</returns>
        private bool isScratch(List<float> inputHistory, float autoSpinSpeed)
        {
            if (inputHistory.Count < 3)
                return false;

            var ignoreZeroGroup = inputHistory.Where(q => (0f < q || q < 0f) &&
                q != autoSpinSpeed)
                .Select((p, i) => new { Content = 0f < p ? 1f : (p < 0f ? -1f : 0f), Index = i })
                .OrderBy(q => q.Content).ThenByDescending(q => q.Index)
                .GroupBy(x => x.Content);
            var ignoreZero = ignoreZeroGroup.Select(g => g.First())
                .Union(ignoreZeroGroup.Select(g => g.Last()))
                .Select(q => q.Content)
                .ToArray();

            if (2 < ignoreZero.Length)
                return (ignoreZero[ignoreZero.Length - 3] > 0 && ignoreZero[ignoreZero.Length - 2] < 0 && ignoreZero[ignoreZero.Length - 1] > 0) ||
                    (ignoreZero[ignoreZero.Length - 3] < 0 && ignoreZero[ignoreZero.Length - 2] > 0 && ignoreZero[ignoreZero.Length - 1] < 0);
            else
                return false;
        }

        /// <summary>
        /// バックスピンをセット
        /// </summary>
        /// <param name="inputBackSpinState">バックスピンの入力情報</param>
        /// <param name="jockeyCommandType">ジョッキーコマンドタイプ</param>
        /// <returns>成功／失敗</returns>
        private bool SetBackSpin(InputBackSpinState inputBackSpinState, IReactiveProperty<int> jockeyCommandType)
        {
            try
            {
                // 入力履歴を保持するリスト
                List<Vector2> inputHistory = new List<Vector2>();
                inputBackSpinState.inputVelocityValue.ObserveEveryValueChanged(x => x.Value)
                    .Subscribe(x =>
                    {
                        // 入力受付時間を超えたら入力リストを削除
                        if (inputBackSpinState.recordInputTimeSecLimit <= inputBackSpinState.recordInputTimeSec.Value ||
                            x.sqrMagnitude == 0f)
                            inputHistory.Clear();
                        // 最後の入力と異なる場合のみ履歴に追加
                        if (inputHistory.Count < 1 ||
                            inputHistory[inputHistory.Count - 1] != x)
                            inputHistory.Add(x);
                        // 入力角度の合計を計算
                        float angleSum = 0f;
                        if (1 < inputHistory.Count)
                            foreach (var item in inputHistory.Select((p, i) => new { Content = p, Index = i })
                                .Where(q => q.Index < inputHistory.Count - 1))
                                angleSum += Vector2.SignedAngle(item.Content, inputHistory[item.Index + 1]);
                        if (inputBackSpinState.targetAngle <= angleSum)
                            jockeyCommandType.Value = (int)JockeyCommandType.BackSpin;
                    });

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// スリップループをセット
        /// </summary>
        /// <param name="inputSlipLoopState">スリップループの入力情報</param>
        /// <param name="jockeyCommandType">ジョッキーコマンドタイプ</param>
        /// <returns>成功／失敗</returns>
        private bool SetSlipLoop(InputSlipLoopState inputSlipLoopState, IReactiveProperty<int> jockeyCommandType)
        {
            try
            {
                switch ((InputMode)MainGameManager.Instance.InputSystemsOwner.CurrentInputMode.Value)
                {
                    case InputMode.Gamepad:
                        inputSlipLoopState.crossVectorHistory.ObserveAdd()
                            .Select(x => x.Value)
                            .Subscribe(x =>
                            {
                                // スリップループ初回
                                if (jockeyCommandType.Value != (int)JockeyCommandType.SlipLoop &&
                                    0f < Mathf.Abs(x.sqrMagnitude))
                                {
                                    jockeyCommandType.Value = (int)JockeyCommandType.SlipLoop;
                                    inputSlipLoopState.beatLength.Value = (int)GetBeatLength(x);
                                }
                                // スリップループ切り替え
                                else if (jockeyCommandType.Value == (int)JockeyCommandType.SlipLoop)
                                {
                                    // 最後に入力された方向を軸にして要素を1つずつ戻して比較、異なった時点までの件数を算出
                                    var lastInput = inputSlipLoopState.crossVectorHistory.Last();
                                    int differentCount = 0;
                                    for (int i = inputSlipLoopState.crossVectorHistory.Count - 2; i >= 0; i--)
                                        if (inputSlipLoopState.crossVectorHistory[i] != lastInput)
                                        {
                                            differentCount = inputSlipLoopState.crossVectorHistory.Count - 1 - i;
                                            break;
                                        }
                                    // 偶数なら再入力、奇数なら切り替えとする
                                    if (differentCount % 2 == 0)
                                    {
                                        inputSlipLoopState.beatLength.Value = (int)BeatLength.None;
                                        jockeyCommandType.Value = (int)JockeyCommandType.None;
                                        inputSlipLoopState.crossVectorHistory.Clear();
                                    }
                                    else
                                    {
                                        inputSlipLoopState.beatLength.Value = (int)GetBeatLength(x);
                                        // 最後の要素以外は削除
                                        for (int i = 0; i < inputSlipLoopState.crossVectorHistory.Count - 1; i++)
                                            inputSlipLoopState.crossVectorHistory.RemoveAt(0);
                                    }
                                }
                            });

                        break;
                    case InputMode.MidiJack:
                        inputSlipLoopState.crossVector.ObserveEveryValueChanged(x => x.Value)
                            .Subscribe(x =>
                            {
                                // スリップループ初回
                                if (jockeyCommandType.Value != (int)JockeyCommandType.SlipLoop &&
                                    0f < Mathf.Abs(x.sqrMagnitude))
                                {
                                    jockeyCommandType.Value = (int)JockeyCommandType.SlipLoop;
                                    inputSlipLoopState.beatLength.Value = (int)GetBeatLength(x);
                                }
                                // スリップループ切り替え
                                else if (jockeyCommandType.Value == (int)JockeyCommandType.SlipLoop)
                                {
                                    if (0f < Mathf.Abs(x.sqrMagnitude))
                                        inputSlipLoopState.beatLength.Value = (int)GetBeatLength(x);
                                    else
                                    {
                                        inputSlipLoopState.beatLength.Value = (int)BeatLength.None;
                                        jockeyCommandType.Value = (int)JockeyCommandType.None;
                                    }
                                }
                            });

                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException($"未対応の入力モード:{(InputMode)MainGameManager.Instance.InputSystemsOwner.CurrentInputMode.Value}");
                }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// 十字キー入力から拍の長さを取得
        /// </summary>
        /// <param name="crossVector">十字キー入力</param>
        /// <returns>拍の長さ</returns>
        private BeatLength GetBeatLength(Vector2 crossVector)
        {
            if (crossVector.Equals(Vector2.up) &&
                !crossVector.Equals(Vector2.right) &&
                !crossVector.Equals(Vector2.down) &&
                !crossVector.Equals(Vector2.left))
                return BeatLength.TwoBeats;
            else if (crossVector.Equals(Vector2.right) &&
                !crossVector.Equals(Vector2.down) &&
                !crossVector.Equals(Vector2.left))
                return BeatLength.OneBeat;
            else if (crossVector.Equals(Vector2.down) &&
                !crossVector.Equals(Vector2.left))
                return BeatLength.HalfBeat;
            else
                return BeatLength.QuarterBeat;
        }

        public bool SetNone(IReactiveProperty<int> jockeyCommandType)
        {
            try
            {
                jockeyCommandType.Value = (int)JockeyCommandType.None;

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
    /// ジョッキーコマンドのユーティリティクラス
    /// インターフェース
    /// </summary>
    public interface IJockeyCommandUtility
    {
        /// <summary>
        /// ジョッキーコマンドタイプを更新
        /// </summary>
        /// <param name="inputValue">入力角度</param>
        /// <param name="inputBackSpinState">バックスピンの入力情報</param>
        /// <param name="inputSlipLoopState">スリップループの入力情報</param>
        /// <param name="jockeyCommandType">ジョッキーコマンドタイプ</param>
        /// <param name="autoSpinSpeed">自動回転の速度</param>
        /// <param name="inputHistoriesLimit">コマンドの入力数</param>
        /// <returns>成功／失敗</returns>
        public bool UpdateJockeyCommandType(IReactiveProperty<float> inputValue, InputBackSpinState inputBackSpinState, InputSlipLoopState inputSlipLoopState, IReactiveProperty<int> jockeyCommandType, float autoSpinSpeed, int inputHistoriesLimit);
        /// <summary>
        /// ジョッキーコマンドへNoneをセット
        /// </summary>
        /// <param name="jockeyCommandType">ジョッキーコマンド</param>
        /// <returns>成功／失敗</returns>
        public bool SetNone(IReactiveProperty<int> jockeyCommandType);
    }
}
