using System.Collections;
using System.Collections.Generic;
using Main.Model;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Main.Audio;
using Main.Common;
using Main.View;
using System.Linq;

namespace Main.Presenter
{
    /// <summary>
    /// 五芒星デモ用
    /// プレゼンタ
    /// </summary>
    public class MainPresenterPentagramDemo : MonoBehaviour
    {
        /// <summary>ペンダグラムシステムのモデル</summary>
        [SerializeField] private PentagramSystemModel pentagramSystemModel;
        /// <summary>ペンダグラムターンテーブルのビュー</summary>
        [SerializeField] private PentagramTurnTableView pentagramTurnTableView;
        /// <summary>式神スキル管理システムのモデル</summary>
        [SerializeField] private ShikigamiSkillSystemModel shikigamiSkillSystemModel;
        /// <summary>ペンダグラムターンテーブルのモデル</summary>
        [SerializeField] private PentagramTurnTableModel pentagramTurnTableModel;
        /// <summary>式神レベル管理のビュー（蝋燭リソースの情報に合わせてUIを変化させるビューコンポーネントを再利用）</summary>
        [SerializeField] private CandleUniversalGaugeView[] candleUniversalGaugeViews;
        /// <summary>蝋燭リソースの情報に合わせてUIを変化させるビュー</summary>
        [SerializeField] private SpGaugeView spGaugeView;

        private void Reset()
        {
            pentagramSystemModel = GameObject.Find("PentagramSystem").GetComponent<PentagramSystemModel>();
            pentagramTurnTableView = GameObject.Find("PentagramTurnTable").GetComponent<PentagramTurnTableView>();
            shikigamiSkillSystemModel = GameObject.Find("ShikigamiSkillSystem").GetComponent<ShikigamiSkillSystemModel>();
            pentagramTurnTableModel = GameObject.Find("PentagramTurnTable").GetComponent<PentagramTurnTableModel>();
            candleUniversalGaugeViews = new CandleUniversalGaugeView[]
            {
                GameObject.Find($"Candle{ShikigamiType.Wrap}Gauge").GetComponent<CandleUniversalGaugeView>(),
                GameObject.Find($"Candle{ShikigamiType.Dance}Gauge").GetComponent<CandleUniversalGaugeView>(),
                GameObject.Find($"Candle{ShikigamiType.Graffiti}Gauge").GetComponent<CandleUniversalGaugeView>(),
            };
            spGaugeView = GameObject.Find("SpGauges").GetComponent<SpGaugeView>();
        }

