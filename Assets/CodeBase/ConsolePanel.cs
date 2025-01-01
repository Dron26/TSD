using System;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsolePanel : MonoBehaviour
{
    [SerializeField] private Button _apply;
    
    [SerializeField] private TMP_InputField _inputComAddress;
    [SerializeField] private TMP_InputField _inputBaudeRate;
    [SerializeField] private TMP_InputField _inputReadTimeout;

    private SaveLoadService _saveLoadService;
    // Ссылка на Text или TextMeshPro
    public Text consoleText; // Для обычного Text
    public TextMeshProUGUI consoleTextMeshPro; // Для TextMeshPro
    // Очередь для хранения сообщений
    private Queue<string> messageQueue = new Queue<string>();

    // Максимальное количество сообщений в консоли
    public int maxMessages = 10;

    public void Init(SaveLoadService saveLoadService)
    {
        _saveLoadService = saveLoadService;
        AddListeners();
        // Очищаем консоль при старте
        ClearConsole();
    }

    private void OnDisable()
    {
        ClearConsole();
    }

    // Метод для добавления сообщения в консоль
    public void LogMessage(string message)
    {
        // Добавляем сообщение в очередь
        messageQueue.Enqueue(message);

        // Если количество сообщений превышает максимальное, удаляем самое старое
        if (messageQueue.Count > maxMessages)
        {
            messageQueue.Dequeue();
        }

        // Обновляем текст в UI
        UpdateConsoleText();
    }

    // Метод для очистки консоли
    public void ClearConsole()
    {
        messageQueue.Clear();
        UpdateConsoleText();
    }

    // Метод для обновления текста в UI
    private void UpdateConsoleText()
    {
        // Собираем все сообщения из очереди в одну строку
        string consoleOutput = string.Join("\n", messageQueue);

        // Обновляем текст в Text или TextMeshPro
        if (consoleText != null)
        {
            consoleText.text = consoleOutput;
        }

        if (consoleTextMeshPro != null)
        {
            consoleTextMeshPro.text = consoleOutput;
        }
    }

    private void SendComData()
    {
        if (!string.IsNullOrEmpty(_inputComAddress.text) ||
            !string.IsNullOrEmpty(_inputBaudeRate.text) ||
            !string.IsNullOrEmpty(_inputReadTimeout.text))
        {
            ComData data = new ComData(Convert.ToInt32(_inputComAddress.text)+1, Convert.ToInt32(_inputBaudeRate.text),Convert.ToInt32(_inputReadTimeout.text),0,0,0,0);
            _saveLoadService.SetComData(data);
        }
    }

    public void Send()
    {
        
            ComData data = new ComData(26, 2400,500,0,0,0,0);
            _saveLoadService.SetComData(data);
        
    }
    
    private void AddListeners()
    {
        _apply.onClick.AddListener(SendComData);

        // _saveLoadService.OnSetComData += CheckSerialPort;
    }

    private void OnDestroy()
    {
        RemuveListeners();
    }
    

    private void RemuveListeners()
    {
        _apply.onClick.RemoveListener(SendComData);
    }
}