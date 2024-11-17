using System.Collections;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using CodeBase.Infrastracture.EquipmentGroup;
using CodeBase.Infrastracture.TrolleyGroup;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SheetProcessor : MonoBehaviour
{
    private readonly int _login = 0;
    private readonly int _password = 1;
    private readonly int _permission = 2;
    private readonly int _serialNumber = 3;
    private readonly int _key = 4;
    private readonly int _printerSerial = 5;
    private readonly int _trolleNumber = 6;
    private readonly char _cellSeporator = ',';
    private readonly int _buyedInfo = 0;
    private bool _isBuyProgramm;
    private string _errorMessage;
    public WebData ProcessData(string cvsRawData)
    {
        WebData data = new WebData();
        data.Employees = new();
        data.Boxes = new();
        data.Trolleys = new();

        if (_isBuyProgramm)
        {
            char lineEnding = GetPlatformSpecificLineEnd();
            string[] rows = cvsRawData.Split(lineEnding);
            int dataStartRawIndex = 2;
            
            Employee employee;
            Equipment equipment;
            Trolley trolley;
            Box box;


            for (int i = dataStartRawIndex; i < rows.Length; i++)
            {
                string[] cells = rows[i].Split(_cellSeporator);


                if (cells[_login] != "" && cells[_password] != "" && cells[_permission] != "")
                {
                    employee = new Employee(cells[_login], cells[_password], cells[_permission]);
                    data.Employees.Add(employee);
                }

                if (cells[_serialNumber] != "" && cells[_key] != "")
                {
                    equipment = new Equipment(cells[_serialNumber]);

                    box = new Box(cells[_key], equipment);
                    data.Boxes.Add(box);
                }

                if (cells[_trolleNumber] != "" && cells[_trolleNumber] != "\r")
                {
                    string x = cells[_trolleNumber];
                    trolley = new Trolley(x);

                    data.Trolleys.Add(trolley);
                }

                if (cells[_trolleNumber] != "" && cells[_trolleNumber] != "\r")
                {
                    string x = cells[_trolleNumber];
                    trolley = new Trolley(x);

                    data.Trolleys.Add(trolley);
                }
            }
        }

        return data;
    }

    public bool CheckBuyedProgramm(string cvsRawData)
    {
        char lineEnding = GetPlatformSpecificLineEnd();
        string[] rows = cvsRawData.Split(lineEnding);
        string[] cells = rows[_buyedInfo].Split(_cellSeporator);

        _isBuyProgramm = false;
        string result= rows[0].Split(_cellSeporator)[0];
        
        if (result == "10")
        {
            _isBuyProgramm = true;
        }
        else
        {
            _errorMessage = rows[1].Split(_cellSeporator)[0];
        }

        return _isBuyProgramm;
    }

    public string GetErrorMessage()
    {
        return _errorMessage;
    }
    private char GetPlatformSpecificLineEnd()
    {
        char lineEnding = '\n';
#if UNITY_IOS
        lineEnding = '\r';
#endif
        return lineEnding;
    }
}