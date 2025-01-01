using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsoleUI : MonoBehaviour
{
    // Ссылка на Text или TextMeshPro
    public Text consoleText; // Для обычного Text
    public TextMeshProUGUI consoleTextMeshPro; // Для TextMeshPro

    // Очередь для хранения сообщений
    private Queue<string> messageQueue = new Queue<string>();

    // Максимальное количество сообщений в консоли
    public int maxMessages = 10;

    void Start()
    {
        // Очищаем консоль при старте
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
}