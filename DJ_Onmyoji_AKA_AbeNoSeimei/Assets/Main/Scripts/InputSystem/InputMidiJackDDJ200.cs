using Main.InputSystem;
using MidiJack;
using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UniRx;
using UnityEngine;

public class InputMidiJackDDJ200 : MonoBehaviour
{
    private void Start()
    {
        MidiMaster.knobDelegate += OnScratch;
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
        _scratch = 0f;
    }

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
        if (IsMidiChannelCh1OrCh2(channel))
        {
            switch ((MidiChannelKnob)knob)
            {
                case MidiChannelKnob.D1_T:
                    if (!UpdateScratch(value))
                        Debug.LogError("UpdateScratch");

                    break;
                case MidiChannelKnob.D1_S:
                    if (!UpdateScratch(value))
                        Debug.LogError("UpdateScratch");

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
    /// スクラッチの値を更新
    /// </summary>
    /// <param name="value">値</param>
    /// <returns>成功／失敗</returns>
    private bool UpdateScratch(float value)
    {
        try
        {
            // valueが0（反時計回り）か1（時計回り）の場合、それに応じて_scratchを更新
            if (value == .496063f)
            {
                // 反時計回りの場合、_scratchを減少させる
                _scratch += scratchLevel; // この値は調整が必要かもしれません
            }
            else if (value == .511811f)
            {
                // 時計回りの場合、_scratchを増加させる
                _scratch -= scratchLevel; // この値は調整が必要かもしれません
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
    /// チャンネル1を使用しているか
    /// </summary>
    /// <param name="channel">MIDIチャンネル</param>
    /// <returns>使用／未使用</returns>
    private bool IsMidiChannelCh1OrCh2(MidiChannel channel)
    {
        try
        {
            switch (channel)
            {
                case MidiChannel.Ch1:
                    return true;
                case MidiChannel.Ch2:
                    return true;
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

    /// <summary>
    /// LayOUTがAutomat5のA～D,M
    /// </summary>
    private enum MidiChannelKnob
    {
        D1_T = 34,
        D1_S = 33,
        M1
        //A = 0,
        //B = 1,
        //C = 2,
        //D = 3,
        //M = 26,
        //A_Submit = 29,
        //A_Pad_2 = 30,
    }
}
