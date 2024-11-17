using System;
using System.Collections.Generic;
using System.IO;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.AdditionalPanels
{
    public class HistoryPanel : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _showData;
        [SerializeField] private Button _showLog;
        [SerializeField] private Button _showEmployees;
        [SerializeField] private Button _clear;
        [SerializeField] private Button _backButton;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private TMP_Text _info;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _priviousButton;
        [SerializeField] private int _startLine;
        [SerializeField] private int _endLine;
        
        private string _selectedFile;
        public Action OnBackButtonCLick;
        private string _equipmentName = " - рохля №";
        private string textBase = "Текущее состояние ";
        private SaveLoadService _saveLoadService;
        private int _maxLength = 400;
        List<string> _logs = new List<string>();

        public void Init(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            AddListeners();
            _info.gameObject.SetActive(false);
            _startLine = 0;
            _endLine = _maxLength;
        }

        public void Reset()
        {
            _infoText.text = "";
            _showData.gameObject.SetActive(true);
            _showLog.gameObject.SetActive(true);
            _showData.interactable = true;
            _showLog.interactable = true;
            _startLine = 0;
            _endLine = _maxLength;
        }

        public void ShowEmployees()
        {
            Reset();

            _showData.interactable = true;
            _showLog.interactable = true;
            _showEmployees.interactable = true;

            SentLogMessage("-> Сотрудники");

            _infoText.text = "";

            _panel.SetActive(true);
            _info.gameObject.SetActive(true);
            _info.text = textBase;
            List<Employee> _employees = _saveLoadService.Database.GetEmployees();

            foreach (var employee in _employees)
            {
                string employeeInfo = "";

                if (employee.HaveBox)
                {
                    employeeInfo = employee.Login + "  ключ " + employee.Box.Key + "  " + " ТСД " +
                                   employee.Equipment.SerialNumber[^4..];
                    _infoText.text += employeeInfo + "\n";
                }

                if (employee.HaveTrolley)
                {
                    if (employee.HaveBox)
                    {
                        employeeInfo += _equipmentName + employee.Trolley.Number;
                    }
                    else
                    {
                        employeeInfo = employee.Login + _equipmentName + employee.Trolley.Number;
                    }
                    _infoText.text += employeeInfo + "\n";
                }

                
            }

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
        
        private void OnClickNextLog()
        {
            _startLine = _endLine;
            _endLine += _maxLength;

            if (_selectedFile != null)
            {
                string filePath = Path.Combine(Application.persistentDataPath, _selectedFile);
                if (File.Exists(filePath))
                {
                    List<string> lastLines = ReadLastLines(filePath, _endLine);
                    if (_endLine > lastLines.Count)
                    {
                        _endLine = lastLines.Count;
                    }
                }
                ShowInfo(_selectedFile);
            }
        }

        private void OnClickPreviousLog()
        {
            if (_startLine == 0)
            {
                _endLine = _startLine + 1;
            }
            else
            {
                _endLine = _startLine;
            }

            _startLine -= _maxLength;

            if (_startLine < 0)
            {
                _startLine = 0;
            }

            if (_selectedFile != null)
            {
                ShowInfo(_selectedFile);
            }
        }

        public void ShowInfo(string dataFileName)
        {
            if (string.IsNullOrEmpty(dataFileName))
            {
                Debug.LogWarning("File name is null or empty.");
                return;
            }

            _selectedFile = dataFileName;
            SentLogMessage("->  " + dataFileName);
            _infoText.text = "";
            _panel.SetActive(true);
            _info.gameObject.SetActive(false);
            string filePath = Path.Combine(Application.persistentDataPath, dataFileName);

            if (File.Exists(filePath))
            {
                List<string> lastLines = ReadLastLines(filePath, _maxLength);
                foreach (var line in lastLines)
                {
                    _infoText.text += line + "\n";
                }

                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
            else
            {
                Debug.LogWarning("Log file does not exist.");
                _infoText.text = "Файл не найден.";
            }
        }
        private List<string> ReadLastLines(string filePath, int numberOfLines)
        {
            List<string> lastLines = new List<string>();
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (lastLines.Count >= numberOfLines)
                        {
                            lastLines.RemoveAt(0);
                        }
                        lastLines.Add(line);
                    }
                }
            }
            return lastLines;
        }
        
        public void SwithAdminState(bool state)
        {
            _panel.SetActive(state);
        }

        public void SwithManagerState(bool state)
        {
            _showData.gameObject.SetActive(false);
            _showLog.gameObject.SetActive(false);
            _panel.SetActive(state);
        }

        public void OnCLickBackButton()
        {
            SentLogMessage("<- Назад");
            Reset();
            SwithAdminState(false);
            _info.gameObject.SetActive(false);
            OnBackButtonCLick?.Invoke();
            _panel.SetActive(false);
        }

        private void SentLogMessage(string message)
        {
            _saveLoadService.SentLogInfo(message, "");
        }
        private void ShowDataInfo() => ShowInfo(Const.DataInfo);
        private void ShowLogInfo() => ShowInfo(Const.LogInfo);
        private void AddListeners()
        {
            _showData.onClick.AddListener(ShowDataInfo);
            _showLog.onClick.AddListener(ShowLogInfo);
            _showEmployees.onClick.AddListener(ShowEmployees);
            _clear.onClick.AddListener(Reset);
            _backButton.onClick.AddListener(OnCLickBackButton);
            _nextButton.onClick.AddListener(OnClickNextLog);
            _priviousButton.onClick.AddListener(OnClickPreviousLog);
        }

        private void RemuveListeners()
        {
            _showData.onClick.RemoveListener(ShowDataInfo);
            _showLog.onClick.RemoveListener(ShowLogInfo);
            _showEmployees.onClick.RemoveListener(ShowEmployees);
            _clear.onClick.RemoveListener(Reset);
            _backButton.onClick.RemoveListener(OnCLickBackButton);
            _nextButton.onClick.RemoveListener(OnClickNextLog);
            _priviousButton.onClick.RemoveListener(OnClickPreviousLog);
        }
        
        private void OnDisable()
        {
            RemuveListeners();
        }
    }
}