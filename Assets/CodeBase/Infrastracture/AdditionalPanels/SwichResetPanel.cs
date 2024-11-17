using System;
using CodeBase.Infrastracture.Datas;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.AdditionalPanels
{
    public class SwichResetPanel : MonoBehaviour, IWindow
    {
        
        public event  Action OnBackButtonClick;
        public event   Action OnSelectPass;
        public event   Action OnSelectEquipment;
        public event   Action OnSelectTrolley;
        [SerializeField] private ResetPanel _resetPanel;
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _backButton;
        
        [SerializeField] private Button _resetEquipmentButton;
        [SerializeField] private Button _resetEmployeePassButton;
        [SerializeField] private Button _resetTrolleyButton;
        
        private SaveLoadService _saveLoadService;
        private bool _isTrolleyActive;


        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _isTrolleyActive = _saveLoadService.IsTrolleActive;
            _resetPanel.Init(_saveLoadService, warningPanel,this);
            _resetPanel.SwithState(false);
            AddListeners();
            Reset();
        }

        public void Reset()
        {
            _resetTrolleyButton.gameObject.SetActive(_isTrolleyActive);
            
            _resetEquipmentButton.gameObject.SetActive(!_isTrolleyActive);
            _resetEmployeePassButton.gameObject.SetActive(!_isTrolleyActive);
        }
        
        public void SwithState(bool state)
        {
            _panel.SetActive(state);
        }
        private void SentLogMessage(string message, string comment)
        {
            _saveLoadService.SentLogInfo(message, comment);
        }
        
        private void AddListeners()
        {
            _resetEquipmentButton.onClick.AddListener(()=>SendAction(OnSelectEquipment));
            _resetTrolleyButton.onClick.AddListener(()=>SendAction(OnSelectTrolley));
            _resetEmployeePassButton.onClick.AddListener(()=>SendAction(OnSelectPass));
            _resetPanel.OnBackButtonClick += OnExitChildrenPanel;
            _backButton.onClick.AddListener(OnCLickBackButton);
            _saveLoadService.OnSelectTrolley += OnSelectedTrolley;
        }
        
        private void OnExitChildrenPanel()
        {
            SwithState(true);
        }

        private void OnSelectedTrolley()
        {
            _isTrolleyActive=!_isTrolleyActive;
        }

        private void SendAction(Action action)
        {
            _resetPanel.SwithState(true);
            action?.Invoke();
            SwithState(false);
        }
        private void RemuveListeners()
        {
            _resetEquipmentButton.onClick.RemoveListener(()=>SendAction(OnSelectEquipment));
            _resetEmployeePassButton.onClick.RemoveListener(()=>SendAction(OnSelectPass));
            _resetTrolleyButton.onClick.AddListener(()=>SendAction(OnSelectTrolley));
            _resetPanel.OnBackButtonClick = OnExitChildrenPanel;
            _backButton.onClick.RemoveListener(OnCLickBackButton);
            _saveLoadService.OnSelectTrolley -= OnSelectedTrolley;
        }
        
        public void OnCLickBackButton()
        {
            SentLogMessage("<- Назад ", "");

            OnBackButtonClick.Invoke();
            SwithState(false);
        }

        private void OnDestroy()
        {
            RemuveListeners();
        }
    }
}