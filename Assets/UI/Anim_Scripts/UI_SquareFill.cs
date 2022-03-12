using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SquareFill : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float tweentime, start_fill, end_fill, delay;
    [SerializeField] Color beginColor, endColor;
    // Start is called before the first frame update
    void Start()
    {
        image.fillAmount = 0;
        LeanTween.value(gameObject, start_fill, end_fill, tweentime)
            .setEaseInOutBack()
            .setOnUpdate((value) =>
            {
                image.fillAmount = value;
                image.color = Color.Lerp(beginColor, endColor, value);
            })
            .setDelay(delay) ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
