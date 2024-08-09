using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.View
{
    /// <summary>
    /// �r���[
    /// �Q�[���I�[�o�[���
    /// </summary>
    public class GameOverView : MonoBehaviour, IGameOverView
    {
        private void OnEnable()
        {
            Time.timeScale = 0f;
            Debug.LogWarning($"Time.timeScale:[{Time.timeScale}]");
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
            Debug.LogWarning($"Time.timeScale:[{Time.timeScale}]");
        }

        public bool SetActiveGameObject(bool active)
        {
            try
            {
                gameObject.SetActive(active);

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
    /// �r���[
    /// �Q�[���I�[�o�[���
    /// �C���^�[�t�F�[�X
    /// </summary>
    public interface IGameOverView
    {
        /// <summary>
        /// �Q�[���I�u�W�F�N�g�̗L���^�������Z�b�g
        /// </summary>
        /// <param name="active">�L���^�������</param>
        /// <returns>�����^���s</returns>
        public bool SetActiveGameObject(bool active);
    }

}
