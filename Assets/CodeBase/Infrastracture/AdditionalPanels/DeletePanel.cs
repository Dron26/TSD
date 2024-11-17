using System;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.AdditionalPanels
{
    public class DeletePanel : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private GameObject _employeerPanel;
        [SerializeField] private GameObject _employeeRepeatPanel;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _applyRepeatButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private GameObject _employeesList;
        [SerializeField] private GameObject _employeeItem;
        [SerializeField] private GameObject _viweport;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _nokButton;
        [SerializeField] private ScrollRect _scroll;
        [SerializeField] private TMP_Text _loginText;
        [SerializeField] private TMP_Text _loginRepeatText;

        public Action OnBackButtonCLick;
        
        private List<GameObject> _employeesListItems = new List<GameObject>();
        private SaveLoadService _saveLoadService;
        private List<Employee> _employees = new();
        private Employee _selectedForDeleteEmployee;
        private Employee _registeredEmployee;
        private WarningPanel _warningPanel;
        private List<string> _lockLogins = new List<string>();
        private Dictionary<GameObject, Employee> _data = new Dictionary<GameObject, Employee>();

        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _warningPanel = warningPanel;
            AddListeners();
            SwithState(false);
        }

        public void Reset()
        {
            _loginText.text = "";
            _loginRepeatText.text = "";
            _applyButton.interactable = false;
            _employeeRepeatPanel.SetActive(false);
            _employeerPanel.SetActive(false);
            foreach (var obj in _employeesListItems)
            {
                obj.SetActive(false);
                Destroy(obj);
            }

            _employeesListItems.Clear();
            _data.Clear();
        }

        public void Work()
        {
            FillEmployeesList();
            _employees = _saveLoadService.GetEmployees();
            _employeerPanel.SetActive(true);
            _okButton.gameObject.SetActive(false);
            _nokButton.gameObject.SetActive(false);
            _applyButton.interactable = false;
        }

        private void FillEmployeesList()
        {
            _employees = _saveLoadService.GetEmployees();

            foreach (Employee employee in _employees)
            {
                if (employee.Login == _saveLoadService.Employee.Login)
                {
                    continue;
                }

                GameObject button = Instantiate(_employeeItem, _employeesList.transform);
                button.SetActive(true);
                TMP_Text text = button.GetComponentInChildren<TMP_Text>();
                text.text = employee.Login;
                button.GetComponent<Button>().onClick.AddListener(() => GetEmployee(button));
                _data.Add(button, employee);
                _employeesListItems.Add(button);
            }
        }

        private void GetEmployee(GameObject button)
        {
            _selectedForDeleteEmployee = _data[button];
            SentLogMessage("Выбран сотрудник " + _selectedForDeleteEmployee.Login, "");
            SetEmployeeInfo();
        }

        private void SetEmployeeInfo()
        {
            _loginText.text = _selectedForDeleteEmployee.Login;

            if (!_selectedForDeleteEmployee.HaveBox)
            {
                _applyButton.interactable = true;
            }
            else
            {
                SentLogMessage(
                    "Неудачная попытка удаления " + _selectedForDeleteEmployee.Login +
                    " У сотрудника есть оборудование", "");
                Reset();
                Work();
                _warningPanel.ShowWindow(WindowNames.OnHaveBox.ToString());
            }

            if (_selectedForDeleteEmployee.HaveTrolley)
            {
                SentLogMessage(
                    "Неудачная попытка удаления " + _selectedForDeleteEmployee.Login +
                    " У сотрудника есть рохля", "");
                Reset();
                Work();
                _warningPanel.ShowWindow(WindowNames.OnHaveBox.ToString());
            }
        }

        public void ValidateDelete()
        {
            _loginRepeatText.text = _selectedForDeleteEmployee.Login;
            _employeerPanel.SetActive(false);
            _employeeRepeatPanel.SetActive(true);
        }

        private void OnClickCancel()
        {
            Reset();
            Work();
        }

        private void ValidateRepeat()
        {
            _employeeRepeatPanel.SetActive(false);
            _okButton.gameObject.SetActive(true);
            _nokButton.gameObject.SetActive(false);
            SentLogMessage("Удачная попытка удаления " + _selectedForDeleteEmployee.Login, "Удаление");
            _registeredEmployee = _saveLoadService.Employee;
            string action = "Выполнил удаление сотрудника" + " " + _selectedForDeleteEmployee.Login;
            string Login = _registeredEmployee.Login;
            string Pass = _registeredEmployee.Password;
            string Key = "*";
            string ShortNumber = "*";
            string Time = DateTime.Now.ToString();

            SentData sentData = new SentData(action, Login, Pass, Key, ShortNumber, Time, "");

            SentDataMessage(sentData);

            action = "Удален сотрудник сотрудник";
            Login = _selectedForDeleteEmployee.Login;
            Pass = _selectedForDeleteEmployee.Password;
            Key = "*";
            ShortNumber = "*";
            Time = DateTime.Now.ToString();

            sentData = new SentData(action, Login, Pass, Key, ShortNumber, Time, "");
            SentDataMessage(sentData);
            RemoveEmployee();

            Reset();
            Work();
        }


        private void RemoveEmployee()
        {
            _saveLoadService.RemoveEmployee(_selectedForDeleteEmployee);
        }

        private void OnApplyAddedEmployee()
        {
            _okButton.gameObject.SetActive(false);
            _nokButton.gameObject.SetActive(false);
            Reset();
        }

        public void SwithState(bool state)
        {
            _panel.gameObject.SetActive(state);
        }

        private void SentLogMessage(string message, string comment)
        {
            _saveLoadService.SentLogInfo(message, comment);
        }

        private void SentDataMessage(SentData sentData)
        {
            _saveLoadService.SentDataInfo(sentData);
        }


        private void OnDisable()
        {
            RemuveListeners();
        }

        private void AddListeners()
        {
            _applyButton.onClick.AddListener(ValidateDelete);
            _applyRepeatButton.onClick.AddListener(ValidateRepeat);
            _cancelButton.onClick.AddListener(OnClickCancel);
            _okButton.onClick.AddListener(OnApplyAddedEmployee);
            _nokButton.onClick.AddListener(OnApplyAddedEmployee);
            _exitButton.onClick.AddListener(OnCLickBackButton);
        }

        private void RemuveListeners()
        {
            _applyButton.onClick.RemoveListener(ValidateDelete);
            _applyRepeatButton.onClick.RemoveListener(ValidateRepeat);
            _cancelButton.onClick.RemoveListener(OnClickCancel);
            _okButton.onClick.RemoveListener(OnApplyAddedEmployee);
            _nokButton.onClick.RemoveListener(OnApplyAddedEmployee);
            _exitButton.onClick.RemoveListener(OnCLickBackButton);
        }


        public void OnCLickBackButton()
        {
            SentLogMessage("<- Назад", "");
            SwithState(false);
            OnBackButtonCLick.Invoke();
        }
    }
}