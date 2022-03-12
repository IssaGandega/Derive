using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class TakeShipControl : MonoBehaviour
{
    public bool isDetecting;
    public float lerpOutline;
    public float whichColor;
    
    
    private bool canChange = true;
    private GameObject player;
    private float rotateAngle;

    [SerializeField] private float rotationSpeed;
    [SerializeField] Material boatMat;

    private void Awake()
    {
        boatMat.SetFloat("_LerpCordes", 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            isDetecting = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            isDetecting = false;
            player = null;
        }
    }

    private void LateUpdate()
    {
        TakeControl();
    }

    private void TakeControl()
    {
        if (isDetecting && canChange && player.GetComponent<PlayerController>().interacting)
        {
            StartCoroutine(ChangeOwnership());
        }
    }

    private IEnumerator ChangeOwnership()
    {
        canChange = false;
        rotateAngle = transform.rotation.z + 180;
        StartCoroutine(RotateRudder());
        
        if (boatMat.GetFloat("_LerpCordes") == 0)
        {
            boatMat.SetFloat("_LerpCordes", 1);
        }

        if (player.name == "Player_Red")
        {
            boatMat.SetFloat("_WhichOne", 0);
        }
        else
        {
            boatMat.SetFloat("_WhichOne", 1);
        }
        
        yield return new WaitForSeconds(3);
        canChange = true;
        rotationSpeed *= -1;
    }

    private IEnumerator RotateRudder()
    {
        transform.Rotate(Vector3.forward * (Time.deltaTime * rotationSpeed));
        yield return new WaitForFixedUpdate();
        if (canChange == false)
        {
            StartCoroutine(RotateRudder());
        }

        
    }
}
