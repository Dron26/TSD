using System;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture
{
    public class MainSwithMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _employeeLogin;
        [SerializeField] private Button _getEquipmentButton;
        [SerializeField] private Button _getTrolleyButton;
        [SerializeField] private Button _backButton;

        public Action OnSelectEquipment;
        public Action OnSelectTrolley;
        public Action OnBackButtonCLick;

        private SaveLoadService _saveLoadService;
        private WarningPanel _warningPanel;
        private bool _isSelectedEquipment;
        private bool _isSelectedTrolley;
        private string _textSelectEquipment = "-> Сканер";
        private string _textSelectTrolley = "-> Рохля";

        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _warningPanel = warningPanel;
            AddListeners();
        }


        private void SetButtonState()
        {
            _getEquipmentButton.gameObject.SetActive(_isSelectedEquipment);

            _getTrolleyButton.gameObject.SetActive(_isSelectedTrolley);
        }
        public void Work()
        {
            SwithState(false);
        }
        
        public void Reset()
        {
            
        }

        public void HandleGetEquipmentButton()
        {
            SentLogMessage(_saveLoadService.Employee.Login + _textSelectEquipment);
            OnSelectEquipment.Invoke();
        }
    
        public void HandleReturnEquipmentButton()
        {
            SentLogMessage(_saveLoadService.Employee.Login + _textSelectTrolley);
            OnSelectTrolley.Invoke();
        }

        public void SwithState(bool state)
        {
            _panel.gameObject.SetActive(state);

            if (state)
            {
                _employeeLogin.text = "Привет !  " + _saveLoadService.Employee.Login;
                _isSelectedEquipment= _saveLoadService.GetSwithStatus().IsEquipmentSelected;
                _isSelectedTrolley=_saveLoadService.GetSwithStatus().IsTrolleySelected;
                SetButtonState();
            }
            else
            {
                _employeeLogin.text = "";
            }
        }
        private void AddListeners()
        {
            _getEquipmentButton.onClick.AddListener(HandleGetEquipmentButton);
            _getTrolleyButton.onClick.AddListener(HandleReturnEquipmentButton);
            _backButton.onClick.AddListener(OnCLickBackButton);
        }

        public void OnCLickBackButton()
        {
            SentLogMessage("Закрытие панели Выбора работы");
            OnBackButtonCLick.Invoke();
        }

        private void RemuveListeners()
        {
            _getEquipmentButton.onClick.RemoveListener(HandleGetEquipmentButton);
            _getTrolleyButton.onClick.RemoveListener(HandleReturnEquipmentButton);
            _backButton.onClick.RemoveListener(OnCLickBackButton);
        }

        private void SentLogMessage(string message)
        {
            _saveLoadService.SentLogInfo(message, "");
        }

        private void OnDisable()
        {
            RemuveListeners();
        }
        
    }
}