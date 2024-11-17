using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastracture;
using CodeBase.Infrastracture.Datas;
using CodeBase.Infrastracture.EquipmentGroup;
using CodeBase.Infrastracture.TrolleyGroup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeleteEquipmentPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _equipmentPanel;
    [SerializeField] private GameObject _printerPanel;
    [SerializeField] private GameObject _trolleyPanel;
    [SerializeField] private GameObject _equipmentList;
    [SerializeField] private GameObject _printertList;
    [SerializeField] private GameObject _trolleyList;
    [SerializeField] private GameObject _viweport;
    [SerializeField] private GameObject _mainItem;
    [SerializeField] private GameObject _mainTrolleyItem;
    [SerializeField] private GameObject _mainPrinterItem;
    
    [SerializeField] private Button _equipmentButton;
    [SerializeField] private Button _printerButton;
    [SerializeField] private Button _trolleyButton;
    [SerializeField] private Button _resetEquipmentInputTextButton;
    [SerializeField] private Button _resetPrinterInputTextButton;
    [SerializeField] private Button _resetTrolleyInputTextButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _okButton;
    [SerializeField] private Button _nokButton;
    [SerializeField] private Button _boxDeleteButton;
    [SerializeField] private Button _printerDeleteButton;
    [SerializeField] private Button _trolleyDeleteButton;
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private TMP_Text _boxText;
    [SerializeField] private TMP_Text _equipmentText;
    [SerializeField] private TMP_Text _printerText;
    [SerializeField] private TMP_Text _trolleyText;
    [SerializeField] private GameObject _trolleyTextPanel;
    [SerializeField] private GameObject _printerTextPanel;
    [SerializeField] private GameObject _equipmentTextPanel;

    public Action OnBackButtonCLick;
    private Employee _registeredEmployee;
    private SaveLoadService _saveLoadService;

    private List<Box> _boxes = new();
    private List<Box> _busyBoxes = new();
    private List<Printer> _printers = new();
    private List<Trolley> _trolleys = new();

    private GameObject _tempEquipmentGroup;
    private GameObject _tempPrinterGroup;
    private GameObject _tempTrolleyGroup;
    private List<GameObject> _equipmentGroups = new();
    private List<GameObject> _printerGroups = new();
    private List<GameObject> _trolleyGroups = new();
    private WarningPanel _warningPanel;

    private bool _isReseted;
    private Box _selectedForDeleteBox;
    private Trolley _selectedForDeleteTrolley;
    private Printer _selectedForDeletePrinter;
    Dictionary<string, Equipment> _freeBoxes = new ();

    public delegate void ActionWithTextNumber(string textNumber);

    private List<int> _freeTrolleys = new();
    private List<Trolley> _busyTrolleys = new();
    private List<Printer> _freePrinters = new();

    public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
    {
        _saveLoadService = saveLoadService;
        _warningPanel = warningPanel;
        _tempEquipmentGroup = Instantiate(_equipmentList, _viweport.transform);
        _tempPrinterGroup = Instantiate(_printertList, _viweport.transform);
        _tempTrolleyGroup = Instantiate(_trolleyList, _viweport.transform);
        AddListeners();
    }

    public void Work()
    {
        FillEquipmentList();
        FillTrolleyList();
        FillPrinterList();
        
        _equipmentPanel.SetActive(false);
        _trolleyPanel.SetActive(false);

        _okButton.gameObject.SetActive(false);
        _nokButton.gameObject.SetActive(false);
        _trolleyTextPanel.SetActive(false);
        _equipmentTextPanel.SetActive(false);

        _equipmentButton.interactable = true;
        _printerButton.interactable = true;
        _trolleyButton.interactable = true;
    }

    public void Reset()
    {
        _isReseted = true;
        _freeBoxes.Clear();
        _freePrinters.Clear();
        _freeTrolleys.Clear();
        _busyBoxes.Clear();
        _busyTrolleys.Clear();
        _boxText.text = "";
        _printerText.text= "";
        _equipmentText.text = "";
        _trolleyText.text = "";

        foreach (var gameObject in _equipmentGroups)
        {
            Button button = gameObject.GetComponent<Button>();
            button.onClick.RemoveListener(() => SelectBox(""));
            Destroy(gameObject);
        }
        foreach (var gameObject in _trolleyGroups)
        {
            Button button = gameObject.GetComponent<Button>();
            button.onClick.RemoveListener(() => SelectTrolley(""));
            Destroy(gameObject);
        }
        foreach (var gameObject in _printerGroups)
        {
            Button button = gameObject.GetComponent<Button>();
            button.onClick.RemoveListener(() => SelectPrinter(""));
            Destroy(gameObject);
        }
        
        _equipmentGroups.Clear();
        _trolleyGroups.Clear();
        _printerGroups.Clear();
        _isReseted = false;
    }
    private void FillEquipmentList()
    {
        _tempEquipmentGroup.SetActive(true);

        _boxes = _saveLoadService.GetBoxes();
        SortButtons();

        foreach (var info in _freeBoxes)
        {
            GameObject newBox = Instantiate(_mainItem, _tempEquipmentGroup.transform);
            TMP_Text textKey = newBox.GetComponent<ItemMain>().GetBox().GetComponentInChildren<TMP_Text>();
            TMP_Text textEquipment =
                newBox.GetComponent<ItemMain>().GetEquipment().GetComponentInChildren<TMP_Text>();
            Button button = newBox.GetComponent<Button>();
            button.onClick.AddListener(() => SelectBox(textKey.text));
            textKey.text = info.Key.ToString();
            textEquipment.text = info.Value.SerialNumber[^4..];
            _equipmentGroups.Add(newBox);
        }

        _tempEquipmentGroup.SetActive(false);
    }

    private void SortButtons()
    {
        foreach (Box box in _boxes)
        {
            if (!box.Busy)
            {
                if (_freeBoxes.ContainsKey(box.Key))
                {
                    _warningPanel.ShowWindow(WindowNames.OnHaveDuplicate.ToString());
                }
                else
                {
                    _freeBoxes.Add(box.Key, box.Equipment);
                }
            }
            else
            {
                _busyBoxes.Add(box);
            }
        }

        _freeBoxes = _freeBoxes.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
    }
    
    private void FillPrinterList()
    {
         
        _tempPrinterGroup.SetActive(true);
        _printers = _saveLoadService.GetPinters();
        SortPrinters();

        foreach (Printer printer in _freePrinters)
        {
            GameObject newPrinter = Instantiate(_mainPrinterItem, _tempPrinterGroup.transform);
            Button button = newPrinter.GetComponent<Button>();
            TMP_Text textNumber = newPrinter.GetComponentInChildren<TMP_Text>();
            button.onClick.AddListener(() => SelectPrinter(textNumber.text));
            textNumber.text = printer.SerialNumber;
            _trolleyGroups.Add(newPrinter);
        }

        _tempTrolleyGroup.SetActive(false);
    }

    private void SortPrinters()
    {
        foreach (Printer printer in _printers)
        {
            if (!printer.Busy)
            {
                _freePrinters.Add(printer);
            }
        }
    }
    
    private void FillTrolleyList()
    {
        
        _tempTrolleyGroup.SetActive(true);
        _trolleys = _saveLoadService.GetTrolleys();
        SortTrolleys();

        foreach (int number in _freeTrolleys)
        {
            GameObject newTrolley = Instantiate(_mainTrolleyItem, _tempTrolleyGroup.transform);
            Button button = newTrolley.GetComponent<Button>();
            TMP_Text textNumber = newTrolley.GetComponentInChildren<TMP_Text>();
            button.onClick.AddListener(() => SelectTrolley(textNumber.text));
            textNumber.text = number.ToString();
            _trolleyGroups.Add(newTrolley);
        }

        _tempTrolleyGroup.SetActive(false);
    }

    private void SortTrolleys()
    {
        foreach (Trolley trolley in _trolleys)
        {
            if (!trolley.Busy)
            {
                _freeTrolleys.Add(Convert.ToInt32(trolley.Number));
            }
            else
            {
                _busyTrolleys.Add(trolley);
            }
        }
    }

    private void SelectBox(string textKey)
    {
        ChangeStateAction(SelectBox);
        _selectedForDeleteBox=_boxes.FirstOrDefault(x => x.Key == textKey);
        SentLogMessage("Выбран ящик " + _selectedForDeleteBox.Key, "");
        SetBoxInfo();
    }

    private void SelectPrinter(string textNumberText)
    {
        ChangeStateAction(SelectPrinter);

        _selectedForDeletePrinter = _printers.FirstOrDefault(x => x.SerialNumber == textNumberText);
        SentLogMessage("Выбран принтер " + _selectedForDeletePrinter.SerialNumber, "");
        SetPrinterInfo();
    }

    private void SelectTrolley(string textNumber)
    {
        ChangeStateAction(SelectTrolley);

        _selectedForDeleteTrolley = _trolleys.FirstOrDefault(x=>x.Number==textNumber);
        SentLogMessage("Выбрана рохля " + _selectedForDeleteTrolley.Number, "");
        SetTrolleyInfo();
    }

    private void SetBoxInfo()
    {
        _boxText.text = _selectedForDeleteBox.Key;
        _equipmentText.text = _selectedForDeleteBox.Equipment.SerialNumber[^4..];
    }
    
    private void SetPrinterInfo()
    {
        _printerText.text = _selectedForDeletePrinter.SerialNumber;
    }
    
    private void SetTrolleyInfo()
    {
        _trolleyText.text = _selectedForDeleteTrolley.Number;
    }

    private void DeleteBox()
    {
        _okButton.gameObject.SetActive(true);
        _nokButton.gameObject.SetActive(false);
        SentLogMessage("Удачная попытка удаления " + _selectedForDeleteBox.Key, "Удаление");
        _registeredEmployee = _saveLoadService.Employee;
        string action = "Выполнил удаление ящика" + " " + _selectedForDeleteBox.Key;
        string Login = _registeredEmployee.Login;
        string Pass = _registeredEmployee.Password;
        string Key = "*";
        string ShortNumber = "*";
        string Time = DateTime.Now.ToString();

        SentData sentData = new SentData(action, Login, Pass, Key, ShortNumber, Time, "");

        SentDataMessage(sentData);

        _saveLoadService.RemoveBox(_selectedForDeleteBox);

        Reset();
        Work();
    }
    
    private void DeletePrinter()
    {
        _okButton.gameObject.SetActive(true);
        _nokButton.gameObject.SetActive(false);
        
        SentLogMessage("Удачная попытка удаления " + _selectedForDeletePrinter.SerialNumber, "Удаление");
        _registeredEmployee = _saveLoadService.Employee;
        string action = "Выполнил удаление принтера" + " " + _selectedForDeletePrinter.SerialNumber;
        string Login = _registeredEmployee.Login;
        string Pass = _registeredEmployee.Password;
        string Key = "*";
        string ShortNumber = "*";
        string Time = DateTime.Now.ToString();

        SentData sentData = new SentData(action, Login, Pass, Key, ShortNumber, Time, "");

        SentDataMessage(sentData);

        _saveLoadService.RemovePrinter(_selectedForDeletePrinter);

        Reset();
        Work();
    }
    
    private void DeleteTrolley()
    {
        _okButton.gameObject.SetActive(true);
        _nokButton.gameObject.SetActive(false);
        SentLogMessage("Удачная попытка удаления " + _selectedForDeleteTrolley.Number, "Удаление");
        _registeredEmployee = _saveLoadService.Employee;
        string action = "Выполнил удаление тележки" + " " + _selectedForDeleteTrolley.Number;
        string Login = _registeredEmployee.Login;
        string Pass = _registeredEmployee.Password;
        string Key = "*";
        string ShortNumber = "*";
        string Time = DateTime.Now.ToString();

        SentData sentData = new SentData(action, Login, Pass, Key, ShortNumber, Time, "");

        SentDataMessage(sentData);

        _saveLoadService.RemoveTrolley(_selectedForDeleteTrolley);

        Reset();
        Work();
    }
    
    private void ChangeStateAction(ActionWithTextNumber action)
    {
        if (action ==SelectTrolley )
        {
            _trolleyPanel.SetActive(true);
            _equipmentPanel.SetActive(false);
            _printerPanel.SetActive(false);
            _trolleyDeleteButton.interactable = true;
        }
        else if (action == SelectBox)
        {
            _trolleyPanel.SetActive(false);
            _equipmentPanel.SetActive(true);
            _printerPanel.SetActive(false);
            _boxDeleteButton.interactable = true;
        }
        else if (action == SelectPrinter)
        {
            _trolleyPanel.SetActive(false);
            _equipmentPanel.SetActive(false);
            _printerPanel.SetActive(true);
            _printerDeleteButton.interactable = true;
        }
    }
    
    private void OnApplyAddedEmployee()
    {
        _okButton.gameObject.SetActive(false);
        _nokButton.gameObject.SetActive(false);
        Reset();
        Work();
    }
    
    private void SelectTrolleyGroup()
    {
        SentLogMessage("-> Удалить рохлю ", "");
        _trolleyTextPanel.SetActive(true);
        _printerTextPanel.SetActive(false);
        _equipmentTextPanel.SetActive(false);
        _tempTrolleyGroup.SetActive(true);
        _tempEquipmentGroup.SetActive(false);
        _tempPrinterGroup.SetActive(false);
        _scroll.content = _tempTrolleyGroup.GetComponent<RectTransform>();
        _scroll.verticalScrollbar.value = 1;    }

    private void SelectEquipmentGroup()
    {
        SentLogMessage("-> Удалить оборудование ", "");
        _trolleyTextPanel.SetActive(false);
        _printerTextPanel.SetActive(false);
        _equipmentTextPanel.SetActive(true);
        _tempEquipmentGroup.SetActive(true);
        _tempTrolleyGroup.SetActive(false);
        _tempPrinterGroup.SetActive(false);
        _scroll.content = _tempEquipmentGroup.GetComponent<RectTransform>();
        _scroll.verticalScrollbar.value = 1;
    }
    
    private void SelectPrinterGroup()
    {
        SentLogMessage("-> Удалить принтер ", "");
        _trolleyTextPanel.SetActive(false);
        _printerTextPanel.SetActive(true);
        _equipmentTextPanel.SetActive(false);
        _tempTrolleyGroup.SetActive(false);
        _tempEquipmentGroup.SetActive(false);
        _tempPrinterGroup.SetActive(true);
        _scroll.content = _tempPrinterGroup.GetComponent<RectTransform>();
        _scroll.verticalScrollbar.value = 1;    
    }

    private void ResetEquipmentInput()
    {
        SentLogMessage("Выполнен сброс при вводе оборудования", "сброс ящик/cканер");
        Reset();
        Work();
    }
    
    private void ResetPrinterInput()
    {
        SentLogMessage("Выполнен сброс при вводе принтера", "сброс принтера");
        Reset();
        Work();
    }

    private void ResetTrolleyInput()
    {
        SentLogMessage("Выполнен сброс рохли", "сброс рохли");
        Reset();
        Work();
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
        _resetEquipmentInputTextButton.onClick.AddListener(ResetEquipmentInput);
        _resetPrinterInputTextButton.onClick.AddListener(ResetPrinterInput);
        _resetTrolleyInputTextButton.onClick.AddListener(ResetTrolleyInput);
        _okButton.onClick.AddListener(OnApplyAddedEmployee);
        _nokButton.onClick.AddListener(OnApplyAddedEmployee);
        _boxDeleteButton.onClick.AddListener(DeleteBox);
        _printerDeleteButton.onClick.AddListener(DeletePrinter);
        _trolleyDeleteButton.onClick.AddListener(DeleteTrolley);
        _equipmentButton.onClick.AddListener(SelectEquipmentGroup);
        _printerButton.onClick.AddListener(SelectPrinterGroup);
        _trolleyButton.onClick.AddListener(SelectTrolleyGroup);
    }
    
    private void RemuveListeners()
    {
        _backButton.onClick.RemoveListener(OnCLickBackButton);
        _resetEquipmentInputTextButton.onClick.RemoveListener(ResetEquipmentInput);
        _resetPrinterInputTextButton.onClick.RemoveListener(ResetPrinterInput);
        _resetTrolleyInputTextButton.onClick.RemoveListener(ResetTrolleyInput);
        _okButton.onClick.RemoveListener(OnApplyAddedEmployee);
        _nokButton.onClick.RemoveListener(OnApplyAddedEmployee);
        _boxDeleteButton.onClick.RemoveListener(DeleteBox);
        _printerDeleteButton.onClick.RemoveListener(DeletePrinter);
        _trolleyDeleteButton.onClick.RemoveListener(DeleteTrolley);
        _trolleyButton.onClick.RemoveListener(SelectTrolleyGroup);
        _printerButton.onClick.RemoveListener(SelectPrinterGroup);
        _equipmentButton.onClick.RemoveListener(SelectEquipmentGroup);
    }
    
    public void OnCLickBackButton()
    {
        SentLogMessage("<- Назад ", "");
        SwithState(false);
        OnBackButtonCLick.Invoke();
    }
}