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
}
