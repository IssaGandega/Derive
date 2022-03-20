using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FadeOut_Fill_PingPong : MonoBehaviour
{
    public Image image;
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
            .setDelay(delay).setLoopPingPong(1);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
