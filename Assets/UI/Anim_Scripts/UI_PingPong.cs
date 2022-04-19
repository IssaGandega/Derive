using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PingPong : MonoBehaviour

{

    [SerializeField] float x_size;
    [SerializeField] float y_size;
    [SerializeField] float time_scale;

    // Start is called before the first frame update
    void OnEnable()
    {
        transform.LeanScale(new Vector3(x_size, y_size, 1), time_scale).setEaseInSine().setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
