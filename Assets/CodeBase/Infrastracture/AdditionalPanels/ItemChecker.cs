using UnityEngine;

public class ItemChecker:MonoBehaviour
{
    [SerializeField] private GameObject _panelLogin;
    [SerializeField] private GameObject _panelKey;


    public GameObject GetLogin()
    {
        return _panelLogin;
    }
    
    public GameObject GetKey()
    {
        return _panelKey;
    }
}