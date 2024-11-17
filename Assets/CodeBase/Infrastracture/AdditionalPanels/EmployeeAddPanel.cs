using System;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.AdditionalPanels
{
    public class EmployeeAddPanel : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private GameObject _employeerPanel;
        [SerializeField] private GameObject _employeeRepeatPanel;
        [SerializeField] private GameObject _employeesList;
        [SerializeField] private GameObject _employeeItem;
        [SerializeField] private GameObject _viweport;
        [SerializeField] private Button _addedButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _applyRepeatButton;
        [SerializeField] private Button _resetInputTextButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _okButton;
        [SerializeField] private ScrollRect _scroll;
        [SerializeField] private TMP_Text _textInfo;
        [SerializeField] private TMP_InputField _inputLoginField;
        [SerializeField] private TMP_InputField _inputPassField;
        [SerializeField] private TMP_InputField _inputRepeatPassField;
        [SerializeField] private TMP_Text _inputHideField;
        [SerializeField] private TMP_Text _inputRepeadHideField;
        [SerializeField] private Toggle _togglePermissionFirst;
        [SerializeField] private Toggle _togglePermissionSecond;
        [SerializeField] private Toggle _togglePermissionTerird;
        [SerializeField] private GameObject _togglePanel;
        public Action OnBackButtonCLick;

        private SaveLoadService _saveLoadService;
        private List<Employee> _employees = new();
        private Employee _selectedEmployee;
        private GameObject _tempPanel;
        private WarningPanel _warningPanel;
        private List<string> _lockLogins = new List<string>();
        private bool _isReseted;
        private bool _isPassInputed;
        private bool _isLogInputed;
        private bool _isPassCorrectInput;
        private char _simbol = '*';
        private int _permissionFirst;
        private int _permissionSecond;
        private int _permissionTerird;
        private int _permission;
        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _warningPanel = warningPanel;
            AddListeners();
        }

        public void Work()
        {
            FillEmployeesList();
            _employees = _saveLoadService.GetEmployees();
            _resetInputTextButton.gameObject.SetActive(false);
            _employeerPanel.SetActive(true);
            _okButton.gameObject.SetActive(false);
            _inputRepeatPassField.interactable = false;
            _inputHideField.text = "";
            _inputLoginField.interactable = true;
            _inputLoginField.Select();
            _inputLoginField.ActivateInputField();
            _applyButton.interactable = true;
            _applyRepeatButton.interactable = true;
            _permissionFirst = 0;
            _permissionSecond = 1;
            _permissionTerird = 2;
            _permission = 0;

            _togglePanel.gameObject.SetActive(false);
            
            if (CanSetPermission())
            {
                _togglePanel.gameObject.SetActive(true);
            }
        }

        private bool CanSetPermission()
        {
            if (_saveLoadService.Employee.Permission=="2")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void FillEmployeesList()
        {
            _tempPanel = Instantiate(_employeesList, _viweport.transform);
            _tempPanel.SetActive(true);
            _scroll.content = _tempPanel.GetComponent<RectTransform>();
            _employees = _saveLoadService.GetEmployees();

            foreach (Employee employee in _employees)
            {
                GameObject newEmployee = Instantiate(_employeeItem, _tempPanel.transform);
                TMP_Text text = newEmployee.GetComponentInChildren<TMP_Text>();
                text.text = employee.Login;
            }
        }

        public void Reset()
        {
            _isReseted = true;
            _inputLoginField.text = "";
            _inputPassField.text = "";
            _inputRepeatPassField.text = "";
            _inputHideField.text = "";
            _inputLoginField.Select();
            _inputLoginField.ActivateInputField();
            _applyButton.interactable = false;
            _resetInputTextButton.interactable = false;
            Destroy(_tempPanel);
            _isReseted = false;
            _employeeRepeatPanel.SetActive(false);
        }

        public void ValidateInput()
        {
            _isLogInputed = IsLoginValidate();
            _isPassInputed = IsPassValidate();

            if (_isLogInputed && _isPassInputed)
            {
                _employeerPanel.SetActive(false);
                AddedEmployee();
            }
        }

        public bool IsLoginValidate()
        {
            foreach (var employee in _employees)
            {
                if (_inputLoginField.text == employee.Login)
                {
                    _warningPanel.ShowWindow(WindowNames.OnLoginAlreadyExist.ToString());
                    SentLogMessage("Логин уже существует", "");
                    return false;
                }
            }

            SentLogMessage("Введен новый логин:  " + _inputLoginField.text, "");
            _isLogInputed = true;
            _inputLoginField.interactable = false;
            _inputPassField.interactable = true;
            _inputPassField.Select();
            _inputPassField.ActivateInputField();

            return true;
        }

        private bool IsPassValidate()
        {
            if (!_isReseted)
            {
                if (_inputPassField.text != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private void ShowRepeatConfirmation()
        {
            _inputHideField.text = "";
            _employeeRepeatPanel.SetActive(true);
            _inputRepeatPassField.interactable = true;
            _inputRepeatPassField.Select();
            _inputRepeatPassField.ActivateInputField();
        }

        private void ValidateRepeatInput()
        {
            if (_inputRepeatPassField.text == _inputPassField.text)
            {
                _isPassCorrectInput = true;
                AddedEmployee();
            }
            else
            {
                _warningPanel.ShowWindow(WindowNames.OnWriteIncorrectPassword.ToString());
                _isPassCorrectInput = false;
            }
        }

        private void OnApplyAddedEmployee()
        {
            _okButton.gameObject.SetActive(false);
            Reset();
            Work();
        }
        
        private void ResetInput()
        {
            SentLogMessage("Выполнен сброс логина/пароля", "сброс логина/пароля");
            Reset();
        }

        private void AddedEmployee()
        {
            _inputHideField.text = "";
            _employeeRepeatPanel.SetActive(false);
            _selectedEmployee = _saveLoadService.Employee;
            SentLogMessage("Выполнено добавление сотрудника", "");

            string action = "Выполнил создание сотрудника" + " " + _inputLoginField.text;
            string Login = _selectedEmployee.Login;
            string Pass = _selectedEmployee.Password;
            string Key = "*";
            string ShortNumber = "*";
            DateTime Time = DateTime.Now;

            SentData sentData = new SentData(action, Login, Pass, Key, ShortNumber, Time.ToString(), "");
            SentDataMessage(sentData);

            action = "Создан новый сотрудник";
            Login = _inputLoginField.text;
            Pass = _inputPassField.text;
            Key = "*";
            ShortNumber = "*";
            Time = DateTime.Now;

            sentData = new SentData(action, Login, Pass, Key, ShortNumber, Time.ToString(), "");
            SentDataMessage(sentData);

            _employeerPanel.SetActive(false);
            _applyButton.interactable = true;
            Employee employee = new Employee(_inputLoginField.text, _inputPassField.text, _permission.ToString());

            employee.SetEquipmentData(DateTime.Now);
            _saveLoadService.AddNewEmployee(employee);
            _saveLoadService.SaveDatabase();
            _inputLoginField.text = "";
            _inputPassField.text = "";
            _inputRepeatPassField.text = "";
            _okButton.gameObject.SetActive(true);
        }

        public void SwithState(bool state)
        {
            _panel.gameObject.SetActive(state);
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

        private void AddListeners()
        {
            _backButton.onClick.AddListener(OnCLickBackButton);
            _resetInputTextButton.onClick.AddListener(ResetInput);
            _applyButton.onClick.AddListener(ValidateInput);
            _applyRepeatButton.onClick.AddListener(ValidateRepeatInput);
            _okButton.onClick.AddListener(OnApplyAddedEmployee);
            _inputPassField.onValueChanged.AddListener(delegate { HideText(_inputPassField); });
            _inputRepeatPassField.onValueChanged.AddListener(delegate { HideText(_inputRepeatPassField); });
            _togglePermissionFirst.onValueChanged.AddListener(delegate { OnChangePermission(_permissionFirst);});
            _togglePermissionSecond.onValueChanged.AddListener(delegate { OnChangePermission(_permissionSecond);});
            _togglePermissionTerird.onValueChanged.AddListener(delegate { OnChangePermission(_permissionTerird);});
        }

        private void OnChangePermission(int permission)
        {
            _permission = permission;
            _togglePermissionFirst.isOn = false;
            _togglePermissionSecond.isOn= false;
                _togglePermissionTerird.isOn= false;
        }

        private void HideText(TMP_InputField _inputField )
        {
            int length = _inputField.text.Length;
            
                if (!_inputRepeatPassField.interactable)
            {
                _inputHideField.text = new string(_simbol, length);
            }
            else
            {
                string text = new string(_simbol, length);
                _inputRepeadHideField.text = text;
            }
           
        }

        private void RemuveListeners()
        {
            _inputPassField.onValueChanged.RemoveListener(delegate { HideText(_inputPassField); });
            _inputRepeatPassField.onValueChanged.RemoveListener(delegate { HideText(_inputRepeatPassField); });
            _backButton.onClick.RemoveListener(OnCLickBackButton);
            _resetInputTextButton.onClick.RemoveListener(ResetInput);
            _applyButton.onClick.RemoveListener(ValidateInput);
            _applyRepeatButton.onClick.RemoveListener(ValidateRepeatInput);
            _okButton.onClick.RemoveListener(OnApplyAddedEmployee);
            _togglePermissionFirst.onValueChanged.RemoveListener(delegate { OnChangePermission(_permissionFirst);});
            _togglePermissionSecond.onValueChanged.RemoveListener(delegate { OnChangePermission(_permissionSecond);});
            _togglePermissionTerird.onValueChanged.RemoveListener(delegate { OnChangePermission(_permissionTerird);});
        }
        
        public void OnCLickBackButton()
        {
            SentLogMessage("<- Назад ", "");
            SwithState(false);
            OnBackButtonCLick.Invoke();
        }
    }
}