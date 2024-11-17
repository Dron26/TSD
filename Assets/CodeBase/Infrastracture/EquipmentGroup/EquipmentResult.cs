using System;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.EquipmentGroup
{
    public class EquipmentResult : MonoBehaviour
    {
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private TMP_Text _employee;
        [SerializeField] private TMP_Text _key;
        [SerializeField] private TMP_Text _tsd;
        [SerializeField] private TMP_Text _printer;

        private Data _data;
        private SaveLoadService _saveLoadService;

        public void Init(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }

        public void Work()
        {
            Reset();
        }

        public void SetData()
        {
            _data = _saveLoadService.Database;
            SwithState(true);
            SetText();
        }

        private void SetText()
        {
            _employee.text = _data.Employee.Login;
            _key.text = _data.Employee.Box.Key;
            _tsd.text = _data.Employee.Equipment.SerialNumber[^4..];
            
            if (_data.Employee.HavePrinter)
            {
                _printer.text = _data.Employee.Printer.SerialNumber[^4..];
            }
        }

        public void SwithState(bool state)
        {
            _resultPanel.gameObject.SetActive(state);
        }
      
        public void Reset()
        {
            _employee.text = "";
            _key.text = "";
            _tsd.text = "";
            _printer.text = "";
        }
    }
}