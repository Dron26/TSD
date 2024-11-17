using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastracture;
using CodeBase.Infrastracture.Datas;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.AdditionalPanels
{
    public class ResetPanel : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject _panel;

        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _applyEquimpmentButton;
        [SerializeField] private Button _applyTrolleyButton;
        [SerializeField] private Button _resetInputPassTextButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _okPanel;
        [SerializeField] private Button _nokPanel;
        [SerializeField] private GameObject _passPanel;
        [SerializeField] private GameObject _equipmentPanel;
        [SerializeField] private GameObject _trolleyPanel;
        [SerializeField] private GameObject _employeesList;
        [SerializeField] private GameObject _employeeItem;
        [SerializeField] private GameObject _viewport;

        [SerializeField] private TMP_InputField _inputLoginField;
        [SerializeField] private TMP_InputField _newPassTextInput;
        [SerializeField] private TMP_Text _textlogin;
        [SerializeField] private TMP_Text _textloginTrolley;
        [SerializeField] private TMP_Text _textEquipment;
        [SerializeField] private TMP_Text _textTrolley;
        [SerializeField] private TMP_Text _hideText;
        [SerializeField] private ScrollRect _scroll;

        Dictionary<GameObject, Employee> _data = new Dictionary<GameObject, Employee>();
        public Action OnBackButtonClick;

        private SaveLoadService _saveLoadService;
        private List<Employee> _employees = new();
        private Employee _selectedEmployee;
        private GameObject _tempPanel;
        private WarningPanel _warningPanel;
        private SwichResetPanel _swichResetPanel;
        private List<string> _logins = new List<string>();
        private string _loginText = "попытка ввода логина : ";
        private char _simbol = '*';
        private bool _isSelectedEquipment;
        private bool _isSelectedEmployee;
        private bool _isSelectedTrolley;
        private bool _isTrolleyActive;
        private bool _isReseted;
        private bool _isLogInputed;
        private bool _isLogStart;
        private bool _isLogStartInputed;
        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel, SwichResetPanel swichResetPanel)
        {
            _saveLoadService = saveLoadService;
            _warningPanel = warningPanel;
            _swichResetPanel = swichResetPanel;
            AddListeners();
        }
        
        public void Reset()
        {
            _isReseted = true;
            _applyButton.interactable = false;
            _applyEquimpmentButton.interactable = false;
            _resetInputPassTextButton.interactable = false;
            _passPanel.SetActive(false);
            _equipmentPanel.SetActive(false);
            _trolleyPanel.SetActive(false);
            _newPassTextInput.interactable = false;
            _newPassTextInput.text = "";
            _data = new Dictionary<GameObject, Employee>();
            _okPanel.gameObject.SetActive(false);
            _nokPanel.gameObject.SetActive(false);
            _isSelectedEquipment = false;
            _isSelectedEmployee = false;
            _isSelectedTrolley = false;
            _inputLoginField.text = "";
            _inputLoginField.interactable = true;
            _inputLoginField.ActivateInputField();
            _inputLoginField.Select();
            _isLogStartInputed = false;
            _isLogStart = false;
            _logins.Clear();
            _isReseted = false;
            Destroy(_tempPanel);
        }

        private void UpdateLogText()
        {
            _resetInputPassTextButton.interactable = true;
            if (_inputLoginField.text.Length > 30)
            {
                _isLogInputed = false;
                Reset();
            }

            // DateTime now = DateTime.Now; 
            // Debug.Log(now);
            if (_isLogStart == false)
            {
                _isLogStart = true;
                StartCoroutine(WaiteForInputLog());
            }
        }

        public void ValidateLogin()
        {
            if (!_isReseted)
            {
                if (_logins.Contains(_inputLoginField.text) && _isLogInputed == false)
                {
                    
                    _isLogStartInputed = true;
                    StopCoroutine(WaiteForInputLog());
                    SentLogMessage(_loginText + _inputLoginField.text, "");
                    SentLogMessage("Логин верный", "");
                    SelectEmployee(_data.FirstOrDefault(x=>x.Value.Login==_inputLoginField.text).Key);
                    _inputLoginField.text = "";
                    _inputLoginField.interactable = true;
                    _isLogInputed = true;
                    _inputLoginField.interactable = false;
                }
            }
        }

        private IEnumerator WaiteForInputLog()
        {
            while (_isLogStartInputed == false)
            {
                yield return new WaitForSeconds(0.3f);

                if (_isLogInputed == false)
                {
                    ValidateLogin();
                }
            }

            yield return null;
        }

        private void ResetInput()
        {
            ResetInputPass();
        }

        private void ResetInputPass()
        {
            _okPanel.gameObject.SetActive(false);
            _nokPanel.gameObject.SetActive(false);

            if (_isSelectedEquipment)
            {
                SentLogMessage("Отмена выбора сотрудника для сброса оборудования", "");
                _resetInputPassTextButton.interactable = false;
                //_equipmentPanel.SetActive(false);
                _textlogin.text = "";
                _textEquipment.text = "";
                _applyEquimpmentButton.interactable = false;
               // _isSelectedEquipment = !_isSelectedEquipment;
                _inputLoginField.text = "";
                _inputLoginField.interactable = true;
                _inputLoginField.ActivateInputField();
            }

            if (_isSelectedEmployee)
            {
                SentLogMessage("Отмена выбора сотрудника для сброса пароля", "");
                _newPassTextInput.text = "";
                _newPassTextInput.Select();
                _newPassTextInput.ActivateInputField();
            }

            if (_isSelectedTrolley)
            {
                SentLogMessage("Отмена выбора сотрудника для сброса рохли", "");
                _textTrolley.text = "";
                _applyTrolleyButton.interactable = false;
                _isSelectedTrolley = !_isSelectedTrolley;
                _newPassTextInput.Select();
                _newPassTextInput.ActivateInputField();
            }
        }

        private void FillEmployeesList(bool isSelectedEquipment, bool isSelectedEmployee, bool _isSelectedTrolley)
        {
            _tempPanel = Instantiate(_employeesList, _viewport.transform);
            _tempPanel.SetActive(true);
            _scroll.content = _tempPanel.GetComponent<RectTransform>();

            _employees = _saveLoadService.GetEmployees();

            foreach (Employee employee in _employees)
            {
                if (isSelectedEquipment)
                {
                    if (employee.HaveEquipment)
                    {
                        CreateUserForList(employee);
                    }
                }
                else if (isSelectedEmployee)
                {
                    CreateUserForList(employee);
                }
                else if (_isSelectedTrolley)
                {
                    if (employee.HaveTrolley)
                    {
                        CreateUserForList(employee);
                    }
                }
            }
            
            foreach (Employee employee in _employees)
            {
                _logins.Add(employee.Login);
            }
        }

        private void CreateUserForList(Employee employee)
        {
            GameObject button = Instantiate(_employeeItem, _tempPanel.transform);
            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            text.text = employee.Login;
            button.GetComponent<Button>().onClick.AddListener(() => SelectEmployee(button));
            _data.Add(button, employee);
            _logins.Add(employee.Login);
        }

        private void SelectEmployee(GameObject button)
        {
            GetEmployee(button);
            SetInfo();
        }

        private void GetEmployee(GameObject button)
        {
            _selectedEmployee = _data[button];
            SentLogMessage("Выбран сотрудник " + _selectedEmployee.Login, "");
            ResetPanels();
        }

        private void SetInfo()
        {
            if (_isSelectedEquipment)
            {
                _equipmentPanel.SetActive(true);

                _textlogin.text = _selectedEmployee.Login;
                _textEquipment.text = _selectedEmployee.Box.Key + "/ " +
                                      _selectedEmployee.Box.Equipment.SerialNumber[^4..];
                _applyEquimpmentButton.interactable = true;

                _passPanel.SetActive(false);
                _trolleyPanel.SetActive(false);

                _applyTrolleyButton.interactable = false;
                _applyButton.interactable = false;
                _resetInputPassTextButton.interactable = false;
                _newPassTextInput.interactable = false;
            }

            if (_isSelectedEmployee)
            {
                _passPanel.SetActive(true);
                _applyButton.interactable = true;
                _resetInputPassTextButton.interactable = true;
                _newPassTextInput.interactable = true;
                _newPassTextInput.Select();
                _newPassTextInput.ActivateInputField();
                _applyEquimpmentButton.interactable = false;
                _equipmentPanel.SetActive(false);
                _trolleyPanel.SetActive(false);

                _textlogin.text = "";
                _textEquipment.text = "";
                _applyTrolleyButton.interactable = false;
            }

            if (_isSelectedTrolley)
            {
                _textTrolley.text = _selectedEmployee.Trolley.Number;
                _applyTrolleyButton.interactable = true;
                _trolleyPanel.SetActive(true);
                _textloginTrolley.text = _selectedEmployee.Login;
                _textTrolley.text = _selectedEmployee.Trolley.Number;


                _applyButton.interactable = false;
                _resetInputPassTextButton.interactable = false;
                _newPassTextInput.interactable = false;
                _applyEquimpmentButton.interactable = false;
            }
        }

        private void ResetPanels()
        {
            _equipmentPanel.SetActive(false);
            _textlogin.text = "";
            _textEquipment.text = "";
            _applyEquimpmentButton.interactable = false;
            _passPanel.SetActive(false);
            _applyButton.interactable = false;
            _resetInputPassTextButton.interactable = false;
            _newPassTextInput.interactable = false;
            _textTrolley.text = "";
            _applyTrolleyButton.interactable = false;
            _trolleyPanel.SetActive(false);
        }

        private void ResetEmployeePass()
        {
            SentLogMessage("-> Сбросить пароль ", "");
            _okPanel.gameObject.SetActive(false);
            _nokPanel.gameObject.SetActive(false);
            FillEmployeesList(false, true, false);
            _passPanel.SetActive(true);
            _isSelectedEmployee = true;
        }

        private void ResetEquipment()
        {
            SentLogMessage("-> Сбросить оборудование ", "");
            _okPanel.gameObject.SetActive(false);
            _nokPanel.gameObject.SetActive(false);
            FillEmployeesList(true, false, false);
            _equipmentPanel.SetActive(true);
            _isSelectedEquipment = true;
            _inputLoginField.interactable = true;
            _inputLoginField.ActivateInputField();
            _inputLoginField.Select();
        }

        private void ResetTrolley()
        {
            SentLogMessage("-> Сбросить рохлю ", "");
            _okPanel.gameObject.SetActive(false);
            _nokPanel.gameObject.SetActive(false);
            FillEmployeesList(false, false, true);
            _trolleyPanel.SetActive(true);
            _isSelectedTrolley = true;
        }

        private void ApplyAction()
        {
            SentLogMessage("-> подтвердить ", "");

            if (_newPassTextInput.text != null || _newPassTextInput.text != "")
            {
                _selectedEmployee.SetPassword(_newPassTextInput.text);
                _saveLoadService.Database.SetCurrentEmployeer(_selectedEmployee);
                _saveLoadService.SaveDatabase();
                _okPanel.gameObject.SetActive(true);
                _nokPanel.gameObject.SetActive(false);
                SentLogMessage("Пароль изменен", "");
                ClosePassPanel();
            }
            else
            {
                _okPanel.gameObject.SetActive(false);
                _nokPanel.gameObject.SetActive(true);
                _warningPanel.ShowWindow(WindowNames.EmptyPassword.ToString());
                ResetInputPass();
            }
        }

        private void ApplyTrolleyAction()
        {
            Employee responsibleEmployee = _saveLoadService.Employee;

            SentLogMessage(
                responsibleEmployee.Login + " Сбросил с  сотрудника " + _selectedEmployee.Login + " рохлю " + "",
                "Сброс рохли");

            SentDataMessage(new SentData("Сброс рохли ", _selectedEmployee.Login, "",
                "", "", DateTime.Now.ToString(), _selectedEmployee.Trolley.Number));

            _saveLoadService.SetTrolley(_selectedEmployee.GetTrolley());
            _saveLoadService.SetCurrentEmployee(_selectedEmployee);
            _saveLoadService.SaveDatabase();
            _textTrolley.text = "";

            if (responsibleEmployee.Login != _selectedEmployee.Login)
            {
                _saveLoadService.SetCurrentEmployee(responsibleEmployee);
            }

            Reset();
            OnCLickBackButton();
        }

        private void ApplyEquipmentAction()
        {
            Employee responsibleEmployee = _saveLoadService.Employee;

            SentLogMessage(
                responsibleEmployee.Login + " Сбросил с  сотрудника " + _selectedEmployee.Login + " оборудование " +
                _selectedEmployee.Equipment.SerialNumber[^4..], "Сброс оборудования");

            SentDataMessage(new SentData("Сброс оборудования ", _selectedEmployee.Login, _selectedEmployee.Password,
                _selectedEmployee.Box.Key,
                _selectedEmployee.Box.Equipment.SerialNumber[^4..], DateTime.Now.ToString(), ""));

            _saveLoadService.SetBox(_selectedEmployee.GetBox());
            _saveLoadService.SetCurrentEmployee(_selectedEmployee);
            _saveLoadService.SaveDatabase();
            _textlogin.text = "";
            _textEquipment.text = "";

            if (responsibleEmployee.Login != _selectedEmployee.Login)
            {
                _saveLoadService.SetCurrentEmployee(responsibleEmployee);
            }

            Reset();
            OnCLickBackButton();
        }

        private void ClosePassPanel()
        {
            _newPassTextInput.text = "";
            _applyButton.interactable = false;
            _applyTrolleyButton.interactable = false;
            _newPassTextInput.interactable = false;
            _passPanel.SetActive(false);
            _resetInputPassTextButton.interactable = false;
        }

        private void AddListeners()
        {
            _swichResetPanel.OnSelectTrolley += ResetTrolley;
            _swichResetPanel.OnSelectEquipment += ResetEquipment;
            _swichResetPanel.OnSelectPass += ResetEmployeePass;

            _resetInputPassTextButton.onClick.AddListener(ResetInput);
            _applyButton.onClick.AddListener(ApplyAction);
            _applyTrolleyButton.onClick.AddListener(ApplyTrolleyAction);
            _applyEquimpmentButton.onClick.AddListener(ApplyEquipmentAction);
            _backButton.onClick.AddListener(OnCLickBackButton);
            _newPassTextInput.onValueChanged.AddListener(delegate { OnValueChangedPassInput(); });
            _saveLoadService.OnSelectTrolley += OnSelectTrolley;
            _inputLoginField.onValueChanged.AddListener(delegate { UpdateLogText(); });
        }

        private void OnSelectTrolley()
        {
            _isTrolleyActive = !_isTrolleyActive;
        }

        private void OnValueChangedPassInput()
        {
            int length = _newPassTextInput.text.Length;
            _hideText.text = new string(_simbol, length);
        }

        private void RemuveListeners()
        {
            _swichResetPanel.OnSelectTrolley -= ResetTrolley;
            _swichResetPanel.OnSelectEquipment -= ResetEquipment;
            _swichResetPanel.OnSelectPass -= ResetEmployeePass;

            _resetInputPassTextButton.onClick.RemoveListener(ResetInput);
            _applyButton.onClick.RemoveListener(ApplyAction);
            _applyTrolleyButton.onClick.RemoveListener(ApplyTrolleyAction);
            _applyEquimpmentButton.onClick.RemoveListener(ApplyEquipmentAction);
            _backButton.onClick.RemoveListener(OnCLickBackButton);
            _newPassTextInput.onValueChanged.RemoveListener(delegate { OnValueChangedPassInput(); });
            _saveLoadService.OnSelectTrolley -= OnSelectTrolley;
            _inputLoginField.onValueChanged.RemoveListener(delegate { UpdateLogText(); });
        }

        private void OnEmployeeScan()
        {
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

        public void SwithState(bool state)
        {
            _panel.SetActive(state);
        }

        

        public void OnCLickBackButton()
        {
            SentLogMessage("<- Назад ", "");
            OnBackButtonClick.Invoke();
            Reset();
            SwithState(false);
        }
    }
}

namespace CodeBase.Infrastracture.AdditionalPanels
{
}