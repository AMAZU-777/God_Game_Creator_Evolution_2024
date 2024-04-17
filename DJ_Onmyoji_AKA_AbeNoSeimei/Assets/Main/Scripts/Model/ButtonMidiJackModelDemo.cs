using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.Model
{
    /// <summary>
    /// MIDIJack経由でボタンへ入力を伝えるモデル
    /// </summary>
    public class ButtonMidiJackModelDemo : MonoBehaviour, ISubmitHandler
    {
        public void OnSubmit(BaseEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
