using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Rotate : MonoBehaviour

{

    [SerializeField] float x;
    [SerializeField] float y;
    [SerializeField] float z;
    [SerializeField] float time_size;
    [SerializeField] float z_rotation;
    [SerializeField] float time_rotation;

    // Start is called before the first frame update
    void Start()
    {
        transform.LeanScale(new Vector3(x, y, z), time_size).setEaseInSine().setLoopPingPong();
        LeanTween.rotateAroundLocal(gameObject, Vector3.forward, z_rotation, time_rotation).setRepeat(-1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
