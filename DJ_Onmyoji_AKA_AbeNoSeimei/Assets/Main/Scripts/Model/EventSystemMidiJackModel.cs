using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Main.Common;
using System.Linq;
using UnityEngine.EventSystems;
using Universal.Utility;
using Main.InputSystem;

namespace Main.Model
{
    /// <summary>
    /// MIDIJack用
    /// イベントシステム
    /// </summary>
    public class EventSystemMidiJackModel : MonoBehaviour
    {
        /// <summary>UIのボタン</summary>
        [SerializeField] private Button[] buttons;
        /// <summary>入力無視の時間（秒）</summary>
        [SerializeField] private float unDeadTimeSec = .2f;

        private void Reset()
        {
            buttons = GameObject.Find("Canvas").GetComponentsInChildren<Button>();
        }

        private void Start()
        {
            Button currentButton = null;
            this.UpdateAsObservable()
                .Select(_ => MainGameManager.Instance)
                .Where(x => x != null &&
                    x.InputSystemsOwner.CurrentInputMode.Value == (int)InputMode.MidiJackTourchOSC)
                .Take(1)
                .Select(x => x.InputSystemsOwner.InputMidiJackTouchOSC)
                .Subscribe(x =>
                {
                    BoolReactiveProperty isLockScroll = new BoolReactiveProperty();
                    currentButton = buttons.FirstOrDefault(x => x == EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
                    BoolReactiveProperty isSubmited = new BoolReactiveProperty();
                    isSubmited.ObserveEveryValueChanged(x => x.Value)
                        .Where(x => x)
                        .Subscribe(_ => ExecuteEvents.Execute(currentButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler));
                    this.UpdateAsObservable()
                        .Subscribe(_ =>
                        {
                            if (0f < Mathf.Abs(x.Scratch) &&
                                !isLockScroll.Value)
                            {
                                isLockScroll.Value = true;
                                StartCoroutine(GeneralUtility.ActionsAfterDelay(unDeadTimeSec, () => isLockScroll.Value = false));
                                if (0f < x.Scratch)
                                {
                                    if (!Scroll(EventSystemMidiJackModelScroll.Back, ref currentButton))
                                        Debug.LogError("Scroll");
                                }
                                else if (x.Scratch < 0f)
                                {
                                    if (!Scroll(EventSystemMidiJackModelScroll.Next, ref currentButton))
                                        Debug.LogError("Scroll");
                                }
                            }
                            if (x.Submited)
                                isSubmited.Value = x.Submited;
                        });
                });
        }

        /// <summary>
        /// スクロールする
        /// </summary>
        /// <param name="eventSystemMidiJackModelScroll">スクロール方向</param>
        /// <param name="currentButton">現在のボタン</param>
        /// <returns>成功／失敗</returns>
        private bool Scroll(EventSystemMidiJackModelScroll eventSystemMidiJackModelScroll, ref Button currentButton)
        {
            try
            {
                switch (eventSystemMidiJackModelScroll)
                {
                    case EventSystemMidiJackModelScroll.Back:
                        if (currentButton.navigation.selectOnUp != null)
                        {
                            currentButton = (Button)currentButton.navigation.selectOnUp;
                            EventSystem.current.SetSelectedGameObject(currentButton.gameObject);
                        }

                        break;
                    case EventSystemMidiJackModelScroll.Next:
                        if (currentButton.navigation.selectOnDown != null)
                        {
                            currentButton = (Button)currentButton.navigation.selectOnDown;
                            EventSystem.current.SetSelectedGameObject(currentButton.gameObject);
                        }

                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException($"指定不可な条件:[{eventSystemMidiJackModelScroll}]");
                }

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
    /// スクロール方向
    /// </summary>
    public enum EventSystemMidiJackModelScroll
    {
        /// <summary>前へ</summary>
        Back = 0,
        /// <summary>次へ</summary>
        Next = 1,
    }
}
