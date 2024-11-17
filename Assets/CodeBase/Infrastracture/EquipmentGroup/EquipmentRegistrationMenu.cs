using System;
using CodeBase.Infrastracture.Datas;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.EquipmentGroup
{
    public class EquipmentRegistrationMenu : MonoBehaviour,IWindow
    {
        [SerializeField] private EquipmentResult _equipmentResult;
        //[SerializeField] private Button _buttonApply;
        [SerializeField] private Button _buttonApplyResult;
        [SerializeField] private Button _backButton;
        [SerializeField] private EquipmentValidator _validator;

        public Action OnApply;
        public Action OnReturnEquipment;
        public Action OnRegistrationEnd;
        public Action OnApplyRegistration;
        public Action OnBackButtonCLick;
        
        private SaveLoadService _saveLoadService;
        
        public void Init(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            _validator.Init(_saveLoadService);
            _equipmentResult.Init(_saveLoadService);
            AddListeners();
        }

        public void Work()
        {
            _equipmentResult.Work();
 //           _buttonApply.interactable = false;
        }

        public void Reset()
        {
            _validator.Reset();
            _equipmentResult.Reset();
  //          _buttonApply.interactable = false;
        }

        private void OnResultButtonClick()
        {
            OnApplyRegistration.Invoke();
            _equipmentResult.SwithState(false);
            Reset();
        }

       

        private void OnGetEquipment()
        {
 //           _buttonApply.interactable = true;
        }

        private void OnApplyButtonClick()
        {
            _validator.SwithState(false);
            OnApply?.Invoke();
            OnRegistrationEnd?.Invoke();
            _equipmentResult.SwithState(true);
            _equipmentResult.SetData();
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
            _validator.OnTakeKey += OnApplyButtonClick;
   //         _buttonApply.onClick.AddListener(OnApplyButtonClick);
            _buttonApplyResult.onClick.AddListener(OnResultButtonClick);
            _backButton.onClick.AddListener(OnCLickBackButton);

        }

        private void RemuveListeners()
        {
            _validator.OnTakeKey -= OnApplyButtonClick;
     //       _buttonApply.onClick.RemoveListener(OnApplyButtonClick);
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