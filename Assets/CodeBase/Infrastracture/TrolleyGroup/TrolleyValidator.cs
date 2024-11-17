using System;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.TrolleyGroup
{
    public class TrolleyValidator : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        //[SerializeField] private GameObject _buttonsPanel;
        //[SerializeField] private GameObject _viewport; 
        [SerializeField] private TMP_Text _number;
        //[SerializeField] private GameObject _freeTrolley;
        [SerializeField] private Button _buttonApply;
       // [SerializeField] private ScrollRect _skroll;

        [SerializeField] private Button _resetInput;
        [SerializeField] private Button _buttonCheck;
        [SerializeField] private Image _CheckUp;
        [SerializeField] private Image _CheckDown;
            // [SerializeField] private TMP_Text _employeeField;
        //[SerializeField] private TMP_Text _trolleyNumber;
        [SerializeField] private TMP_InputField _inputReturnField;
        private bool _isReseted;
        
        public Action OnTakeTrolley;
        
      // private GameObject _tempPanel;
        private List<Trolley> _trolleys;
        private SaveLoadService _saveLoadService;
      //  private List<GameObject> _freeTrolleys = new();
      //  private List<GameObject> _busyTrolleys = new();
        private Employee _employee;
        private Trolley _trolley;
        private List<int> _trolleysNumbers = new List<int>();
        private string _applyText = ": подтвердил выбор рохли";
        private int _numberTrolley;
        private bool _isButtonFilling;
        private WarningPanel _warningPanel;

        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _warningPanel = warningPanel;
            AddListeners();
        }

        public void Work()
        {
            _employee = _saveLoadService.Employee;
          
            
            _buttonApply.interactable = false;
            
            _inputReturnField.Select();
            _inputReturnField.ActivateInputField();
            _inputReturnField.interactable = true;
            _CheckDown.enabled = true;
            _CheckUp.enabled = false;
            //FillButtons();
            
            _buttonApply.interactable = false;
            _CheckDown.enabled = true;
            _CheckUp.enabled = false;
            _buttonCheck.interactable = false;
        }

        public void Reset()
        {
            _trolleys = new List<Trolley>();
          //  _freeTrolleys = new();
          //  _busyTrolleys = new();
            _isButtonFilling = false;
            _trolleysNumbers.Clear();
            _number.text = "";
            _trolley= new Trolley("");
                
            _isReseted = true;
            //_employeeField.text = "";
           // _trolleyNumber.text = "";
            _inputReturnField.text = null;
            _inputReturnField.interactable = true;
            _inputReturnField.ActivateInputField();
            _inputReturnField.Select();
            _buttonApply.interactable = false;
            _CheckDown.enabled = true;
            _CheckUp.enabled = false;
            _resetInput.interactable = false;
            _isReseted = false;
                
           // Destroy(_tempPanel);
        }

        
        private void ResetInput()
        {
            SentLogMessage(_employee.Login + "Выполнил сброс ввода QR тележки", "");
            Reset();
        }

        private void ValidateInput()
        {
            if (_isReseted == false)
            {
                CheckInput();
            }
        }

        private void CheckInput()
        {
            string text = _inputReturnField.text;
            _trolleys = _saveLoadService.GetTrolleys();
            
            if (text!= "")
            {
                _resetInput.interactable = true;
            }
            else
            {
                _resetInput.interactable = false;
            }
            
            foreach (Trolley trolley in _trolleys)
            {
                if (trolley.Number == text)
                {
                    if (trolley.Busy == true)
                    {
                        _saveLoadService.SetSelectedTrolley(trolley);
                        _warningPanel.ShowWindow(WindowNames.CanNotRegistredTrolley.ToString());
                        SentLogMessage(_employee.Login + ": отсканировал неверный QR", "");
                    }
                    else
                    {
                        _trolley= trolley;
                        _buttonApply.interactable = true;
                        _CheckDown.enabled = false;
                        _CheckUp.enabled = true;
                        _buttonCheck.interactable = false;
                        SentLogMessage(_employee.Login + ": отсканировал верный QR", "");
                    }
                }
            }
            
        }
        

        // private void FillButtons()
        // {
        //     _freeTrolley.gameObject.SetActive(true);
        //     _tempPanel = Instantiate(_buttonsPanel, _viewport.transform);
        //     _tempPanel.SetActive(true);
        //     _skroll.content = _tempPanel.GetComponent<RectTransform>();
        //
        //     SortTrolleys();
        //
        //     foreach (int number in _trolleysNumbers)
        //     {
        //         GameObject button = Instantiate(_freeTrolley, _tempPanel.transform);
        //
        //         TMP_Text text = button.GetComponentInChildren<TMP_Text>();
        //         text.text = number.ToString();
        //         button.GetComponent<Button>().onClick.AddListener(() => ShowInfo(text.text));
        //
        //         button.SetActive(true);
        //         _freeTrolleys.Add(button);
        //     }
        //
        //     
        //     
        //     _isButtonFilling = true;
        // }

        // private void SortTrolleys()
        // {
        //     
        //     
        //     foreach (Trolley trolley in _trolleys)
        //     {
        //         if (!trolley.Busy)
        //         {
        //             _trolleysNumbers.Add(Convert.ToInt32(trolley.Number));
        //         }
        //     }
        //     
        //     _trolleysNumbers.Sort();
        // }
        //
        // private void ShowInfo(string text)
        // {
        //
        //     for (int i = 0; i < _trolleys.Count; i++)
        //     {
        //         if (_trolleys[i].Number == text)
        //         {
        //             _trolley = _trolleys[i];
        //             _trolley.Number = _trolleys[i].Number ;
        //             _trolleyNumber = i;
        //             break;
        //         }
        //     }
        //     _employee = _saveLoadService.Employee;
        //     
        //     _number.text = _trolley.Number;
        //     _buttonApply.interactable = true;
        //     SentLogMessage(_employee.Login + " выбрал рохлю : " + _trolley.Number ,"");
        // }

        private void OnButtonClick()
        {
            _employee = _saveLoadService.Employee;

            _trolley.Busy = true;
            int index = _trolleys.IndexOf(_trolley);
            _saveLoadService.SetTrolley(_trolley);
            _employee.SetTrolley(_trolley);

            SentLogMessage(_employee.Login + _applyText +" "+ _employee.Trolley.Number , " Выдача рохли ");
            
            SentDataMessage(new SentData(" Выдача рохли ", _employee.Login, _employee.Password,
                "","", DateTime.Now.ToString(),_employee.Trolley.Number));
            
            SentData sentData = new SentData(" Выдача рохли ", _employee.Login, _employee.Password, "", "", "", "");
            sentData.SetTrolleyNumber(_employee.Trolley.Number);
            SentDataTrolleyMessage(sentData);
            
            _saveLoadService.SetCurrentEmployee(_employee);
            _saveLoadService.SaveDatabase();
            _buttonApply.interactable = false;

            OnTakeTrolley?.Invoke();
        }

        public void SwithState(bool state)
        {
            _panel.SetActive(state);
        }

        private void AddListeners()
        {
            _buttonApply.onClick.AddListener(OnButtonClick);
            _buttonCheck.onClick.AddListener(ValidateInput);
            _inputReturnField.onValueChanged.AddListener(delegate { CheckInputField(); });
            _resetInput.onClick.AddListener(ResetInput);
        }

       

        private void RemuveListeners()
        {
            _buttonApply.onClick.RemoveListener(OnButtonClick);
            _buttonCheck.onClick.RemoveListener(ValidateInput);
            _inputReturnField.onValueChanged.AddListener(delegate { CheckInputField(); });
            _resetInput.onClick.AddListener(ResetInput);
        }

        private void CheckInputField()
        {
            if(_inputReturnField.text.Length > 0)
            {
                _buttonCheck.interactable = true;
                _resetInput.interactable = true;
            }
            else
            {
                _buttonCheck.interactable = false;
                _resetInput.interactable = true;
            }
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
    }
}