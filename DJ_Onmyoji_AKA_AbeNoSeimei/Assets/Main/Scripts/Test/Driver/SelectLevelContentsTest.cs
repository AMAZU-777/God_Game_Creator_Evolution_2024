using Main.Common;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Main.Test.Driver
{
    public class SelectLevelContentsTest : MonoBehaviour
    {
        public IReactiveProperty<bool> IsSubmited { get; private set; } = new BoolReactiveProperty();
        // Start is called before the first frame update
        void Start()
        {
            this.UpdateAsObservable()
                .Select(_ => MainGameManager.Instance)
                .Where(x => x != null)
                .Take(1)
                .Select(x => x.InputSystemsOwner.InputMidiJackTouchOSC)
                .Subscribe(x =>
                {
                    this.UpdateAsObservable()
                        .Subscribe(_ => IsSubmited.Value = x.Submited);
                });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
