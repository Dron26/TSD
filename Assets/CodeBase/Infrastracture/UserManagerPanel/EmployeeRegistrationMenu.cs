using System;
using CodeBase.Infrastracture.Datas;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.UserManagerPanel
{
    public class EmployeeRegistrationMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _registrationPanel;
        [SerializeField] private EmployeeValidator _employeeValidator;
        [SerializeField] private Image _CheckUpLogin;
        [SerializeField] private Image _CheckDownLogin;
        [SerializeField] private Image _CheckUpPass;
        [SerializeField] private Image _CheckDownPass;
        
        public Action OnLogged;
        
        private SaveLoadService _saveLoadService;
        private string _textButtn = ": подтвердил логин/пароль ";

        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            AddListeners();
            _employeeValidator.Init(_saveLoadService, warningPanel);
        }

        public void Work()
        {
            _CheckDownLogin.enabled = true;
            _CheckUpLogin.enabled = false;
            _CheckDownPass.enabled = true;
            _CheckUpPass.enabled = false;
            _employeeValidator.Work();
        }

        public void ResetIcon()
        {
            _CheckDownLogin.enabled = true;
            _CheckUpLogin.enabled = false;
            _CheckDownPass.enabled = true;
            _CheckUpPass.enabled = false;
        }

        public void Reset()
        {
            ResetIcon();
            _employeeValidator.Reset();
            _employeeValidator.Work();
        }

        private void OnInputCorrectPassword()
        {
            _CheckDownPass.enabled = false;
            _CheckUpPass.enabled = true;
            OnLogged?.Invoke();
            Reset();
        }

        private void OnIsLogged()
        {
            _CheckDownLogin.enabled = false;
            _CheckUpLogin.enabled = true;
        }

        public void SwithPanelState(bool state)
        {
            _registrationPanel.gameObject.SetActive(state);
        }

        private void AddListeners()
        {
            _employeeValidator.IsLogged += OnIsLogged;
            _employeeValidator.InputCorrectPassword += OnInputCorrectPassword;
            _employeeValidator.OnInputReset += Reset;
        }

        private void RemuveListeners()
        {
            _employeeValidator.IsLogged -= OnIsLogged;
            _employeeValidator.InputCorrectPassword -= OnInputCorrectPassword;
            _employeeValidator.OnInputReset -= Reset;
        }

        private void SentLogMessage
            (string message)
        {
            _saveLoadService.SentLogInfo(message, "");
        }

        private void OnDisable()
        {
            RemuveListeners();
        }
    }
}