using System;
using System.Collections;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.EquipmentGroup
{
    public class EquipmentReturnMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private GameObject _equipmentPanel;
        [SerializeField] private GameObject _printerPanel;
        [SerializeField] private GameObject _demoPrinter;
        [SerializeField] private Button _buttonApply;
        [SerializeField] private TMP_Text _employeeField;
        [SerializeField] private TMP_Text _keyField;
        [SerializeField] private TMP_Text _tsdField;
        [SerializeField] private TMP_Text _printerField;
        [SerializeField] private TMP_InputField _inputReturnField;
        [SerializeField] private TMP_InputField _inputReturnPrinterField;
        [SerializeField] private Button _resetInput;
        [SerializeField] private Image _CheckUp;
        [SerializeField] private Image _CheckDown;
        [SerializeField] private Image _CheckUpPrinter;
        [SerializeField] private Image _CheckDownPrinter;
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_Text _inputHideField;
        [SerializeField] private TMP_Text _inputPrinterHideField;
        public Action OnBackButtonCLick;
        
        private char _simbol = '*';
        private SaveLoadService _saveLoadService;
        private Employee _employee;
        private WarningPanel _warningPanel;
        private bool _isReseted;
        private bool _isSerialNumberInputed;
        private bool _isSerialNumberPrinterInputed;
        private bool _mustReturnPrinter;
        
        
        private Data _data;
        private bool  _isOpenBox=false;
        private bool _isActive;
        
        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _warningPanel = warningPanel;
            AddListeners();
        }

        public void Work()
        {
            _employee = _saveLoadService.Employee;
            _isActive = true;
            _inputReturnField.Select();
            _inputReturnField.ActivateInputField();
            _inputReturnField.interactable = true;
            _inputReturnPrinterField.interactable = false;
            _buttonApply.interactable = false;
            _CheckDown.enabled = true;
            _CheckUp.enabled = false;
            _equipmentPanel.SetActive(true);
            _printerPanel.SetActive(false);
            _demoPrinter.SetActive(false);
            _saveLoadService.OnArduinoAnswer+=ArduinoListen;
        }
        private void OnApplyButtonClick()
        {
            SentDataMessage(new SentData("Возврат оборудования ", _employee.Login, _employee.Password, _employee.Box.Key,
                _employee.Box.Equipment.SerialNumber[^4..], DateTime.Now.ToString(), ""));
            SentLogMessage("Возврат оборудования" +"//"+ _employee.Login +"//"+  _employee.Password +"//"+  _employee.Box.Key +"//"+ 
             _employee.Box.Equipment.SerialNumber[^4..] +"//"+  DateTime.Now.ToString(), " Возврат оборудования ");
            _saveLoadService.SetBox(_employee.GetBox());
            _saveLoadService.SetCurrentEmployee(_employee);
            _saveLoadService.SaveDatabase();
            OnCLickBackButton();
        }

        public void SetData()
        {
            _employee = _saveLoadService.GetCurrentEmployee();
            _employeeField.text = _employee.Login;
            _keyField.text = _employee.Box.Key;
            _tsdField.text = _employee.Equipment.SerialNumber[^4..];
            if (_employee.HavePrinter)
            {
                _printerField.text = _employee.Printer.SerialNumber[^4..];
            }
        }

        public void Reset()
        {
            _isReseted = true;
            _employeeField.text = "";
            _keyField.text = "";
            _tsdField.text = "";
            _printerField.text = "";
            _inputHideField.text = "";
            _inputPrinterHideField.text = "";
            _inputReturnField.text = null;
            _inputReturnField.interactable = true;
            _inputReturnField.ActivateInputField();
            _inputReturnField.Select();
            _inputReturnPrinterField.text = "";
            _inputReturnPrinterField.interactable = false;
            _buttonApply.interactable = false;
            _CheckDown.enabled = true;
            _CheckUp.enabled = false;
            _resetInput.interactable = false;
            _isSerialNumberInputed = false;
            _isSerialNumberPrinterInputed = false;
            _equipmentPanel.SetActive(true);
            _printerPanel.SetActive(false);
            _demoPrinter.SetActive(false);
            _saveLoadService.OnArduinoAnswer-=ArduinoListen;
            _isReseted = false;
        }

        private void ResetInput()
        {
            SentLogMessage(_employee.Login + "Выполнил сброс ввода QR сканера", "");
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
        
        public void ValidateReturnPrinter()
        {
            if (_isReseted == false)
            {
                CheckInputPrinter();
            }
        }
        private IEnumerator Whait()
        {
            Debug.Log("Whait");
            
            _saveLoadService.SendCommandArduino("OPEN"+_employee.Box.Key);

            while (!_isOpenBox)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _isOpenBox = true;
                }
                yield return null;
            }
            Debug.Log("Opened1zzzz");


            OnApplyButtonClick();
        }
        private void ArduinoListen(string message)
        {
            if (_isActive)
            {
                string messag = message[..6];
                Debug.Log(message);
            
                if (messag == "Closed" )
                {
                    Debug.Log("ArduinoListen");
                    Debug.Log(message);

                    string key = _employee.Box.Key;
                
                    if (message == "Closed"+key)
                    {
                        _isOpenBox = true;
                        Debug.Log(message);
                    }
                }

            }
        }

        private void ShowReturnPrinterPanel()
        {
            _equipmentPanel.SetActive(false);
            _printerPanel.SetActive(true);
            _demoPrinter.SetActive(true);
            _CheckDownPrinter.enabled = true;
            _CheckUpPrinter.enabled = false;
            _inputReturnPrinterField.interactable = true;
            _inputReturnPrinterField.Select();
            _inputReturnPrinterField.ActivateInputField();
        }

        private void CheckAvailabilityPrinter()
        {
            _mustReturnPrinter = _employee.Printer!=null;
        }

        private void CheckInputPrinter()
        {
            string text = _inputReturnPrinterField.text;
            _inputPrinterHideField.text = new string(_simbol, _inputReturnPrinterField.text.Length);
            
            if (_inputReturnPrinterField.text != "")
            {
                _resetInput.interactable = true;
            }
            else
            {
                _resetInput.interactable = false;
            }

            if (text == _employee.Printer.SerialNumber && _isSerialNumberPrinterInputed == false)
            {
                SentLogMessage(_employee.Login + ": отсканировал верный QR Printer", "");

                _isSerialNumberPrinterInputed = true;
                _inputReturnPrinterField.interactable = false;
                _CheckDownPrinter.enabled = false;
                _CheckUpPrinter.enabled = true;
                WhaiteToOpenBox();
            }
        }
        public void WhaiteToOpenBox()
        {
            StartCoroutine(Whait());
        }
        private void CheckInput()
        {
            string text = _inputReturnField.text;
            _inputHideField.text = new string(_simbol, _inputReturnField.text.Length);
            
            if (_inputReturnField.text != "")
            {
                _resetInput.interactable = true;
            }
            else
            {
                _resetInput.interactable = false;
            }

            if (text == _employee.Equipment.SerialNumber[^4..] && _isSerialNumberInputed == false)
            {
                SentLogMessage(_employee.Login + ": отсканировал верный QR", "");

                _isSerialNumberInputed = true;
                CheckAvailabilityPrinter();
                if (!_mustReturnPrinter )
                {
                    _inputReturnField.interactable = false;
                    _buttonApply.interactable = true;
                    _CheckDown.enabled = false;
                    _CheckUp.enabled = true;
                    WhaiteToOpenBox();
                }
                else
                {
                    ShowReturnPrinterPanel();
                }
            }

            if (_isSerialNumberInputed == false)
            {
                List<Employee> employees = _saveLoadService.GetEmployees();
            
                try
                {
                    foreach (var employee in employees)
                    {
                        if (employee != _employee && _inputReturnField.text == employee.Equipment.SerialNumber[^4..]|| _inputReturnField.text == employee.Equipment.SerialNumber[^5..])
                        {
                            _warningPanel.ShowWindow(WindowNames.CanNotReturnOtherEquipment.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
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
                StopCoroutine(Whait());
                Reset();
                
            }

            _panel.SetActive(state);
        }

        private void AddListeners()
        {
           // _buttonApply.onClick.AddListener(OnApplyButtonClick);
            _inputReturnField.onValueChanged.AddListener(delegate { ValidateReturn(); });
            _inputReturnPrinterField.onValueChanged.AddListener(delegate { ValidateReturnPrinter(); });
            _resetInput.onClick.AddListener(ResetInput);
            _backButton.onClick.AddListener(OnCLickBackButton);
            
        }

        private void RemuveListeners()
        {
            _inputReturnField.onValueChanged.RemoveListener(delegate { ValidateReturn(); });
            _inputReturnPrinterField.onValueChanged.RemoveListener(delegate { ValidateReturnPrinter(); });
           // _buttonApply.onClick.RemoveListener(OnApplyButtonClick);
            _resetInput.onClick.RemoveListener(ResetInput);
            _backButton.onClick.RemoveListener(OnCLickBackButton);
            _saveLoadService.OnArduinoAnswer-=ArduinoListen;
        }
        
        private void SentLogMessage(string message, string comment)
        {
            _saveLoadService.SentLogInfo(message, comment);
        }

        private void SentDataMessage(SentData message)
        {
            _saveLoadService.SentDataInfo(message);
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