using System;
using System.IO;
using System.IO.Ports;
using CodeBase.Infrastracture.Datas;
using UnityEngine;
using UnityEngine.UI;

public class ArduinoCommunication : MonoBehaviour
{
    [SerializeField] private ConsolePanel _consolePanel;
    
    private int _comAddress;
    private int _baudeRate;
    private int _readTimeout;

    private SerialPort serialPort;
    private string lastMessage = "";
    private bool _isRunning = false;
    private bool isConsoleOpen = false;
    private SaveLoadService _saveLoadService;
    public void Init(SaveLoadService saveLoadService )
    {
        _saveLoadService = saveLoadService;
        AddListeners();
    }

    private void CheckSerialPort()
    {
        if (TrySetSerialPort())
        {
            SentLogMessage("Соединение с Arduino установлено.");
        }
        else
        {
            SentLogMessage(
                "Не удалось установить соединение с Arduino. Проверьте наличие подключения и правильность настроек COM-порта.");
        }
    }

    private bool TrySetSerialPort()
    {
        _isRunning = false;

        try
        {
            serialPort = new SerialPort("COM" + _comAddress.ToString(), _baudeRate);
            serialPort.ReadTimeout = _readTimeout; // Устанавливаем таймаут
            serialPort.Open();
            Debug.Log("Подключение к Arduino установлено.");
            SentLogMessage("Подключение к Arduino установлено.");
            _isRunning = true;
        }
        catch (IOException e)
        {
            Debug.LogError("Ошибка при подключении к Arduino: " + e.Message);
            SentLogMessage("Ошибка при подключении к Arduino: " + e.Message);

        }

        return _isRunning;
    }

    void OnApplicationQuit()
    {
        _isRunning = false;

        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Подключение к Arduino закрыто.");
            SentLogMessage("Подключение к Arduino закрыто.");
        }

        RemuveListeners();
    }

    public void Send(string text)
    {
        if (_isRunning)
        {
            try
            {
                serialPort.WriteLine(text); // Отправка команды на Arduino
                Debug.Log("Команда отправлена на Arduino: " + text);
                SentLogMessage("Команда отправлена на Arduino: " + text);
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка отправки команды: " + ex.Message);
                SentLogMessage("Ошибка отправки команды: " + ex.Message);
            }
        }
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen && serialPort.BytesToRead > 0)
        {
            try
            {
                string message = serialPort.ReadLine(); 
                _saveLoadService.TakeMessage(message);// Чтение сообщения от Arduino
                Debug.Log("Сообщение от Arduino: " + message);
                SentLogMessage("Сообщение от Arduino: " + message);
            }
            catch (TimeoutException)
            {
                // Игнорируем таймауты, так как они ожидаемы
            }
            catch (IOException e)
            {
                Debug.LogError("Ошибка ввода-вывода: " + e.Message);
                SentLogMessage("Ошибка ввода-вывода: " + e.Message);
            }
        }
    }

    private void SetComData(ComData data)
    {
        _comAddress = data.ComAddress;
        _baudeRate = data.BaudeRate;
        _readTimeout = data.ReadTimeout;
        CheckSerialPort();
    }

    private void AddListeners()
    {
        _saveLoadService.OnSetComData += SetComData;
        _saveLoadService.OnConsoleOpen += SetConsoleState;

        // _saveLoadService.OnSetComData += CheckSerialPort;
    }

    private void RemuveListeners()
    {
        //_saveLoadService.OnSetComData -= SetComData;
        _saveLoadService.OnConsoleOpen -= SetConsoleState;
    }

    private void SetConsoleState(bool state)
    {
        isConsoleOpen = state;
    }


    private void SentLogMessage(string message)
    {
        _saveLoadService.SentLogInfo(message, "");

        if (isConsoleOpen)
        {
            _consolePanel.LogMessage(message);
        }
    }
}