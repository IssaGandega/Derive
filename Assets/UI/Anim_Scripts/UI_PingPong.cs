using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PingPong : MonoBehaviour

{

    [SerializeField] float x;
    [SerializeField] float y;
    [SerializeField] float time;

    // Start is called before the first frame update
    void Start()
    {
        transform.LeanScale(new Vector3(x, y, 1), time).setEaseInSine().setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
