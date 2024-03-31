using System.Collections;
using System.Collections.Generic;
using Main.Audio;
using Main.Common;
using UnityEngine;

namespace Main.View
{
    /// <summary>
    /// ※デッドロジック※
    /// ペンダグラムターンテーブル
    /// プレゼンタから伝達された入力を元に出力を行う
    /// Imageコンポーネントへ入力操作を行う
    /// ビュー
    /// </summary>
    public class PentagramTurnTableOnModalView : PentagramTurnTableCommonView, IPentagramTurnTableView
    {
        public bool CalibrationToTarget(Transform transform)
        {
            throw new System.NotImplementedException();
        }

        public bool MoveSpin(BgmConfDetails bgmConfDetails)
        {
            throw new System.NotImplementedException();
        }

        public bool MoveSpin(InputSlipLoopState inputSlipLoopState)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator MoveSpin(System.IObserver<bool> observer, InputSlipLoopState inputSlipLoopState)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator PlayDirectionBackSpin(System.IObserver<bool> observer, JockeyCommandType jockeyCommandType)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator PlayDirectionExplosionOfWrapTurretView(System.IObserver<bool> observer, int index)
        {
            throw new System.NotImplementedException();
        }

        public bool ResetFromAngle()
        {
            throw new System.NotImplementedException();
        }

        public bool SetSpriteIndex(float timeSec, float limitTimeSecMax)
        {
            throw new System.NotImplementedException();
        }
    }
}
