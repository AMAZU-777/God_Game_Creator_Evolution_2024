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
        [SerializeField] private SelectLevelContentsTest selectLevelContentsTest;
        [SerializeField] private PentagramSystemModel pentagramSystemModel;
        [SerializeField] private PentagramSystemModelTest pentagramSystemTest;

        private void Reset()
        {
            selectLevelContentsTest = GameObject.Find("SelectLevelContentsTest").GetComponent<SelectLevelContentsTest>();
            pentagramSystemModel = GameObject.Find("PentagramSystem").GetComponent<PentagramSystemModel>();
            pentagramSystemTest = GameObject.Find("PentagramSystemModelTest").GetComponent<PentagramSystemModelTest>();
        }

        // Start is called before the first frame update
        void Start()
        {
            selectLevelContentsTest.IsSubmited.ObserveEveryValueChanged(x => x.Value)
                .Subscribe(x =>
                {
                    Debug.Log($"IsSubmited:[{x}]");
                });
            //pentagramSystemModel.InputValue.ObserveEveryValueChanged(x => x.Value)
            //    .Subscribe(x => Debug.Log($"InputValue:[{x}]"));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
