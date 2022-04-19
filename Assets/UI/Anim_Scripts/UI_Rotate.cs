using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Rotate : MonoBehaviour

{

    [SerializeField] float z_rotation;
    [SerializeField] float time_rotation;

    // Start is called before the first frame update
    void OnEnable()
    {
        LeanTween.rotateAroundLocal(gameObject, Vector3.forward, z_rotation, time_rotation).setEaseInQuad().setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
