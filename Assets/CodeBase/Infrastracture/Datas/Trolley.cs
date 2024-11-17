using System;
using Unity.VisualScripting;

namespace CodeBase.Infrastracture.TrolleyGroup
{  
    [Serializable]
    public class Trolley
    {
        [Serialize] public string Number { get; set; }
        [Serialize] public bool Busy;

        public Trolley(string number)
        {
            Number = number;
            
        }
        
        public void SetBusy(bool state)
        {
            Busy = state;
        }
    }
}