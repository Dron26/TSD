using System;

[Serializable]
public class SwithStatus
{

    public bool IsEquipmentSelected;
    public bool IsTrolleySelected;

    public SwithStatus(bool state, bool state2)
    {
        IsEquipmentSelected = state;
        IsTrolleySelected = state2;
    }

    
}