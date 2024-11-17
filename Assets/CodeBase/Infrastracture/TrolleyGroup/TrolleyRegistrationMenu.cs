using System;
using CodeBase.Infrastracture.Datas;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.TrolleyGroup
{
    public class TrolleyRegistrationMenu : MonoBehaviour,IWindow
    {
        [SerializeField] private TrolleyResult _trolleyResult;
        [SerializeField] private TrolleyValidator _validator;
        [SerializeField] private Button _buttonApply;
        [SerializeField] private Button _buttonApplyResult;
        [SerializeField] private Button _backButton;

        public Action OnApplyRegistration;
        public Action OnBackButtonCLick;
        
        private SaveLoadService _saveLoadService;
        
        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _validator.Init(_saveLoadService,warningPanel);
            _trolleyResult.Init(_saveLoadService);
            AddListeners();
        }

        public void Work()
        {
            _trolleyResult.Work();
            _buttonApply.interactable = false;
        }

        public void Reset()
        {
            _validator.Reset();
            _trolleyResult.Reset();
            _buttonApply.interactable = false;
        }

        private void OnResultButtonClick()
        {
            OnApplyRegistration.Invoke();
            _trolleyResult.SwithState(false);
            Reset();
        }

        private void OnGetTrolley()
        {
            _buttonApply.interactable = true;
        }

        private void OnApplyButtonClick()
        {
            _validator.SwithState(false);
            _trolleyResult.SwithState(true);
            _trolleyResult.SetData();
        }

        public void SwitchValidatorState(bool state)
        {
            _validator.SwithState(state);
            
            if (state)
            {
                _validator.Work();
            }
            else
            {
                _validator.Reset();
            }
        }

        private void AddListeners()
        {
            _validator.OnTakeTrolley += OnGetTrolley;
            _buttonApply.onClick.AddListener(OnApplyButtonClick);
            _buttonApplyResult.onClick.AddListener(OnResultButtonClick);
            _backButton.onClick.AddListener(OnCLickBackButton);

        }

        private void RemuveListeners()
        {
            _validator.OnTakeTrolley -= OnGetTrolley;
            _buttonApply.onClick.RemoveListener(OnApplyButtonClick);
            _buttonApplyResult.onClick.RemoveListener(OnResultButtonClick);
            _backButton.onClick.RemoveListener(OnCLickBackButton);
        }

        private void OnDisable()
        {
            RemuveListeners();
        }

        public void OnCLickBackButton()
        {
            OnBackButtonCLick.Invoke();
        }
    }
}