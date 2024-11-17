using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeBase.Infrastracture.Datas;
using UnityEngine;

namespace CodeBase.Infrastracture
{
    public class Logger : MonoBehaviour
    {
        [SerializeField] private RequestQueue _requestQueue;
        
        private Queue<SentData> _queue = new Queue<SentData>();
        private Uri uri = null;
        private int _maxCountDay = 8;
        private bool _isAdditionalLogOn = false;
        private bool _isProcessing = false;
        private bool _isLogged=true;
        private SaveLoadService _saveLoadService;
        
        
        public void Init(SaveLoadService saveLoadService)
        {
            
            Application.logMessageReceivedThreaded += LogHandler;
            _saveLoadService = saveLoadService;
            _requestQueue.Init(_saveLoadService);
            DontDestroyOnLoad(_requestQueue);
            DontDestroyOnLoad(this);
            SetLogStatus();
            AddListeners();
        }
        private void SetLogStatus()
        {
            _isLogged = _saveLoadService.IsLogged;
        }
        public void WriteData(SentData data)
        {
            DateTime Time = DateTime.Now;
            string info = $"Time: {Time}, {data.Action},Login: {data.Login}, Password: {data.Pass}, Tsd: {data.ShortNumber}, Key: {data.Key},Comment ";

            AppendDataToFile(Const.DataInfo, info);
        }

        public void WriteLog(SentData data)
        {
            DateTime currentTime = DateTime.Now;
            string info = $"{currentTime} :  {data.Action}";

            AppendDataToFile(Const.LogInfo, info);
        }

        public void SendLog(SentData log)
        {
            if (_isLogged)
            {
                WriteLog(log);
                _requestQueue.EnqueueLog(log);
            }
        }

        public void SendData(SentData data)
        {
            if (_isLogged)
            {
                WriteData(data);
                _requestQueue.EnqueueData(data);
            }
        }

        public void SendTrolleyData(SentData data)
        {
            
             _requestQueue.EnqueueTrolleyData(data);
        }

        public void SendWorkInfo()
        {
           StartCoroutine( _requestQueue.SendWorkInfo());
        }
        
        public void CheckTime()
        {
            List<string> consts = new List<string>();

            consts.Add(Const.DataInfo);
            consts.Add(Const.LogInfo);

            for (int i = 0; i < consts.Count; i++)
            {
                string filename = consts[i].ToString();
                string path = Path.Combine(Application.persistentDataPath, filename);

                if (File.Exists(path))
                {
                    try
                    {
                        string[] pempText = File.ReadAllLines(path);
                        string text = "";
                        DateTime date;

                        if (pempText.Length != 0)
                        {
                            text = File.GetCreationTime(path).ToString("dd.MM.yyyy");
                        }

                        if (DateTime.TryParse(text, out date))
                        {
                            DateTime thresholdDate = date.AddDays(_maxCountDay);

                            if (thresholdDate <= DateTime.Now)
                            {
                                string newPath = Path.Combine(Application.persistentDataPath, date.ToString("dd.MM.yyyy"), filename);
                                File.Copy(path, newPath);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                if (!File.Exists(path))
                {
                    string text = DateTime.Now.Date.ToString("dd.MM.yyyy");

                    using (StreamWriter writer = new StreamWriter(path, false))
                    {
                        writer.WriteLineAsync(text);
                    }
                }
            }
        }

        public void AppendDataToFile(string filename, string content)
        {
            string path = Path.Combine(Application.persistentDataPath, filename);

            try
            {
                using (StreamWriter
                       writer = new StreamWriter(path, true)) 
                {
                    writer.WriteLine(content); 
                }

            }
            catch (Exception e)
            {
            }
        }
        
        private void AddListeners()
        {
            _saveLoadService.ChangeLogStatus += SetLogStatus;
        }
        
        private void RemoveListeners()
        {
            _saveLoadService.ChangeLogStatus -= SetLogStatus;
        }
        
        public void LogHandler(string logString, string stackTrace, LogType type)
        {
            string action=("**** UNITY MESSAGE BEGINE ****"
                                    + "\nLOG STRING\n" + logString
                                    + "\nSTACK TRACE\n" + stackTrace
                                    + "\nLOG TYPE\n" + type.ToString()
                                    + "\n**** UNITY MESSAGE END ****\n");
            SentData log = new SentData(action);
            SendLog(log);
        }

        public void SendOverdueInfo()
        {
            _requestQueue.SendOverdueInfo();
        }
    }
}