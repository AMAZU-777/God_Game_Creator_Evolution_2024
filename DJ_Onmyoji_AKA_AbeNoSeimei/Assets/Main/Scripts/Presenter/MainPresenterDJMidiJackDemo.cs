using Main.Model;
using Main.Test.Driver;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Presenter
{
    public class MainPresenterDJMidiJackDemo : MonoBehaviour
    {
        [SerializeField] private PentagramSystemModel pentagramSystemModel;
        [SerializeField] private PentagramSystemModelTest pentagramSystemTest;

        private void Reset()
        {
            pentagramSystemModel = GameObject.Find("PentagramSystem").GetComponent<PentagramSystemModel>();
            pentagramSystemTest = GameObject.Find("PentagramSystemModelTest").GetComponent<PentagramSystemModelTest>();
        }

        // Start is called before the first frame update
        void Start()
        {
            //pentagramSystemModel.InputValue.ObserveEveryValueChanged(x => x.Value)
            //    .Subscribe(x => Debug.Log($"InputValue:[{x}]"));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
