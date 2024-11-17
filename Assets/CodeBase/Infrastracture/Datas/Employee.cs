using System;
using System.Globalization;
using CodeBase.Infrastracture.EquipmentGroup;
using CodeBase.Infrastracture.TrolleyGroup;
using UnityEngine;

namespace CodeBase.Infrastracture.Datas
{
    [Serializable]
    public class Employee
    {
        public Box Box;
        public Equipment Equipment;
        public Trolley Trolley;
        public Printer Printer;
        public string Login;
        public string Password;
        public string Permission;
        public bool HaveEquipment;
        public bool HaveBox;
        public bool HaveTrolley;
        public bool HavePrinter;
        public string DateTakenEquipment;
        
        public Employee(string login, string password, string permission)
        {
            Login = login;
            Password = password;
            Permission = permission;
            HaveBox = false;
            HaveEquipment = false;
            Box = null;
        }

        public Box GetBox()
        {
            Box oldBox = new(Box.Key, Box.Equipment);
            
            if (HavePrinter)
            {
                oldBox.SetPrinter(Printer);
            }
            
            Equipment=new Equipment("");
            Printer = null;
            Box = null;
            HaveBox = false;
            HaveEquipment= false;
            HavePrinter = false;
            SetEquipmentData( DateTime.Now);
            return oldBox;
        }

        public void SetBox(Box box)
        {
            Box = new(box.Key, box.Equipment);
            Box.SetBusy(true);
            HaveBox = true;
            Equipment = Box.Equipment;
            
            if (box.Printer!=null)
            {
                Printer = Box.Printer;
                HavePrinter = true;
            }
            
            HaveEquipment = true;
        }
        
        public void SetPermision(string permission)
        {
            Permission = permission;
        }
        
        public void SetPassword(string password)
        {
            Password = password;
        }
        
        public void SetTrolley(Trolley trolley)
        {
            HaveTrolley= true;
            Trolley = new Trolley(trolley.Number);
        }
        
        public void SetPrinter(Printer printer)
        {
            HavePrinter = true;
            Printer = new Printer(printer.SerialNumber);
            Printer.SetBusy(true);
        }
        
        public Trolley GetTrolley()
        {
            Trolley oldTrolley = Trolley;
            Trolley = null;
            HaveTrolley = false;
            return oldTrolley;
        }
        
        public void SetEquipmentData(DateTime date)
        {
            DateTakenEquipment = date.ToString("dd-MM-yyyyTHH:mm:ss.fffffffK");
        }
        
        public DateTime GetDateTakenEquipment()
        {
            if (DateTime.TryParseExact(DateTakenEquipment, "dd-MM-yyyyTHH:mm:ss.fffffffK", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            else
            {
                throw new FormatException("Invalid date format. Expected format: MM-dd-yyyyTHH:mm:ss.fffffffK");
            }
        }
    }
}