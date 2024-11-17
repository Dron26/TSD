using System;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using CodeBase.Infrastracture.TrolleyGroup;
using UnityEngine;

namespace CodeBase.Infrastracture
{
    public class WarningPanel : MonoBehaviour
    {
        [SerializeField] private WarningWindow _warningWindow;
        SaveLoadService _saveLoadService;

        private List<string> _messages = new List<string>();
        private Dictionary<WindowNames, Action> _windowNames;
        private List<string> _texts;
        
        private string _equipmentNumber;
        private string _trollyNumber;
        private string _cellNumber;
        private string _employeeLogin;
        private string _textMassage="!!!Warning!!! Ответ сотруднику: ";
        private string _errorMessage;

        public void Init(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }
        
        public void Work()
        {
            FillText();
            FillDictionary();
        }

        private void FillDictionary()
        {
            _windowNames = new Dictionary<WindowNames, Action>();
            _windowNames.Add(WindowNames.CanNotTakeEquipment, ShowCanNotTakeEquipment);
            _windowNames.Add(WindowNames.CanNotReturnEquipment, CanNotReturnEquipment);
            _windowNames.Add(WindowNames.CanNotTakeTrolley, CanNotTakeTrolley);
            _windowNames.Add(WindowNames.CanReturnTrolley, CanReturnTrolley);
            _windowNames.Add(WindowNames.CanNotReturnOtherEquipment, CanNotReturnOtherEquipment);
            _windowNames.Add(WindowNames.CanReturnEquipment, CanReturnEquipment);
            _windowNames.Add(WindowNames.CanNotTakeAnyEquipment, CanNotTakeAnyEquipment);
            _windowNames.Add(WindowNames.EmptyPassword, OnEmptyPassword);
            _windowNames.Add(WindowNames.OnWrongLogin, OnWrongLogin);
            _windowNames.Add(WindowNames.OnLoginAlreadyExist, OnLoginAlreadyExist);
            _windowNames.Add(WindowNames.OnHaveBox, OnHaveBox);
            _windowNames.Add(WindowNames.OnWriteIncorrectPassword, OnWriteIncorrectPassword);
            _windowNames.Add(WindowNames.CanNotReturnOtherTrolley, CanNotReturnOtherTrolly);
            _windowNames.Add(WindowNames.OnHavetrolley, OnHavetrolley);
            _windowNames.Add(WindowNames.OnBoxAlreadyExist, OnBoxAlreadyExist);
            _windowNames.Add(WindowNames.OnEquipmentAlreadyExist, OnEquipmentAlreadyExist);
            _windowNames.Add(WindowNames.OnTrolleyAlreadyExist, OnTrolleyAlreadyExist);
            _windowNames.Add(WindowNames.OnPrinterAlreadyExist, OnPrinterAlreadyExist);
            _windowNames.Add(WindowNames.NoScannersAvailable, NoScannersAvailable);
            _windowNames.Add(WindowNames.NoTrolleyAvailable, NoTrolleyAvailable);
            _windowNames.Add(WindowNames.OnHaveDuplicate, OnHaveDuplicate);
            _windowNames.Add(WindowNames.CanNotRegistredTrolley, CanNotRegistredTrolley);
            _windowNames.Add(WindowNames.NotReturnYesterday, NotReturnYesterday);
            _windowNames.Add(WindowNames.ErrorBuyed, OnErrorBuyed);
        }

        private void FillText()
        {
            _texts = new List<string>();
            _texts.Add("Для получения ");
            _texts.Add(" должен хранится ");
            _texts.Add(" зарегистрирован на сотруднике: ");
            _texts.Add("Разместите ");
            _texts.Add("Сканер № ");
            _texts.Add(" в ячейке № ");
            _texts.Add("На сотруднике: ");
            _texts.Add(" нет зарегистрированных сканеров ");
            _texts.Add("Пароль не может быть пустым ");
            _texts.Add("Введеный логин недопустим ");
            _texts.Add("Введеный логин уже существует ");
            _texts.Add("Перед удалением необходимо вернуть ");
            _texts.Add("Введенный пароль не соответствует ");
            _texts.Add(" Рохля № ");//13
            _texts.Add(" зарегистрирована на сотруднике: ");
            _texts.Add(" оборудование ");
            _texts.Add(" Ящик ");
            _texts.Add(" Сканер ");//17
            _texts.Add(" Рохла ");
            _texts.Add(" с таким номером уже существует ");
            _texts.Add(" нового сканера, ");
            _texts.Add(" необходимо сдать ");
            _texts.Add(" Нет свободных ящиков ");
            _texts.Add(" Нет свободных рохлей ");//23
            _texts.Add(" Допущен дубликат ");//24
            _texts.Add(" Вы не вернули оборудование в конце смены! Не повторяйте ошибок, оборудование необходимо сдавать в конце смены и ставить на зарядку.");//25
            _texts.Add(" Принтер ");//26
            _texts.Add(_errorMessage);
        }

        private void CanNotRegistredTrolley()
        {
            List<Employee> employees = _saveLoadService.GetEmployees();
            Trolley trolley = _saveLoadService.SelectedTrolley;
            foreach (Employee employee in employees)
            {
                if (employee.HaveTrolley)
                {
                    if (employee.Trolley.Number==trolley.Number)
                    {
                        _employeeLogin = employee.Login;
                        break;
                    }
                    else
                    {
                        _employeeLogin="!!!Error!!!";
                    }
                }
            }
            
            string text = _texts[18]+_texts[14] +_employeeLogin;
           
            SetText(text);
        }

