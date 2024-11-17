using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastracture.EquipmentGroup;
using CodeBase.Infrastracture.TrolleyGroup;
using Unity.VisualScripting;

namespace CodeBase.Infrastracture.Datas
{
    [Serializable]
    public class Data
    {
        [Serialize] public List<Employee> _employees = new();
        [Serialize] public List<Equipment> _equipments = new();
        [Serialize] public List<Box> _boxes = new();
        [Serialize] public List<string> _keys = new();
        [Serialize] public List<Trolley> _trollyes = new();
        [Serialize] public List<Printer> _printers = new();
        [Serialize] public Employee Employee;
        [Serialize] public Employee _employee;

        private int _currentIndex;

        public Data(List<Employee> employees, List<Box> boxes, List<Trolley> trollyes)
        {
            foreach (var employee in employees)
            {
                Employee newEmployee = new Employee(employee.Login, employee.Password, employee.Permission);

                if (employee.HaveBox)
                {
                    newEmployee.SetBox(employee.Box);
                    
                    if (employee.Box.Printer != null)
                    {
                        newEmployee.Box.SetPrinter(employee.Box.Printer);
                    }
                }
                //newEmployee.SetEquipmentData(DateTime.Now);
                newEmployee.SetEquipmentData(employee.GetDateTakenEquipment());
                
                if (employee.HavePrinter)
                {
                    newEmployee.SetPrinter(employee.Printer);
                }

                if (employee.HaveTrolley)
                {
                    newEmployee.SetTrolley(employee.Trolley);
                    newEmployee.Trolley.SetBusy(true);
                }

                _employees.Add(newEmployee);
            }

            foreach (var box in boxes)
            {
                Box newBox = new Box(box.Key, box.Equipment);
                if (box.Printer != null)
                {
                    newBox.SetPrinter(box.Printer);
                    _printers.Add(box.Printer);
                }

                newBox.SetBusy(box.Busy);
                _boxes.Add(newBox);
                _keys.Add(box.Key);
            }

            foreach (var trolly in trollyes)
            {
                Trolley newTrolley = new Trolley(trolly.Number);
                newTrolley.SetBusy(newTrolley.Busy);
                _trollyes.Add(newTrolley);
            }
        }

        public void SetCurrentEmployeer(Employee employee)
        {
            var employeeToUpdate = _employees.FirstOrDefault(e => e.Login == employee.Login);

            if (employeeToUpdate != null)
            {
                _currentIndex = _employees.IndexOf(employeeToUpdate);
                _employees[_currentIndex] = new(employee.Login, employee.Password, employee.Permission);

                if (employee.HaveBox)
                {
                    Box box = new Box(employee.Box.Key, employee.Box.Equipment);
                    _employees[_currentIndex].SetBox(box);
                }
                _employees[_currentIndex].SetEquipmentData(employee.GetDateTakenEquipment());

                if (employee.HaveTrolley)
                {
                    Trolley trolley = new Trolley(employee.Trolley.Number);
                    _employees[_currentIndex].SetTrolley(trolley);
                }

                if (employee.HavePrinter)
                {
                    Printer printer = new Printer(employee.Printer.SerialNumber);
                    _employees[_currentIndex].SetPrinter(printer);
                }

                Employee = _employees[_currentIndex];
            }
            
            SortEmployeesByLogin();
        }

        public void SetBox(Box box)
        {
            string key = box.Key;
            int index;
            
            if (_keys.Contains(key))
            {
                index = _boxes.IndexOf(_boxes.Find(x => x.Key == box.Key));
                _boxes[index].SetBusy(box.Busy);
            }
            else
            {
                _boxes.Add(box);
                index = _boxes.IndexOf(box);
                _keys.Add(box.Key);
            }

            if (box.Printer != null)
            {
                _boxes[index].SetPrinter(box.Printer);
                SetPrinter(box.Printer);
            }
        }

        public List<Box> GetBoxes()
        {
            return new List<Box>(_boxes);
        }

        public List<Trolley> GetTrolleys()
        {
            return new List<Trolley>(_trollyes);
        }

        public void SetTrolleys(Trolley trolley)
        {
            bool changed = false;

            foreach (Trolley thisTrolley in _trollyes)
            {
                if (thisTrolley.Number == trolley.Number)
                {
                    thisTrolley.Busy = trolley.Busy;
                    changed = true;
                    break;
                }
            }

            if (!changed)
            {
                _trollyes.Add(trolley);
            }
        }

        public List<Printer> GetPrinters()
        {
            return new List<Printer>(_printers);
        }

        public void SetPrinter(Printer printer)
        {
            bool changed = false;

            foreach (Printer thisPrinter in _printers)
            {
                if (thisPrinter.SerialNumber == printer.SerialNumber)
                {
                    thisPrinter.Busy = printer.Busy;
                    changed = true;
                    break;
                }
            }

            if (!changed)
            {
                _printers.Add(printer);
            }
        }

        public List<Employee> GetEmployees()
        {
            SortEmployeesByLogin();
            return new List<Employee>(_employees);
        }

        public void AddNewEmployee(Employee employee)
        {
            foreach (var thisEmployee in _employees)
            {
                if (thisEmployee.Login == employee.Login)
                {
                    int index = _employees.IndexOf(thisEmployee);

                    if (index < _currentIndex)
                    {
                        _currentIndex--;
                    }

                    break;
                }
            }

            _employees.Add(employee);
        }

        public void RemoveEmployee(Employee employee)
        {
            _employees.Remove(employee);
        }

        public void RemoveBox(Box box)
        {
            if (box.Printer!=null)
            {
                _printers.Remove(box.Printer);
            }
            
            _boxes.Remove(box);
            _keys.Remove(box.Key);
            
        }

        public void RemoveTrolley(Trolley trolley)
        {
            _trollyes.Remove(trolley);
        }

        public void RemovePrinter(Printer printer)
        {
            if (_printers.Contains(printer))
            {
                int index = _boxes.IndexOf(_boxes.Find(x => x.Printer != null && x.Printer.SerialNumber == printer.SerialNumber));
                _boxes[index].GetPrinter();
                _printers.Remove(printer);
            }
        }
        
        public void SortEmployeesByLogin()
        {
            _employees.Sort((employee1, employee2) => string.Compare(employee1.Login, employee2.Login, StringComparison.Ordinal));
        }
    }
}