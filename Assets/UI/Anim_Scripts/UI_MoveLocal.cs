using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_MoveLocal : MonoBehaviour
{
    public Transform Gameobject;
    [SerializeField] float X_Start;
    [SerializeField] float Y_Start;
    [SerializeField] float X_Destination;
    [SerializeField] float Time;
    [SerializeField] float delay;

    [SerializeField] float x_size;
    [SerializeField] float y_size;
    [SerializeField] float time_scale;

    // Start is called before the first frame update
    void OnEnable()
    {
        Gameobject.localPosition = new Vector2(X_Start, Y_Start);
        LeanTween.moveLocalX(gameObject, X_Destination, Time).setEaseInOutBack().setDelay(delay);
        transform.LeanScale(new Vector3(x_size, y_size, 1), time_scale).setEaseInOutBack().setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
