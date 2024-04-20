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

    /// <summary>�X�N���b�`</summary>
    private float _scratch;
    /// <summary>�X�N���b�`</summary>
    public float Scratch => _scratch;
    /// <summary>�X�N���b�`�̒l���x��</summary>
    [SerializeField] private float scratchLevel = 1f;
    /// <summary>���[�U��1���͂��s�����ώ��ԁi0.2�`0.5�b�j</summary>
    [SerializeField] private float userActionTime = .2f;

    /// <summary>
    /// Scratch�̃A�N�V�����ɉ����ăt���O���X�V
    /// </summary>
    /// <param name="context">�R�[���o�b�N</param>
    public void OnScratch(MidiChannel channel, int knob, float value)
    {
        Debug.Log($"channel:[{channel}]_knob:[{knob}]_value:[{value}]");
        if (IsMidiChannelCh1(channel))
        {
            switch ((MidiChannelKnob)knob)
            {
                case MidiChannelKnob.M:
                    // value��0�i�����v���j��1�i���v���j�̏ꍇ�A����ɉ�����_scratch���X�V
                    if (value == 0)
                    {
                        // �����v���̏ꍇ�A_scratch������������
                        _scratch += scratchLevel; // ���̒l�͒������K�v��������܂���
                    }
                    else if (value == 1)
                    {
                        // ���v���̏ꍇ�A_scratch�𑝉�������
                        _scratch -= scratchLevel; // ���̒l�͒������K�v��������܂���
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
    /// �`�����l��1���g�p���Ă��邩
    /// </summary>
    /// <param name="channel">MIDI�`�����l��</param>
    /// <returns>�g�p�^���g�p</returns>
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
