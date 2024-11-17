using System;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.TrolleyGroup
{
    public class TrolleySwithMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _getTrolleyButton;
        [SerializeField] private Button _returnTrolleyButton;
        [SerializeField] private Button _backButton;

        public Action OnGetTrolley;
        public Action OnReturnTrolley;
        public Action OnBackButtonCLick;

        private SaveLoadService _saveLoadService;
        private WarningPanel _warningPanel;

        private string _textGetEquipment = " : пытается получить рохлю";
        private string _textReturnEquipment = " : пытается вернуть рохлю";

        private bool _haveFreeTrolley;
        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _warningPanel = warningPanel;
            AddListeners();
        }

        public void Work()
        {
            SwithState(false);
        }

        private bool HaveFreeTrolley()
        {
            List< Trolley> trolleys = _saveLoadService.GetTrolleys();
            _haveFreeTrolley = false;
            foreach (Trolley trolley in trolleys)
            {
                if (!trolley.Busy)
                {
                    _haveFreeTrolley = true;
                    break;
                }
            }

            return _haveFreeTrolley;
        }
        
        public void HandleGetEquipmentButton()
        {
            Employee employee = _saveLoadService.Employee;

            if (employee.HaveTrolley)
            {
                _warningPanel.ShowWindow(WindowNames.CanNotTakeEquipment.ToString());
            }
            else
            {
                SentLogMessage(_saveLoadService.Employee.Login + _textGetEquipment);
                OnGetTrolley.Invoke();
            }
        }

        public void HandleReturnEquipmentButton()
        {
            Employee employee = _saveLoadService.Employee;

            if (!employee.HaveTrolley)
            {
                _warningPanel.ShowWindow(WindowNames.CanNotTakeEquipment.ToString());
            }
            else
            {
                SentLogMessage(_saveLoadService.Employee.Login + _textReturnEquipment);
                OnReturnTrolley.Invoke();
            }
        }

        public void SwithState(bool state)
        {
            _panel.gameObject.SetActive(state);

            if (state)
            {
                if (HaveFreeTrolley())
                {
                    if (_saveLoadService.GetCurrentEmployee().HaveTrolley)
                    {
                        _returnTrolleyButton.gameObject.SetActive(true);
                        _getTrolleyButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        _returnTrolleyButton.gameObject.SetActive(false);
                        _getTrolleyButton.gameObject.SetActive(true);
                    }
                }
                else
                {
                    _warningPanel.ShowWindow(WindowNames.NoTrolleyAvailable.ToString());
                    _getTrolleyButton.gameObject.SetActive(false);
                }

               
            }
        }
        private void AddListeners()
        {
            _getTrolleyButton.onClick.AddListener(HandleGetEquipmentButton);
            _returnTrolleyButton.onClick.AddListener(HandleReturnEquipmentButton);
            _backButton.onClick.AddListener(OnCLickBackButton);
        }

        public void OnCLickBackButton()
        {
            SentLogMessage("Закрытие панели Выбора действий рохли");
            OnBackButtonCLick.Invoke();
        }

        private void RemuveListeners()
        {
            _getTrolleyButton.onClick.RemoveListener(HandleGetEquipmentButton);
            _returnTrolleyButton.onClick.RemoveListener(HandleReturnEquipmentButton);
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