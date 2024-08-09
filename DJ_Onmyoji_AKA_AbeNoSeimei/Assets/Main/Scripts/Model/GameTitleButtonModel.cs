using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Main.Model
{
    /// <summary>
    /// ���f��
    /// �X�e�[�W�I���֖߂�{�^��
    /// </summary>
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(EventTrigger))]

    public class GameTitleButtonModel : UIEventController, IButtonEventTriggerModel
    {
        /// <summary>�{�^��</summary>
        private Button _button;
        /// <summary>�C�x���g�g���K�[</summary>
        private EventTrigger _eventTrigger;
        /// <summary>�g�����X�t�H�[��</summary>
        private Transform _transform;
        /// <summary>�g�����X�t�H�[��</summary>
        public Transform Transform => _transform != null ? _transform : _transform = transform;

        public bool SetButtonEnabled(bool enabled)
        {
            return _mainUGUIsModelUtility.SetButtonEnabledOfButton(enabled, _button, Transform);
        }

        public bool SetEventTriggerEnabled(bool enabled)
        {
            return _mainUGUIsModelUtility.SetEventTriggerEnabledOfEventTrigger(enabled, _eventTrigger, Transform);
        }
    }

}

