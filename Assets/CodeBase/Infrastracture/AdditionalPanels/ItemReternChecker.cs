using UnityEngine;

public class ItemReternChecker:MonoBehaviour
{
    [SerializeField] private GameObject _panelLogin;
    [SerializeField] private GameObject _panelKey;
    [SerializeField] private GameObject _time;
    [SerializeField] private GameObject _badTime;
    
    public GameObject GetLogin()
    {
        return _panelLogin;
    }
    
    public GameObject GetKey()
    {
        return _panelKey;
    }
    
    public GameObject GetTime()
    {
        return _time;
    }
    
    public GameObject GetBadTime()
    {
        return _badTime;
    }
}