        private void Start()
        {
            this.UpdateAsObservable()
                .Select(_ => pentagramTurnTableModel.WrapTurretModel.OnmyoBulletModelStates)
                .Where(q => q != null &&
                    0 < q.Length)
                .Take(1)
                .Subscribe(x =>
                {
                    foreach (var onmyoBulletModelStates in x.Select((p, i) => new { Content = p, Index = i }))
                        onmyoBulletModelStates.Content.IsExplosion.ObserveEveryValueChanged(x => x.Value)
                            .Where(x => x)
                            .Subscribe(x =>
                            {
                                Observable.FromCoroutine<bool>(observer => pentagramTurnTableView.PlayDirectionExplosionOfWrapTurretView(observer, onmyoBulletModelStates.Index))
                                    .Subscribe(_ => {})
                                    .AddTo(gameObject);
                            });
                });
            BgmConfDetails bgmConfDetails = new BgmConfDetails();
            this.UpdateAsObservable()
                // .Where(_ => pentagramSystemModel.InputSlipLoopState.IsLooping != null &&
                // !pentagramSystemModel.InputSlipLoopState.IsLooping.Value)
                .Select(_ => pentagramSystemModel.InputValue)
                .Subscribe(x =>
                {
                    bgmConfDetails.InputValue = x.Value;
                    if (!pentagramTurnTableView.MoveSpin(bgmConfDetails))
                        Debug.LogError("MoveSpin");
                });
            pentagramSystemModel.JockeyCommandType.ObserveEveryValueChanged(x => x.Value)
                .Pairwise()
                .Subscribe(pair =>
                {
                    if (!shikigamiSkillSystemModel.UpdateCandleResource((JockeyCommandType)pair.Current, (JockeyCommandType)pair.Previous))
                        Debug.LogError("UpdateCandleResource");
                    if (!pentagramTurnTableModel.BuffAllTurrets((JockeyCommandType)pair.Current))
                        Debug.LogError("BuffAllTurrets");
                    if (!shikigamiSkillSystemModel.ForceZeroAndRapidRecoveryCandleResource((JockeyCommandType)pair.Current))
                        Debug.LogError("ForceZeroAndRapidRecoveryCandleResource");
                    Observable.FromCoroutine<bool>(observer => pentagramTurnTableView.PlayDirectionBackSpin(observer, (JockeyCommandType)pair.Current))
                        .Subscribe(x =>
                        {
                            if (!pentagramSystemModel.ResetJockeyCommandType())
                                Debug.LogError("ResetJockeyCommandType");
                        })
                        .AddTo(gameObject);
                    if (!pentagramSystemModel.SetIsLooping((JockeyCommandType)pair.Current))
                        Debug.LogError("SetIsLooping");
                    if (!shikigamiSkillSystemModel.SetIsStopRecovery((JockeyCommandType)pair.Current))
                        Debug.LogError("SetIsStopRecovery");
                });
            this.UpdateAsObservable()
                .Select(_ => pentagramSystemModel.InputSlipLoopState.IsLooping)
                .Where(x => x != null)
                .Take(1)
                .Subscribe(x =>
                {
                    x.ObserveEveryValueChanged(x => x.Value)
                        .Subscribe(x =>
                        {
                            if (!x)
                            {
                                if (!pentagramTurnTableView.ResetFromAngle())
                                    Debug.LogError("ResetFromAngle");
                            }
                            if (!pentagramTurnTableModel.SetActionRateNormalOfOnmyoTurret(x))
                                Debug.LogError("SetActionRateNormalOfOnmyoTurret");
                        });
                });
            this.UpdateAsObservable()
                .Select(_ => pentagramSystemModel.InputSlipLoopState.ActionTrigger)
                // .Select(_ => pentagramSystemModel.InputSlipLoopState.IsLooping)
                .Where(x => x != null)
                .Take(1)
                .Subscribe(x =>
                {
                    x.ObserveEveryValueChanged(x => x.Value)
                        .Subscribe(x =>
                        {
                            if (x)
                            {
                                if (!pentagramTurnTableModel.SetMoveDirectionsToDanceOfOnmyoWrapGraffitiTurret())
                                    Debug.LogError("SetMoveDirectionsToDanceOfOnmyoWrapGraffitiTurret");
                                Observable.FromCoroutine<bool>(observer => pentagramTurnTableView.MoveSpin(observer, pentagramSystemModel.InputSlipLoopState))
                                    .Subscribe(x =>
                                    {
                                        if (x)
                                        {
                                            if (!pentagramTurnTableModel.AttackOfOnmyoTurret())
                                                Debug.LogError("AttackOfOnmyoTurret");
                                            if (!shikigamiSkillSystemModel.UpdateCandleResourceOfAttackOnmyoTurret())
                                                Debug.LogError("UpdateCandleResourceOfAttackOnmyoTurret");
                                            if (!pentagramTurnTableModel.SetMoveDirectionsDefaultOfOnmyoWrapGraffitiTurret())
                                                Debug.LogError("SetMoveDirectionsDefaultOfOnmyoWrapGraffitiTurret");
                                        }
                                    })
                                    .AddTo(gameObject);
                            }
                        });
                });
            this.UpdateAsObservable()
                .Select(_ => shikigamiSkillSystemModel.ShikigamiInfos)
                .Where(x => x != null)
                .Take(1)
                .Subscribe(x =>
                {
                    foreach (var item in x.Select((p, i) => new { Content = p, Index = i }))
                        item.Content.state.tempoLevel.ObserveEveryValueChanged(x => x.Value)
                            .Subscribe(x =>
                            {
                                foreach (var candleUniversalGaugeView in candleUniversalGaugeViews)
                                {
                                    if (!candleUniversalGaugeView.SetSliderValue(x, item.Content.prop.type))
                                        Debug.LogError("SetSliderValue");
                                }
                                if (!pentagramTurnTableModel.UpdateTempoLvValues(x, item.Content.prop.type))
                                    Debug.LogError("UpdateTempoLvValues");
                            });

                });
            this.UpdateAsObservable()
                .Select(_ => shikigamiSkillSystemModel.CandleInfo.CandleResource)
                .Where(x => x != null)
                .Take(1)
                .Subscribe(x =>
                {
                    x.ObserveEveryValueChanged(x => x.Value)
                        .Subscribe(x =>
                        {
                            if (!spGaugeView.SetAnchor(x, shikigamiSkillSystemModel.CandleInfo.LimitCandleResorceMax))
                                Debug.LogError("SetAnchor");
                        });
                });
            this.UpdateAsObservable()
                .Select(_ => shikigamiSkillSystemModel.CandleInfo.IsOutCost)
                .Where(x => x != null)
                .Take(1)
                .Subscribe(x =>
                {
                    x.ObserveEveryValueChanged(x => x.Value)
                        .Subscribe(x =>
                        {
                            // TODO:SPゲージの急速回復が始まるの他にもし演出が必要ならここで処理を実行する
                            if (!pentagramSystemModel.SetIsLooping(JockeyCommandType.None))
                                Debug.LogError("SetIsLooping");
                            if (!shikigamiSkillSystemModel.SetIsStopRecovery(JockeyCommandType.None))
                                Debug.LogError("SetIsStopRecovery");
                        });
                });
        }
    }
}
