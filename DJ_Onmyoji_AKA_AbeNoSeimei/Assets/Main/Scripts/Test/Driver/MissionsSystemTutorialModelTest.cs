// ChatGPT 4o
using System.Linq;
using UnityEngine;
using UniRx;
using Main.Common;
using Main.Model;
using UniRx.Triggers;

namespace Main.Test.Driver
{
    public class MissionsSystemTutorialModelTest : MonoBehaviour
    {
        [SerializeField] private MissionsSystemTutorialModel missionsSystemTutorialModel;
        [SerializeField] private GuideMessageID testGuideMessageID;

        private void Reset()
        {
            missionsSystemTutorialModel = GameObject.FindObjectOfType<MissionsSystemTutorialModel>();
        }

        private void Start()
        {
            if (missionsSystemTutorialModel == null)
            {
                Debug.LogError("MissionsSystemTutorialModel�����蓖�Ă��Ă��܂���I");
                return;
            }

            System.IDisposable modelUpdObservable = this.UpdateAsObservable().Subscribe(_ => { });
            modelUpdObservable.Dispose();
            // �i�s���̃~�b�V����ID�̍X�V���Ď����ăf�o�b�O�o��
            missionsSystemTutorialModel.CallMissionID
                .ObserveEveryValueChanged(x => x.Value)
                .Subscribe(missionID =>
                {
                    modelUpdObservable.Dispose();

                    Debug.Log($"�i�s���̃~�b�V����ID���X�V����܂���: {missionID}");

                    switch (missionID)
                    {
                        case MissionID.MI0000:
                            break;
                        case MissionID.MI0001:
                            modelUpdObservable = this.UpdateAsObservable()
                                .Select(x => missionsSystemTutorialModel.CurrentMissionsSystemTutorialStruct)
                                .Where(currentStruct => currentStruct.killedEnemyCount != null &&
                                    currentStruct.isCompleted != null)
                                .Subscribe(currentStruct =>
                                {
                                    // ���݂̃~�b�V���������f�o�b�O�o��
                                    var currentMission = currentStruct;
                                    Debug.Log($"���ݎ��s���̃~�b�V�������: ID={currentMission.missionID}, ���j��={currentMission.killedEnemyCount.Value}/{currentMission.killedEnemyCountMax}, ����={currentMission.isCompleted.Value}");
                                })
                                .AddTo(missionsSystemTutorialModel);

                            break;
                        case MissionID.MI0002:
                            modelUpdObservable = this.UpdateAsObservable()
                                .Select(x => missionsSystemTutorialModel.CurrentMissionsSystemTutorialStruct)
                                .Where(currentStruct => currentStruct.killedEnemyCount != null &&
                                    currentStruct.isCompleted != null)
                                .Subscribe(currentStruct =>
                                {
                                    // ���݂̃~�b�V���������f�o�b�O�o��
                                    var currentMission = missionsSystemTutorialModel.CurrentMissionsSystemTutorialStruct;
                                    Debug.Log($"���ݎ��s���̃~�b�V�������: ID={currentMission.missionID}, ���j��={currentMission.killedEnemyCount.Value}/{currentMission.killedEnemyCountMax}, ����={currentMission.isCompleted.Value}");
                                })
                                .AddTo(missionsSystemTutorialModel);

                            break;
                        default:
                            break;
                    }
                })
                .AddTo(this);
        }

        private void OnGUI()
        {
            // SetCallMissionID�{�^��
            if (GUI.Button(new Rect(10, 10, 200, 50), "Set Call Mission ID"))
            {
                if (missionsSystemTutorialModel.SetCallMissionID(testGuideMessageID))
                {
                    Debug.Log($"SetCallMissionID���������܂���: {testGuideMessageID}");
                }
                else
                {
                    Debug.LogError("SetCallMissionID�����s���܂���");
                }
            }

            // UpdateKilledEnemyCount�{�^��
            if (GUI.Button(new Rect(10, 70, 200, 50), "Update Killed Enemy Count"))
            {
                if (missionsSystemTutorialModel.UpdateKilledEnemyCount())
                {
                    Debug.Log("UpdateKilledEnemyCount���������܂���");
                }
                else
                {
                    Debug.LogError("UpdateKilledEnemyCount�����s���܂���");
                }
            }
        }
    }
}
