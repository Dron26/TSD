using System;
using System.Collections;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.UserManagerPanel
{
    public class EmployeeValidator : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputLoginField;
        [SerializeField] private TMP_InputField _inputPassField;
        [SerializeField] private TMP_InputField _inputHideField;
        [SerializeField] private Button _resetInput;


        public Action IsLogged;
        public Action InputCorrectPassword;
        public Action OnInputReset;

        private List<string> _logins = new List<string>();
        private WarningPanel _warningPanel;
        private List<Employee> _employees;
        private Employee _newEmployee;
        private SaveLoadService _saveLoadService;
        private string _loginText = "попытка ввода логина : ";
        private string _passText = " ввел пароль : ";
        private char _simbol = '*';
        private bool _isReseted;
        private bool _isPassInputed;
        private bool _isLogInputed;
        private bool _tabPressed;
        private bool _isLogStartInputed;
        private bool _isPassStartInputed;
        private bool _isLogStart;
        private bool _isPassStart;


        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _warningPanel = warningPanel;
            AddListeners();
        }

        public void Work()
        {
            _employees = _saveLoadService.GetEmployees();
            _inputLoginField.interactable = true;
            _inputLoginField.ActivateInputField();
            _inputLoginField.Select();
            _newEmployee = _employees[0];
            _inputPassField.interactable = false;

            foreach (Employee employee in _employees)
            {
                _logins.Add(employee.Login);
            }
        }

        public void Reset()
        {
            _isReseted = true;
            _inputLoginField.interactable = true;
            _inputLoginField.ActivateInputField();
            _inputLoginField.Select();
            _inputPassField.interactable = false;
            _inputLoginField.text = "";
            _inputPassField.text = "";
            _inputHideField.text = "";
            _isLogInputed = false;
            _logins.Clear();
            _isPassInputed = false;
            _isLogStartInputed = false;
            _isPassStartInputed = false;
            _isLogStart = false;
            _isPassStart = false;
            _tabPressed = false;
            _isReseted = false;
        }

        private void ResetInput()
        {
            SentLogMessage("Выполнен сброс ввода логина/пароля", "");
            OnInputReset?.Invoke();
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

                    _isLogInputed = true;
                    IsLogged?.Invoke();
                    _inputLoginField.interactable = false;
                    _inputPassField.interactable = true;
                    _inputPassField.ActivateInputField();
                    _inputPassField.Select();
                }
            }
        }

        private void ValidatePass()
        {
            if (!_isReseted)
            {
                if (_isLogInputed)
                {
                    int index = _logins.IndexOf(_inputLoginField.text);
                    string oldPass = _employees[index].Password;

                    if (oldPass == _inputLoginField.text)
                    {
                        _employees[index].SetPassword(_inputPassField.text);
                        oldPass = _employees[index].Password;
                    }

                    if (oldPass == _inputPassField.text && _isPassInputed == false &&
                        _inputLoginField.text == _employees[index].Login)
                    {
                        StopCoroutine(WaiteForInputPass());
                        SentLogMessage(_employees[index].Login + _passText + _employees[index].Password, "");
                        SentLogMessage("Пароль верный", "");

                        SentLogMessage(_employees[index].Login + " вошел в систему ", "Вход в систему");
                        _newEmployee = _employees[index];
                        SentLogMessage("___________________________", "");
                        _isPassInputed = true;

                        string action = "Вошел в систему";
                        string login = _newEmployee.Login;
                        string pass = "*";
                        string key = "*";
                        string shortNumber = "*";
                        string time = DateTime.Now.ToString();
                        string printerNumber = "";
                        if (_newEmployee != _saveLoadService.Employee)
                        {
                            _saveLoadService.SetCurrentEmployee(_newEmployee);
                            if (_newEmployee.Box != null)
                            {
                                key = _newEmployee.Box.Key.ToString();
                                shortNumber = _newEmployee.Box.Equipment.SerialNumber[^4..];
                                if (_newEmployee.HavePrinter)
                                {
                                    printerNumber = _newEmployee.Printer.SerialNumber;
                                }
                                
                            }

                            SentData sentData = new SentData(action, login, pass, key, shortNumber, time, "");
                            SentDataMessage(sentData);
                        }

                        CheckPermission();
                        _saveLoadService.SaveDatabase();
                        InputCorrectPassword?.Invoke();
                    }
                }
            }
        }

        private void CheckPermission()
        {
            string text = "";

            if (_saveLoadService.Employee.Permission == "0")
            {
                text = "сотрудника ";
            }
            else if (_saveLoadService.Employee.Permission == "1")
            {
                text = "менеджера ";
            }
            else if (_saveLoadService.Employee.Permission == "2")
            {
                text = "администратора ";
            }

            SentLogMessage($" {_newEmployee.Login} имеет права {text}", "");
        }


        private void AddListeners()
        {
            _inputLoginField.onValueChanged.AddListener(delegate { UpdateLogText(); });
            _inputPassField.onValueChanged.AddListener(delegate { UpdatePassText(); });
            _resetInput.onClick.AddListener(ResetInput);
        }

        private void UpdateLogText()
        {
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

        private IEnumerator WaiteForInputPass()
        {
            while (_isPassStartInputed == false)
            {
                yield return new WaitForSeconds(0.3f);

                if (_isPassInputed == false)
                {
                    ValidatePass();
                }
            }

            yield return null;
        }

        private void UpdatePassText()
        {
            if (_inputPassField.text.Length > _inputHideField.text.Length || _inputHideField.text.Length == 0)
            {
                _inputHideField.text += _inputPassField.text[_inputPassField.text.Length - 1];
            }

            int length = _inputPassField.text.Length;
            _inputHideField.text = new string(_simbol, length);
            
            if (_isPassStart == false)
            {
                _isPassStart = true;
                StartCoroutine(WaiteForInputPass());
            }
        }

        private void RemuveListeners()
        {
            _inputLoginField.onValueChanged.RemoveListener(delegate { UpdateLogText(); });
            _inputPassField.onValueChanged.RemoveListener(delegate { UpdatePassText(); });
            _resetInput.onClick.RemoveListener(ResetInput);
        }


        private void SentLogMessage(string message, string comment)
        {
            _saveLoadService.SentLogInfo(message, comment);
        }

        private void SentDataMessage(SentData message)
        {
            _saveLoadService.SentDataInfo(message);
        }
    }
}