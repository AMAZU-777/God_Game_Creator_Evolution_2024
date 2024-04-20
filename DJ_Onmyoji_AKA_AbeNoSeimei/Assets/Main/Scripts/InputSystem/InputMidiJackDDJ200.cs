using Main.InputSystem;
using MidiJack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMidiJackDDJ200 : MonoBehaviour
{
    private void Start()
    {
        MidiMaster.knobDelegate += OnScratch;
    }

    private void OnDestroy()
    {
        MidiMaster.knobDelegate -= OnScratch;
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
}
