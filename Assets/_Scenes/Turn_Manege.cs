using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn_Manege : MonoBehaviour
{

    [SerializeField] float z_rotation;
    [SerializeField] float time_rotation;

    // Start is called before the first frame update
    void Start()
    {
        LeanTween.rotateLocal(gameObject, z_rotation, time_rotation).setEaseInOutCubic();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) // un seul doigt sur l'écran
        {

        }
    }
}
