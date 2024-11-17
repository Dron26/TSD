using System;
using JetBrains.Annotations;
using Unity.VisualScripting;

namespace CodeBase.Infrastracture
{
    [Serializable]
    public class SentData
    {
        public string Action { get; set; }
        public string Login { get; set; }
        
        public string Pass { get; set; }
        public string ShortNumber { get; set; }
        public string Key { get; set; }
        public string Time { get; set; }
        
        public string Comment { get; set; }
        
        public string TrolleyNumber { get; set; }
        public string PrinterNumber { get; set; }
        public string InitialLogin { get; set; }

        public SentData(string action="", string login="",string password="", string key="", string shortNumber="", string time="",string comment="")
        {
            Action = action;
            Login = login;
            Pass=password;
            Key = key;
            ShortNumber = shortNumber;
            Time = time;
            Comment = comment;
        }
        
        public void SetTrolleyNumber(string trolleyNumber)=> 
            TrolleyNumber = trolleyNumber;
        
        public void SetPrinterNumber(string printerNumber)=> 
            PrinterNumber = printerNumber;
        
        public void SetInitial(string login)=> 
            InitialLogin = login;
    }
}