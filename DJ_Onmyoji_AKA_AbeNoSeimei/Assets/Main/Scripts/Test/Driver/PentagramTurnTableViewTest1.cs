using System.Collections;
using System.Collections.Generic;
using Main.View;
using UniRx;
using UnityEngine;

namespace Main.Test.Driver
{
    public class PentagramTurnTableViewTest1 : MonoBehaviour
    {
        [SerializeField] private PentagramTurnTableView pentagramTurnTableView;

        private void Reset()
        {
            pentagramTurnTableView = GameObject.Find("PentagramTurnTable").GetComponent<PentagramTurnTableView>();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("爆発処理を実行"))
            {
                Observable.FromCoroutine<bool>(observser => pentagramTurnTableView.PlayDirectionExplosionOfWrapTurretView(observser, 0))
                    .Subscribe(_ => Debug.Log("爆発処理が成功しました。"))
                    .AddTo(gameObject);
            }
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
