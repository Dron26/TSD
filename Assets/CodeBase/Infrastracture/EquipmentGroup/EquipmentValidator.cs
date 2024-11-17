using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.EquipmentGroup
{
    public class EquipmentValidator : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private GameObject _buttonsPanel;
        [SerializeField] private GameObject _viewport;
        [SerializeField] private GameObject _printerPanel;
        [SerializeField] private ScrollRect _skroll;
        [SerializeField] private TMP_Text _equipmentNumber;
        [SerializeField] private TMP_Text _boxNumber;
        [SerializeField] private GameObject _freeBox;
        [SerializeField] private Button _buttonApply;
        [SerializeField] private Button _applyGetPrinter;
        [SerializeField] private Button _cancellGetPrinter;
        
        public Action OnTakeKey;
        public Action OnTakeTsd;
        public Action OnBackButtonCLick;
        private string _selectedKey;
        private GameObject _tempPanel;
        private List<Box> _boxes;
        private SaveLoadService _saveLoadService;
        private List<Box> _busyBoxes = new();
        private Employee _employee;
        Dictionary<string,Equipment> _freeBoxes = new Dictionary<string,Equipment>();
        private string _applyText = ": подтвердил выбор ";
        public bool _canTakeKey = false;
        public bool _canTakeTsd = false;
        private bool _isButtonFilling;
        private bool _isTakePrinter;
        public void Init(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            AddListeners();
        }

        public void Work()
        {
            _boxes = _saveLoadService.GetBoxes();
            _buttonApply.interactable = false;
            _printerPanel.SetActive(false);
            FillButtons();
        }

        public void Reset()
        {
            _boxes = new List<Box>();
            _equipmentNumber.text = "";
            _boxNumber.text = "";
            _freeBoxes = new();
            _busyBoxes = new();
            _canTakeKey = false;
            _canTakeTsd = false;
            _isButtonFilling = false;
            _isTakePrinter = false;
            Destroy(_tempPanel);
        }

        private void FillButtons()
        {
            _freeBox.gameObject.SetActive(true);
            _tempPanel = Instantiate(_buttonsPanel, _viewport.transform);
            _tempPanel.SetActive(true);

            SortButtons();
            
            foreach (var info in _freeBoxes)
            {
                GameObject button = Instantiate(_freeBox, _tempPanel.transform);

                TMP_Text text = button.GetComponentInChildren<TMP_Text>();
                text.text = info.Key.ToString();
                button.GetComponent<Button>().onClick.AddListener(() => ShowInfo(text.text));
            }
            
            _skroll.content=_tempPanel.GetComponent<RectTransform>();
            _isButtonFilling = true;
        }

        private void SortButtons()
        {
            foreach (Box box in _boxes)
            {
                if (!box.Busy)
                {
                    _freeBoxes.Add(box.Key,box.Equipment);
                }
                else
                {
                    _busyBoxes.Add(box);
                }
            }

            _freeBoxes = _freeBoxes.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }
        
        private void ShowInfo(string text)
        {
            _employee = _saveLoadService.Employee;
            _selectedKey = text;
            
            if (_freeBoxes.ContainsKey(_selectedKey))
            {
                _boxNumber.text= text;
                _equipmentNumber.text = _freeBoxes[_selectedKey].SerialNumber[^4..];
                _buttonApply.interactable = true;
                SentLogMessage(_employee.Login + " выбрал ячейку : " + _boxNumber.text + " сканер : " + _equipmentNumber.text, "");
            }
        }

        private void CheckPrinterState()
        {
            
            _employee = _saveLoadService.Employee;
            
            string convertNumber=_selectedKey.ToString();

            foreach (Box box in _boxes)
            {
                if (box.Key == convertNumber)
                {
                    int index = _boxes.IndexOf(box);

                    if (_boxes[index].Printer != null)
                    {
                        ShowPrinterWindow();
                    }
                    else
                    {
                        OnButtonClick();
                    }
                }
            }
        }
        
        private void OnButtonClick()
        {
            _employee = _saveLoadService.Employee;
            
            string convertNumber=_selectedKey.ToString();

            foreach (Box box in _boxes)
            {
                if (box.Key==convertNumber)
                {
                    int index = _boxes.IndexOf(box);

                    Box temp = new Box(_boxes[index].Key, _boxes[index].Equipment);
                    temp.SetBusy(true);
                    
                    _employee.SetEquipmentData(DateTime.Now);
                    _employee.SetBox(temp);
                    
                    SentData data = new SentData(" Выдача оборудования ", _employee.Login, _employee.Password,
                        _employee.Box.Key,
                        _employee.Box.Equipment.SerialNumber[^4..], DateTime.Now.ToString(), "");
                    
                    _saveLoadService.SetBox(_employee.Box);
                    
                    if (_isTakePrinter)
                    {
                        _employee.Box.SetPrinter(_boxes[index].GetPrinter());
                        _employee.SetPrinter(_employee.Box.GetPrinter());
                        _saveLoadService.SetPrinter(_employee.Printer);
                        data.SetPrinterNumber(_employee.Printer.SerialNumber);
                        SentLogMessage(_employee.Login + _applyText + " ячейки : " + _employee.Box.Key + " сканера : " +
                                       _employee.Equipment.SerialNumber[^4..], " Выдача оборудования, Printer SN: "+_employee.Printer.SerialNumber);

                    }
                    else
                    {
                        SentLogMessage(_employee.Login + _applyText + " ячейки : " + _employee.Box.Key + " сканера : " +
                                       _employee.Equipment.SerialNumber[^4..], " Выдача оборудования ");
                    }
                    
                    SentDataMessage(data);
                    
                    
                    _saveLoadService.SetCurrentEmployee(_employee);
                    _saveLoadService.SaveDatabase();
                    _buttonApply.interactable = false;

                    OnTakeKey?.Invoke();
                   
                    break;
                }
            }
        }

        private void ShowPrinterWindow()
        {
            _printerPanel.SetActive(true);
        }

        public void SwithState(bool state)
        {
            _panel.SetActive(state);
        }

        private void AddListeners()
        {
            _buttonApply.onClick.AddListener(CheckPrinterState);
            _applyGetPrinter.onClick.AddListener(()=>TakePrinter(true));
            _cancellGetPrinter.onClick.AddListener(()=>TakePrinter(false));
        }

        private void RemuveListeners()
        {
            _buttonApply.onClick.RemoveListener(CheckPrinterState);
            _applyGetPrinter.onClick.RemoveListener(()=>TakePrinter(true));
            _cancellGetPrinter.onClick.RemoveListener(()=>TakePrinter(false));
        }

        
        private void TakePrinter(bool isTake)
        {
            _isTakePrinter = isTake;
            OnButtonClick();
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
    }
}