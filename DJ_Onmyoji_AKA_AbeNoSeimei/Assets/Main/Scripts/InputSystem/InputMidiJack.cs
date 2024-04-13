using MidiJack;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.InputSystem
{
    /// <summary>
    /// MIDIJackの入力を取得
    /// </summary>
    /// <see href="https://github.com/keijiro/MidiJack?tab=readme-ov-file"/>
    public class InputMidiJack : MonoBehaviour, IInputSystemsOwner
    {
        private void Start()
        {
            MidiMaster.knobDelegate += OnScratch;
            MidiMaster.knobDelegate += OnSubmited;
            FloatReactiveProperty elapsedTime = new FloatReactiveProperty();
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    elapsedTime.Value += Time.deltaTime;
                    if (userActionTime < elapsedTime.Value)
                    {
                        elapsedTime.Value = 0f;
                        _scratch = 0f;
                    }
                });
        }

        private void OnDestroy()
        {
            MidiMaster.knobDelegate -= OnScratch;
            MidiMaster.knobDelegate -= OnSubmited;
            _scratch = 0f;
        }

        /// <summary>決定入力</summary>
        private bool _submited;
        /// <summary>決定入力</summary>
        public bool Submited => _submited;
        /// <summary>
        /// Pauseのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnSubmited(MidiChannel channel, int knob, float value)
        {
            Debug.Log($"channel:[{channel}]_knob:[{knob}]_value:[{value}]");
            if (IsMidiChannelCh1(channel))
            {
                switch ((MidiChannelKnob)knob)
                {
                    case MidiChannelKnob.A_Submit:
                        _submited = value == 1f;

                        break;
                    default:
                        break;
                }
            }
        }

        ///// <summary>キャンセル入力</summary>
        //private bool _canceled;
        ///// <summary>キャンセル入力</summary>
        //public bool Canceled => _canceled;
        ///// <summary>
        ///// Cancelのアクションに応じてフラグを更新
        ///// </summary>
        ///// <param name="context">コールバック</param>
        //public void OnCanceled(InputAction.CallbackContext context)
        //{
        //    _canceled = context.ReadValueAsButton();
        //}
        /// <summary>スクラッチ</summary>
        private float _scratch;
        /// <summary>スクラッチ</summary>
        public float Scratch => _scratch;
        /// <summary>スクラッチの値レベル</summary>
        [SerializeField] private float scratchLevel = 1f;
        /// <summary>ユーザの1入力を行う平均時間（0.2～0.5秒）</summary>
        [SerializeField] private float userActionTime = .2f;

        /// <summary>
        /// Scratchのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnScratch(MidiChannel channel, int knob, float value)
        {
            Debug.Log($"channel:[{channel}]_knob:[{knob}]_value:[{value}]");
            if (IsMidiChannelCh1(channel))
            {
                switch ((MidiChannelKnob)knob)
                {
                    case MidiChannelKnob.M:
                        // valueが0（反時計回り）か1（時計回り）の場合、それに応じて_scratchを更新
                        if (value == 0)
                        {
                            // 反時計回りの場合、_scratchを減少させる
                            _scratch += scratchLevel; // この値は調整が必要かもしれません
                        }
                        else if (value == 1)
                        {
                            // 時計回りの場合、_scratchを増加させる
                            _scratch -= scratchLevel; // この値は調整が必要かもしれません
                        }

                        break;
                    default:
                        _scratch = 0f;

                        break;
                }
            }
            else
                _scratch = 0f;
        }

        /// <summary>
        /// チャンネル1を使用しているか
        /// </summary>
        /// <param name="channel">MIDIチャンネル</param>
        /// <returns>使用／未使用</returns>
        private bool IsMidiChannelCh1(MidiChannel channel)
        {
            try
            {
                switch (channel)
                {
                    case MidiChannel.Ch1:
                        return false;
                    default:
                        return false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                throw e;
            }
        }

        public void DisableAll() { }
    }

    /// <summary>
    /// LayOUTがAutomat5のA～D,M
    /// </summary>
    public enum MidiChannelKnob
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        M = 26,
        A_Submit = 29,
    }
}
