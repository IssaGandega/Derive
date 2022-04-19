using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Bounce_Move_Fade : MonoBehaviour
{
    public Transform Gameobject;
    [SerializeField] float X_Start;
    [SerializeField] float Y_Start;
    [SerializeField] float Y_Destination;
    [SerializeField] float Time;
    [SerializeField] float delay;

    [SerializeField] float x_size;
    [SerializeField] float y_size;
    [SerializeField] float time_scale;

    [SerializeField] Image image;
    [SerializeField] Color beginColor, endColor;

    // Start is called before the first frame update
    void OnEnable()
    {
        Gameobject.localPosition = new Vector2(X_Start, Y_Start);
        image.color = beginColor;
        LeanTween.moveLocalY(gameObject, Y_Destination, Time).setEaseInOutBack().setDelay(delay)
            .setOnUpdate((value) =>
            {
                image.fillAmount = value;
                image.color = Color.Lerp(beginColor, endColor, value);
            });

        transform.LeanScale(new Vector3(x_size, y_size, 1), time_scale).setEaseInOutBack();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
