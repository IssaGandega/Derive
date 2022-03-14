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

    public State state;
    private State lastState;
    private GameObject player;
    private GameObject fx;
    private Material actualMat;
    
    void Start()
    {
        state = State.Disabled;
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

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<PlayerController>() && state != State.Disabled)
        {
            player = other.gameObject;
            if (player.name.Replace("Player_", "") == state.ToString())
            {
                gameObject.GetComponent<MeshRenderer>().material.SetInt("_Idle", 0);
                
            }
            else
            {
                fx = Pooler.instance.Pop("Crack");
                Pooler.instance.DelayedDePop(2, "Crack", fx);
                fx.transform.position = transform.position;
                fx.transform.parent = transform;
            }
        }
    }
    
}
