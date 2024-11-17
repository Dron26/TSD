using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Random;

namespace CodeBase
{
    public class CatMover: MonoBehaviour
    {
        [SerializeField] private List<Image> _cats;
        private Image _cat;
        
        private int _second = 200;
        private WaitForSeconds wait;
        private int _number; 
        System.Random random = new();
        private bool _isMove;
        private int number;
        private bool isWork;
        
        private void Start()
        {
            isWork = true;
            number = 0;
            _isMove = false;
            StartCoroutine(CatMove());
        }

        private IEnumerator CatMove()
        {
            while (isWork)
            {
                if (!_isMove)
                {
                    SetTime();
                    yield return wait;
                
                    StopCoroutine(IconMove());
                    yield return wait= new (0.5f);
                
                    ShowCat();
                }
                
                yield return null;
            }
        }

        private void SetTime()
        {
            wait= new(random.Next(_second/2, _second));
        }
        
        private void ShowCat()
        {
            if (number==0)
            {
                number = 1;
            }
            else
            {
                number = 0;
            }

            _cat=_cats[number];
            _cat.gameObject.SetActive(true);
        
            StartCoroutine(IconMove());
        }
        
        private IEnumerator IconMove()
        {
            _isMove = true;
            _cat.gameObject.SetActive(true);
            float size = _cat.GetComponent<RectTransform>().rect.height;
            float tempSize = 0f;
            
            while (tempSize<=size)
            {
                
                _cat.transform.position = new Vector3(_cat.transform.position.x, _cat.transform.position.y + 1f, _cat.transform.position.z);
                tempSize +=1f;
                yield return null;
            }
            
            yield return wait= new (3f);
            
            while (0<tempSize)
            {
                _cat.transform.position = new Vector3(_cat.transform.position.x, _cat.transform.position.y - 1f, _cat.transform.position.z);
                tempSize -=1f;
                yield return null;
            }
            _cat.gameObject.SetActive(false);
            _isMove = false;
            yield break;
        }
        
        public void SetWork(TMP_Text time)
        {
            isWork = !isWork;
            
            if (isWork)
            {
                Start();
                
                
                if (Int32.TryParse(time.text, out int newTime))
                {
                    wait = new(newTime*60f);
                }
                else
                {
                    wait = new(_second);
                }
            }
            else
            {
                StopCoroutine(CatMove());
            }
        }
    }
}