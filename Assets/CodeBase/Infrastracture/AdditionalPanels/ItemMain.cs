using UnityEngine;

public class ItemMain:MonoBehaviour
{
    [SerializeField] private GameObject _panelBox;
    [SerializeField] private GameObject _panelEquipment;


    public GameObject GetBox()
    {
        return _panelBox;
    }
    
    public GameObject GetEquipment()
    {
        return _panelEquipment;
    }
}