using System;
using Unity.VisualScripting;

[Serializable]
public class Printer
{
    [Serialize] public string SerialNumber { get; set; }
    [Serialize] public bool Busy;
    public Printer(string serialNumber)
    {
        SerialNumber = serialNumber;
    }
    public void SetBusy(bool state)
    {
        Busy = state;
    }
}