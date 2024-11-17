using System;
using System.Collections;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverdueEquipmentChecker : MonoBehaviour
{
    [SerializeField] private GameObject _overdueEmployeeList;
    [SerializeField] private GameObject _itemLogin;
    [SerializeField] private GameObject _overdueEmployeeViewport;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _sendButton;
    [SerializeField] private GameObject _panel;
    [SerializeField] private ScrollRect _overdueEmployeeScroll;

    private SaveLoadService _saveLoadService;
    private List<Employee> _employees = new();
    private GameObject _tempPanelOverdueEmployee;
    private List<Employee> _overdueEmployees;
    public void Init(SaveLoadService saveLoadService)
    {
        _saveLoadService = saveLoadService;
        AddListeners();
    }

    public void Work()
    {
        SwithState(true);
        FillOverdueEmployeeInfo();
    }
    
    private void FillOverdueEmployeeInfo()
    {
        if (_tempPanelOverdueEmployee != null)
        {
            Destroy(_tempPanelOverdueEmployee);
        }

        _tempPanelOverdueEmployee = Instantiate(_overdueEmployeeList, _overdueEmployeeViewport.transform);
        _tempPanelOverdueEmployee.SetActive(true);
        _overdueEmployeeScroll.content = _tempPanelOverdueEmployee.transform.GetComponent<RectTransform>();
        _overdueEmployeeScroll.verticalScrollbar.value = 1;

         _overdueEmployees = GetOverdueEmployees();
        
         
         
        foreach (Employee employee in _overdueEmployees)
        {
            GameObject overdueEmployee = Instantiate(_itemLogin, _tempPanelOverdueEmployee.transform);
            TMP_Text employeeLogin =
                overdueEmployee.GetComponent<ItemReternChecker>().GetLogin().GetComponentInChildren<TMP_Text>();
            employeeLogin.text = employee.Login;
            TMP_Text employeeKey =
                overdueEmployee.GetComponent<ItemReternChecker>().GetKey().GetComponentInChildren<TMP_Text>();
            employeeKey.text = $"Номер ключа/сканера: {employee.Box.Key}{employee.Box.Equipment.SerialNumber[^4..]}";

            DateTime date = employee.GetDateTakenEquipment();
            
            TMP_Text employeeDate =
                overdueEmployee.GetComponent<ItemReternChecker>().GetTime().GetComponentInChildren<TMP_Text>();
            employeeDate.text = $"Взял: {date}";
            TMP_Text employeeBadDate =
                overdueEmployee.GetComponent<ItemReternChecker>().GetBadTime().GetComponentInChildren<TMP_Text>();
            employeeBadDate.text = $"Просрочено: {GetDaysOverdue(date)} дней";
            
        }

        Canvas.ForceUpdateCanvases();
        _overdueEmployeeScroll.verticalNormalizedPosition = 0f;
    }

    public List<Employee> GetOverdueEmployees()
    {
        _employees = _saveLoadService.GetEmployees();
         _overdueEmployees = new();
        int maxHours = _saveLoadService.MaxEmployeeHours;

        foreach (Employee employee in _employees)
        {
            if (employee.HaveBox && employee.GetDateTakenEquipment() != DateTime.MinValue)
            {
                int hoursPassed = GetHoursPassedSince(employee.GetDateTakenEquipment());
                if (hoursPassed > maxHours)
                {
                    _overdueEmployees.Add(employee);
                }
            }
        }

        return _overdueEmployees;
    }

    private int GetHoursPassedSince(DateTime dateTakenEquipment)
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeSpan = currentTime - dateTakenEquipment;
        return (int)timeSpan.TotalHours;
    }

    private int GetDaysOverdue(DateTime dateTakenEquipment)
    {
        int hoursPassed = GetHoursPassedSince(dateTakenEquipment);
        int daysOverdue = (hoursPassed - 14) / 24; 
        return daysOverdue;
    }

    private void AddListeners()
    {
        _backButton.onClick.AddListener(Reset);
        _sendButton.onClick.AddListener(Send);
    }

    private void Send()
    {
        _saveLoadService.SendOverdueInfo();
    }

    public void Reset()
    {
        if (_tempPanelOverdueEmployee != null)
        {
            Destroy(_tempPanelOverdueEmployee);
        }

        SwithState(false);
    }

    private void OnDisable()
    {
        _sendButton.onClick.RemoveListener(Send);
        _backButton.onClick.RemoveListener(Reset);
    }

    public void SwithState(bool state)
    {
        _panel.SetActive(state);
    }
}