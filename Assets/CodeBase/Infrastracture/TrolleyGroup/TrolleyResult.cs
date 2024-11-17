using System;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.TrolleyGroup
{
    public class TrolleyResult : MonoBehaviour
    {
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private TMP_Text _employeeField;
        [SerializeField] private TMP_Text _trollyNumber;

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
            _employeeField.text = _saveLoadService.Employee.Login;
            _trollyNumber.text = _saveLoadService.Employee.Trolley.Number;
        }

        public void SwithState(bool state)
        {
            _resultPanel.gameObject.SetActive(state);
        }

        public void Reset()
        {
            _employeeField.text = "";
            _trollyNumber.text = "";
        }
    }
}