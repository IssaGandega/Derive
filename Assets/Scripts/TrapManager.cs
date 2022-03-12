using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrapManager : MonoBehaviour
{
    [SerializeField] private Material trapMat;

    public enum State
    {
        Disabled,
        Red,
        Blue
    }

    public State state;
    private State lastState;
    
    private Material actualMat;
    private GameObject player;
    private bool isDetecting;
    
    void Start()
    {
        state = State.Disabled;
        gameObject.GetComponent<MeshRenderer>().materials[1].SetFloat("_LerpOutline", 0);
        actualMat = Instantiate(trapMat);
        gameObject.GetComponent<MeshRenderer>().materials[1] = actualMat;
    }

    private void Update()
    {
        UpdateState();
        SetTrap();
    }

    private void UpdateState()
    {
        if (state != lastState)
        {
            switch (state)
            {
                case State.Disabled:
                    gameObject.GetComponent<MeshRenderer>().materials[1].SetFloat("_LerpOutline", 0);
                    break;
                case State.Blue:
                    gameObject.GetComponent<MeshRenderer>().materials[1].SetFloat("_LerpOutline", 1);
                    gameObject.GetComponent<MeshRenderer>().materials[1].SetFloat("_WhichColor", 1);
                    break;
                case State.Red:
                    gameObject.GetComponent<MeshRenderer>().materials[1].SetFloat("_LerpOutline", 1);
                    gameObject.GetComponent<MeshRenderer>().materials[1].SetFloat("_WhichColor", 0);
                    break;
            }
        }

        lastState = state;
    }

    private void SetTrap()
    {
        if (isDetecting && state == State.Disabled && player.GetComponent<PlayerController>().interacting)
        {
            if (player.name == "Player_Red")
            {
                state = State.Red;
            }
            else
            {
                state = State.Blue;
            }
        }
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
}
