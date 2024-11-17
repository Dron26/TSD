using System;
using CodeBase.Infrastracture;
using CodeBase.Infrastracture.AdditionalPanels;
using CodeBase.Infrastracture.Datas;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SwichEquipmentAction:MonoBehaviour
{
    [SerializeField] private Button _add;
    [SerializeField] private Button _delete;
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject _panel;
    [SerializeField] private DeleteEquipmentPanel _deletePanel;
    [SerializeField] private EquipmentAddPanel _addAddPanel;

    public Action OnBackButtonCLick;
    public Action OnClickAdd;
    public Action OnClickDelete;
    private SaveLoadService _saveLoadService;

    public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
    {
        _saveLoadService = saveLoadService;
        _deletePanel.Init( saveLoadService,  warningPanel);
        _addAddPanel.Init(saveLoadService, warningPanel);
        AddListeners();
    }

    public void Work()
    {
        _add.interactable = true;
        _delete.interactable = true;
    }

    private void OnCLickAddButton()
    {
        _addAddPanel.SwithState(true);
        _addAddPanel.Work();
        SwithState( false);
            
    }

    private void OnCLickDeleteButton()
    { 
        _deletePanel.SwithState(true);
        _deletePanel.Work();
        SwithState( false);
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
    private void AddListeners()
    {
        _add.onClick.AddListener(OnCLickAddButton);
        _delete.onClick.AddListener(OnCLickDeleteButton);
        _deletePanel.OnBackButtonCLick += ReternFromChildren;
        _addAddPanel.OnBackButtonCLick += ReternFromChildren;
        _backButton.onClick.AddListener(OnCLickBackButton);
    }

    private void OnDestroy()
    {
        _add.onClick.RemoveListener(OnCLickAddButton);
        _delete.onClick.RemoveListener(OnCLickDeleteButton);
        _backButton.onClick.RemoveListener(OnCLickBackButton);
        _deletePanel.OnBackButtonCLick += ReternFromChildren;
        _addAddPanel.OnBackButtonCLick += ReternFromChildren;
    }

    private void ReternFromChildren()
    {
        SwithState( true);
       Reset();
    }

    public void OnCLickBackButton()
    {
        SentLogMessage("Закрытие панели Запись ", "");
        SwithState(false);
        OnBackButtonCLick.Invoke();
    }

    public void Reset()
    {
        _deletePanel.Reset();
        _addAddPanel.Reset();
    }
}