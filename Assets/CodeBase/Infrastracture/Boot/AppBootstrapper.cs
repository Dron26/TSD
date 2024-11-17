using CodeBase.Infrastracture.Datas;
using UnityEngine;

namespace CodeBase.Infrastracture.Boot
{
    public class AppBootstrapper : MonoBehaviour
    {
        [SerializeField] private Program _program;
        [SerializeField] private SaveLoadService _saveLoad;

        private void Awake()
        {
            _saveLoad.Init();
            Init();
        }
        
        public void Init()
        {
            _program.Init(_saveLoad);
            _program.Work();
        }
    }
}