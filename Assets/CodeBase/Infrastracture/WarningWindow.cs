using TMPro;
using UnityEngine;

namespace CodeBase.Infrastracture
{
    public class WarningWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}