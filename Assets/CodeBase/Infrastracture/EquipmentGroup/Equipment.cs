using System;
using Unity.VisualScripting;

namespace CodeBase.Infrastracture.EquipmentGroup
{
    [Serializable]
    public class Equipment
    {
        [Serialize] public string SerialNumber { get; set; }


        public Equipment(string serialNumber)
        {
            SerialNumber = serialNumber;
        }
    }
}