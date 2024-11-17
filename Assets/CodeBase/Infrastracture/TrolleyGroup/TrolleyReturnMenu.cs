using System;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.TrolleyGroup
{
    public class TrolleyReturnMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _buttonApply;
        [SerializeField] private Button _resetInput;
        [SerializeField] private Button _backButton;
        [SerializeField] private Image _CheckUp;
        [SerializeField] private Image _CheckDown;
        [SerializeField] private TMP_Text _employeeField;
        [SerializeField] private TMP_Text _trolleyNumber;
        [SerializeField] private TMP_InputField _inputReturnField;

        public Action OnBackButtonCLick;

        private SaveLoadService _saveLoadService;
        private Employee _employee;
        private WarningPanel _warningPanel;
        private bool _isReseted;
        private bool _isSerialNumberInputed;

        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _warningPanel = warningPanel;
            AddListeners();
        }

        public void Work()
        {
            _employee = _saveLoadService.Employee;
            _inputReturnField.Select();
            _inputReturnField.ActivateInputField();
            _inputReturnField.interactable = true;
            _buttonApply.interactable = false;
            _CheckDown.enabled = true;
            _CheckUp.enabled = false;
        }

        private void OnApplyButtonClick()
        {
            SentDataMessage(new SentData(" Возврат рохли ", _employee.Login, "", "","", DateTime.Now.ToString(), _employee.Trolley.Number));
            SentLogMessage(
                "Возврат рохли" + DateTime.Now.ToString(),_employee.Trolley.Number );
            
            SentData sentData = new SentData(" Возврат рохли ", _employee.Login, _employee.Password, "", "", "", "");
            sentData.SetTrolleyNumber(_employee.Trolley.Number);
            SentDataTrolleyMessage(sentData);
            
            _saveLoadService.SetTrolley(_employee.GetTrolley());
            _saveLoadService.SetCurrentEmployee(_employee);
            _saveLoadService.SaveDatabase();
            OnCLickBackButton();
        }

        public void SetData()
        {
            _employee = _saveLoadService.GetCurrentEmployee();
            _employeeField.text = _employee.Login;
            _trolleyNumber.text = _employee.Trolley.Number;
        }

        public void Reset()
        {
            _isReseted = true;
            _employeeField.text = "";
            _trolleyNumber.text = "";
            _inputReturnField.text = null;
            _inputReturnField.interactable = true;
            _inputReturnField.ActivateInputField();
            _inputReturnField.Select();
            _buttonApply.interactable = false;
            _CheckDown.enabled = true;
            _CheckUp.enabled = false;
            _resetInput.interactable = false;
            _isSerialNumberInputed = false;
            _isReseted = false;
        }

        private void ResetInput()
        {
            SentLogMessage(_employee.Login + "Выполнил сброс ввода QR тележки", "");
            Reset();
            SetData();
        }

        public void ValidateReturn()
        {
            if (_isReseted == false)
            {
                CheckInput();
            }
        }
        
        private void CheckInput()
        {
            string text = _inputReturnField.text;

            if (_inputReturnField.text != "")
            {
                _resetInput.interactable = true;
            }
            else
            {
                _resetInput.interactable = false;
            }

            if (text == _employee.Trolley.Number && _isSerialNumberInputed == false)
            {
                SentLogMessage(_employee.Login + ": отсканировал верный QR", "");

                _isSerialNumberInputed = true;
                _inputReturnField.interactable = false;
                _buttonApply.interactable = true;
                _CheckDown.enabled = false;
                _CheckUp.enabled = true;
            }

            if (_isSerialNumberInputed == false)
            {
                List<Employee> employees = _saveLoadService.GetEmployees();

                try
                {
                    foreach (var employee in employees)
                    {
                        if (employee != _employee && _inputReturnField.text == employee.Trolley.Number)
                        {
                            _warningPanel.ShowWindow(WindowNames.CanNotReturnOtherEquipment.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                SentLogMessage(_employee.Login + ": отсканировал неверный QR", "");
            }
        }

        public void SwitchPanelState(bool state)
        {
            if (state)
            {
                SetData();
            }
            else
            {
                Reset();
            }

            _panel.SetActive(state);
        }

        private void AddListeners()
        {
            _buttonApply.onClick.AddListener(OnApplyButtonClick);
            _inputReturnField.onValueChanged.AddListener(delegate { ValidateReturn(); });
            _resetInput.onClick.AddListener(ResetInput);
            _backButton.onClick.AddListener(OnCLickBackButton);
        }

        private void RemuveListeners()
        {
            _inputReturnField.onValueChanged.RemoveListener(delegate { ValidateReturn(); });
            _buttonApply.onClick.RemoveListener(OnApplyButtonClick);
            _resetInput.onClick.RemoveListener(ResetInput);
            _backButton.onClick.RemoveListener(OnCLickBackButton);
        }


        private void SentLogMessage(string message, string comment)
        {
            _saveLoadService.SentLogInfo(message, comment);
        }

        private void SentDataMessage(SentData message)
        {
            _saveLoadService.SentDataInfo(message);
        }

        private void SentDataTrolleyMessage(SentData message)
        {
            _saveLoadService.SentDataTrolleyMessage(message);
        }
        
        private void OnDisable()
        {
            RemuveListeners();
        }

        public void OnCLickBackButton()
        {
            OnBackButtonCLick.Invoke();
        }
    }
}