        private void OnHaveDuplicate()
        {
            string text = _texts[24];
           
            SetText(text);
        }

        private void NoScannersAvailable()
        {
            string text = _texts[22];
           
            SetText(text);
        }

        private void NoTrolleyAvailable()
        {
            string text = _texts[23];
           
            SetText(text);
        }

        private void CanReturnTrolley()
        {
            string text = _texts[0] + _texts[20] +_texts[4] + _equipmentNumber;
           
            SetText(text);
        }

        private void CanNotTakeTrolley()
        {
            string text = _texts[0] + _texts[20] +_texts[4] + _equipmentNumber;
           
            SetText(text);
        }

        private void OnTrolleyAlreadyExist()
        {
            string text = _texts[18]+_texts[19]+_employeeLogin;
            
            SetText(text);
        }

        private void OnPrinterAlreadyExist()
        {
            string text = _texts[26]+_texts[19]+_employeeLogin;
            
            SetText(text);
        }

        private void OnEquipmentAlreadyExist()
        {
            string text = _texts[17]+_texts[19];
            
            SetText(text);
        }

        private void OnBoxAlreadyExist()
        {
            string text = _texts[16]+_texts[19];
            
            SetText(text);
        }
        
        private void OnErrorBuyed()
        {
            string text = _errorMessage;
            
            SetText(text);
        }
        
        public void ShowWindow(string name)
        {
            Employee employee = _saveLoadService.Employee;
           
            if (employee.Box.Key!=null)
            {
                _equipmentNumber = employee.Equipment.SerialNumber[^4..];
                _cellNumber = employee.Box.Key;
            }
            else
            {
                _equipmentNumber="";
                _cellNumber="";
            }
            
            
            if (employee.Trolley!=null)
            {
                _trollyNumber = employee.Trolley.Number;
            }
            else
            {
                _trollyNumber="";
            }
            
            _employeeLogin = employee.Login;

            if (_windowNames.ContainsKey((WindowNames)Enum.Parse(typeof(WindowNames), name)))
            {
                _windowNames[(WindowNames)Enum.Parse(typeof(WindowNames), name)]();
            }
        }

        private void ShowCanNotTakeEquipment()
        {
            string text = _texts[0] + _texts[20] + _texts[21]+_texts[4] + _equipmentNumber;
           
            SetText(text);
        }

        private void CanNotReturnEquipment()
        {
            string text = _texts[4] + _equipmentNumber + _texts[1] + _texts[5] + _cellNumber;
            
            SetText(text);
        }

        private void CanNotReturnOtherEquipment()
        {
            string text = _texts[4] + _equipmentNumber + _texts[2] + _employeeLogin;
            
            SetText(text);
        }
        
        private void CanNotReturnOtherTrolly()
        {
            string text = _texts[13] + _trollyNumber + _texts[14] + _employeeLogin;
            
            SetText(text);
        }
        
        private void CanReturnEquipment()
        {
            string text = _texts[3] + _texts[4] + _equipmentNumber + _texts[5] + _cellNumber;
            
            SetText(text);
        }
        
        private void CanNotTakeAnyEquipment()
        {
            string text = _texts[6] + _employeeLogin  + _texts[7];
            
            SetText(text);
        }
        
        private void OnWrongLogin()
        {
            string text = _texts[9];
            
            SetText(text);
        }
        
        private void OnLoginAlreadyExist()
        {
            string text = _texts[10];
            
            SetText(text);
        }

        private void OnEmptyPassword()
        {
            string text = _texts[8];
            
            SetText(text);
        }
        
        private void OnHaveBox()
        {
            string text = _texts[11]+_texts[15];
            
            SetText(text);
        }

        private void OnWriteIncorrectPassword()
        {
            string text = _texts[13];
            
            SetText(text);
        }
        private void OnHavetrolley()
        {
            string text = _texts[11]+_texts[13];
            
            SetText(text);
        }
        
        private void NotReturnYesterday()
        {
            string text = _texts[25];
            
            SetText(text);
        }
        
        private void SetText(string text)
        {
            _warningWindow.gameObject.SetActive(true);
            SentLogMessage(_textMassage+text);
            _warningWindow.SetText(text);
        }

        private void SentLogMessage(string message)
        {
            _saveLoadService.SentLogInfo(message,"");
        }

        public void SetErrorMessage(string errorMessage)
        {
            _errorMessage = errorMessage;
        }
    }

    enum WindowNames
    {
        CanNotTakeEquipment,
        CanNotReturnEquipment,
        CanNotTakeTrolley,
        CanReturnTrolley,
        CanNotReturnOtherEquipment,
        CanReturnEquipment,
        CanNotTakeAnyEquipment,
        EmptyPassword,
        OnWrongLogin,
        OnLoginAlreadyExist,
        OnBoxAlreadyExist,
        OnEquipmentAlreadyExist,
        OnTrolleyAlreadyExist,
        OnPrinterAlreadyExist,
        OnHaveBox,
        OnHavetrolley,
        OnWriteIncorrectPassword,
        CanNotReturnOtherTrolley,
        NoScannersAvailable,
        NoTrolleyAvailable,
        OnHaveDuplicate,
        CanNotRegistredTrolley,
        NotReturnYesterday,
        ErrorBuyed
    }
}