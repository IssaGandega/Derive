using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopesManager : MonoBehaviour
{
    [SerializeField] private Material trapMat;

    public enum State
    {
        Disabled,
        Red,
        Blue
    }

    public enum Type
    {
        Static,
        Regular
    }

    
    public State state;
    public Type type;
    private State lastState;
    private GameObject player;
    private GameObject fx;
    private Material actualMat;

    [Space]
    
    private bool isMoving;
    private bool isTowardMin;

    public float minScale = -1;
    public float maxScale = 1;
    
    [Space]

    [SerializeField] private float animationSpeed;
    [SerializeField] private int bounceNumber;

    [Space] 
    
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private AudioClip crackSound;
    

    private int totalBounces;
    
    void Start()
    {
        actualMat = Instantiate(trapMat);
        gameObject.GetComponent<MeshRenderer>().material = actualMat;
    }

    private void Update()
    {
        if (state != lastState)
        {
            switch (state)
            {
                case State.Disabled:
                    gameObject.GetComponent<MeshRenderer>().material.SetFloat("_LerpCordes", 0);
                    break;
                case State.Blue:
                    gameObject.GetComponent<MeshRenderer>().material.SetFloat("_LerpCordes", 1);
                    gameObject.GetComponent<MeshRenderer>().material.SetFloat("_WhichOne", 1);
                    break;
                case State.Red:
                    gameObject.GetComponent<MeshRenderer>().material.SetFloat("_LerpCordes", 1);
                    gameObject.GetComponent<MeshRenderer>().material.SetFloat("_WhichOne", 0);
                    break;
            }
        }

        lastState = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            player = other.gameObject;
            if (player.name.Replace("Player_", "") == state.ToString())
            {
                GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                GetComponent<BoxCollider>().enabled = false;
                StartCoroutine(DisableRope());
                AudioManager.PlaySound(crackSound);
                fx = Pooler.instance.Pop("Crack");
                Pooler.instance.DelayedDePop(2, "Crack", fx);
                fx.transform.position = transform.position;
                fx.transform.parent = transform;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<PlayerController>())
        {
            AudioManager.PlaySound(bounceSound);
            gameObject.GetComponent<MeshRenderer>().material.SetInt("_Idle", 0);
            //player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player.GetComponent<PlayerController>().destination = player.transform.position;
            totalBounces = bounceNumber;
            StartCoroutine(RopeTimer());
        }
    }

    private IEnumerator DisableRope()
    {
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(2);
        GetComponent<MeshRenderer>().enabled = true;
    }
    
    private IEnumerator RopeTimer()
    {
        // Just in case block concurrent routines
        if(isMoving) yield break;

        // Block input
        isMoving = true;

        // Decide if going to Max or min
        var targetAmplitude = isTowardMin ? minScale : maxScale;

        // Store Start values
        var startAmplitude = gameObject.GetComponent<MeshRenderer>().material.GetFloat("_Amplitude");

        var duration = 1 / animationSpeed;
        var timePassed = 0f;

        while(timePassed < duration)
        {
            var t = timePassed / duration;
            // Optional: add ease-in and ease-out
            t = Mathf.SmoothStep(0, 1, t);

            gameObject.GetComponent<MeshRenderer>().material.SetFloat("_Amplitude", Mathf.Lerp(startAmplitude, targetAmplitude, Time.deltaTime));
            

            // Increase the time passed since last frame
            startAmplitude = gameObject.GetComponent<MeshRenderer>().material.GetFloat("_Amplitude");


            timePassed += Time.deltaTime;

            // This tells Unity to "pause" the routine here,
            // render this frame and continue from here
            // in the next frame
            yield return null;
        }

        // Just to be sure to end with exact values apply them hard once
        gameObject.GetComponent<MeshRenderer>().material.SetFloat("_Amplitude", targetAmplitude);

        // Invert direction
        isTowardMin = !isTowardMin;

        // Done -> unlock input
        isMoving = false;
        bounceNumber -= 1;

        if (bounceNumber > 0)
        {
            StartCoroutine(RopeTimer());
        }
        else
        {
            bounceNumber = totalBounces;
            gameObject.GetComponent<MeshRenderer>().material.SetInt("_Idle", 1);
        }
    }
}
