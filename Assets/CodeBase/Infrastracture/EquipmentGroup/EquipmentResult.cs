using System;
using System.Collections;
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
        [SerializeField] private Button _buttonApplyResult;

        private Data _data;
        private SaveLoadService _saveLoadService;
        private bool  _isOpenBox=false;
        private bool  isActive=false;
        public event Action ApplyEquipment;

        public void Init(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            AddListeners();
        }

        public void Work()
        {
            isActive = true;
            Reset();
        }

        public void SetData()
        {
            _data = _saveLoadService.Database;
            SetText();
        }

        public void WhaiteToOpenBox()
        {
                StartCoroutine(Whait());
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
            
            if (!state)
            {
                StopCoroutine(Whait());
            }
        }
      
        private IEnumerator Whait()
        {
            Debug.Log("Whait");
            
            _saveLoadService.SendCommandArduino("OPEN"+_data.Employee.Box.Key);

            while (!_isOpenBox)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _isOpenBox = true;
                }
                yield return null;
            }
            Debug.Log("Opened1ffff");
            
            
            ApplyEquipment?.Invoke();
        }
        private void ArduinoListen(string message)
        {
            if (isActive)
            {
                string messag = message[..6];
                Debug.Log(message);
            
                if (messag == "Closed" )
                {
                    _data = _saveLoadService.Database;
                    Debug.Log("ArduinoListen");
                    Debug.Log(message);

                    string key = _data.Employee.Box.Key;
                
                    if (message == "Closed"+key)
                    {
                        _isOpenBox = true;
                        Debug.Log(message);
                    }
                }

            }
        }
        
        private void AddListeners()
        {
            _saveLoadService.OnArduinoAnswer+=ArduinoListen;
        }

        private void RemuveListeners()
        {
            _saveLoadService.OnArduinoAnswer-=ArduinoListen;
        }

        private void OnResultButtonClick()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            _employee.text = "";
            _key.text = "";
            _tsd.text = "";
            _printer.text = "";
        }

        private void OnDisable()
        {
            RemuveListeners();
            Debug.Log("OnDisable");
        }
    }
}