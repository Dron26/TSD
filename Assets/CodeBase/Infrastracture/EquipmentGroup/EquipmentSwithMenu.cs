using System;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using CodeBase.Infrastracture.TrolleyGroup;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.EquipmentGroup
{
    public class EquipmentSwithMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _getEquipmentButton;
        [SerializeField] private Button _returnEquipmentButton;
        [SerializeField] private Button _backButton;

        public Action OnGetEquipment;
        public Action OnReturnEquipment;
        public Action OnBackButtonCLick;

        private SaveLoadService _saveLoadService;
        private WarningPanel _warningPanel;

        private string _textGetEquipment = " : пытается получить сканер";
        private string _textReturnEquipment = " : пытается вернуть сканер";
        private bool _haveFreeEquipment;

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

        public void HandleGetEquipmentButton()
        {
            Employee employee = _saveLoadService.Employee;

            if (employee.HaveEquipment)
            {
                _warningPanel.ShowWindow(WindowNames.CanNotTakeEquipment.ToString());
            }
            else
            {
                SentLogMessage(_saveLoadService.Employee.Login + _textGetEquipment);
                OnGetEquipment.Invoke();
            }
        }

        public void HandleReturnEquipmentButton()
        {
            Employee employee = _saveLoadService.Employee;

            if (!employee.HaveEquipment)
            {
                _warningPanel.ShowWindow(WindowNames.CanNotTakeEquipment.ToString());
            }
            else
            {
                SentLogMessage(_saveLoadService.Employee.Login + _textReturnEquipment);
                OnReturnEquipment.Invoke();
            }
        }

        private bool HaveFreeEquipment()
        {
            List<Box> boxes = _saveLoadService.GetBoxes();
            _haveFreeEquipment = false;
            foreach (Box box in boxes)
            {
                if (!box.Busy)
                {
                    _haveFreeEquipment = true;
                    break;
                }
            }

            return _haveFreeEquipment;
        }

        public void SwithState(bool state)
        {
            _panel.gameObject.SetActive(state);

            if (state)
            {
                if (_saveLoadService.GetCurrentEmployee().HaveEquipment)
                {
                    if (_saveLoadService.GetCurrentEmployee().GetDateTakenEquipment().Day != DateTime.Now.Day)
                    {
                        _warningPanel.ShowWindow(WindowNames.NotReturnYesterday.ToString());
                        SentDataMessage(new SentData(" Просроченный возврат оборудования", _saveLoadService.GetCurrentEmployee().Login, _saveLoadService.GetCurrentEmployee().Password,
                            _saveLoadService.GetCurrentEmployee().Box.Key,
                            _saveLoadService.GetCurrentEmployee().Box.Equipment.SerialNumber[^4..], DateTime.Now.ToString(), "Block"));
                    }
                    
                    _returnEquipmentButton.gameObject.SetActive(true);
                    _getEquipmentButton.gameObject.SetActive(false);
                }
                else
                {
                    if (HaveFreeEquipment())
                    {
                        _returnEquipmentButton.gameObject.SetActive(false);
                        _getEquipmentButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        _warningPanel.ShowWindow(WindowNames.NoScannersAvailable.ToString());
                        _getEquipmentButton.gameObject.SetActive(false);
                    }
                }
            }
        }


        private void AddListeners()
        {
            _getEquipmentButton.onClick.AddListener(HandleGetEquipmentButton);
            _returnEquipmentButton.onClick.AddListener(HandleReturnEquipmentButton);
            _backButton.onClick.AddListener(OnCLickBackButton);
        }

        public void OnCLickBackButton()
        {
            SentLogMessage("Закрытие панели Выбора действий оборудования");
            OnBackButtonCLick.Invoke();
        }

        private void RemuveListeners()
        {
            _getEquipmentButton.onClick.RemoveListener(HandleGetEquipmentButton);
            _returnEquipmentButton.onClick.RemoveListener(HandleReturnEquipmentButton);
            _backButton.onClick.RemoveListener(OnCLickBackButton);
        }

        private void SentLogMessage(string message)
        {
            _saveLoadService.SentLogInfo(message, "");
        }
        
        private void SentDataMessage(SentData message)
        {
            _saveLoadService.SentDataInfo(message);
        }

        private void OnDisable()
        {
            RemuveListeners();
        }
    }
}