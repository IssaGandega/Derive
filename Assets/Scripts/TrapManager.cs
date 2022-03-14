using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrapManager : MonoBehaviour
{
    [SerializeField] private Material trapMat;
    [SerializeField] private GameObject smoke;
    [SerializeField] private TrapScriptableObject trapSO;

    public enum State
    {
        Disabled,
        Red,
        Blue
    }

    public enum Type
    {
        Fut,
        Sceau,
        Filet
    }

    public Type type;
    public State state;
    private State lastState;
    
    private Material actualMat;
    public MeshRenderer meshRenderer;
    public GameObject activatingPlayer;
    public GameObject player;
    private float effectDuration;
    private float resetTime;
    private bool isTriggered;
    private bool isDetecting;
    private bool isLoading;
    private GameObject fx;
    private GameObject sploush;

    private void Start()
    {
        state = State.Disabled;
        gameObject.GetComponent<MeshRenderer>().materials[1].SetFloat("_LerpOutline", 0);
        actualMat = Instantiate(trapMat);
        gameObject.GetComponent<MeshRenderer>().materials[1] = actualMat;

        resetTime = trapSO.resetTime;
        effectDuration = trapSO.effectDuration;

    }

    private void Update()
    {
        UpdateState();
        SetTrap();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<PlayerController>()) return;
        isDetecting = true;
        player = other.gameObject;
        
        if (activatingPlayer != player)
        {
            TriggerTrap();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<PlayerController>()) return;
        isDetecting = false;
        player = null;
        effectDuration = trapSO.effectDuration;
    }
    
    private void TriggerTrap()
    {
        if (state != State.Disabled)
        {
            isTriggered = true;
            activatingPlayer = null;
            state = State.Disabled;
            StartCoroutine(TrapTriggeredCo());

            switch (type)
            {
                case Type.Fut:
                    FutEffect();
                    break;
                case Type.Filet:
                    FiletEffect();
                    break;
                case Type.Sceau:
                    SceauEffect();
                    break;
            }
        }
    }

    private void UpdateState()
    {
        if (state != lastState)
        {
            switch (state)
            {
                case State.Disabled:
                    activatingPlayer = null;
                    break;
                case State.Blue:
                    break;
                case State.Red:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        lastState = state;
    }

    private void SetTrap()
    {
        if (isDetecting && state == State.Disabled && player.GetComponent<PlayerController>().interacting && !isLoading)
        {
            player.GetComponent<PlayerController>().StopSpeed();
            activatingPlayer = player;
            isLoading = true;
            fx = Pooler.instance.Pop("Loading");
            fx.transform.position = player.transform.position + Vector3.up;
            meshRenderer = fx.GetComponent<MeshRenderer>();
            StartCoroutine(TrapSetCo());
        }

        if (!isDetecting && isLoading)
        {
            isLoading = false;
            activatingPlayer.GetComponent<PlayerController>().RestoreSpeed();
            activatingPlayer = null;
        }
    }

    public void ChangeTrap()
    {
        StartCoroutine(TrapResetCo());
        smoke.GetComponent<ParticleSystem>().Play();
        GetComponent<MeshRenderer>().enabled = false;
        gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        foreach (var particle in gameObject.transform.GetChild(0).GetComponentsInChildren<ParticleSystem>())
        {
            particle.Play();
        }

        if (type == Type.Sceau)
        {
            gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
            gameObject.transform.GetChild(2).GetComponent<MeshRenderer>().enabled = false;
        }
        
        if (type == Type.Filet) return;
        GetComponent<CapsuleCollider>().enabled = false;


    }
    
    public void ResetTrap()
    {
        smoke.GetComponent<ParticleSystem>().Play();
        GetComponent<MeshRenderer>().enabled = true;
        
        foreach (var particle in gameObject.transform.GetChild(0).GetComponentsInChildren<ParticleSystem>())
        {
            particle.Stop();
        }
        if (type == Type.Sceau)
        {
            gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;
            gameObject.transform.GetChild(2).GetComponent<MeshRenderer>().enabled = true;
        }
        gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        state = State.Disabled;
        
        if (type == Type.Filet) return;
        GetComponent<CapsuleCollider>().enabled = true;
    }

    private IEnumerator TrapResetCo()
    {
        yield return new WaitForSeconds(resetTime);
        if (!isTriggered)
        {
            ResetTrap();
        }
    }

    private IEnumerator TrapTriggeredCo()
    {
        yield return new WaitForSeconds(effectDuration);
        ResetTrap();
        isTriggered = false;
    }

    private IEnumerator TrapSetCo()
    {
        
        yield return new WaitForEndOfFrame();

        if ((meshRenderer.material.GetFloat("_FillAmount") < 1 && isLoading))
        {
            meshRenderer.material.SetFloat("_FillAmount", meshRenderer.material.GetFloat("_FillAmount") + 0.001f);
            StartCoroutine(TrapSetCo());
        }
        
        else if (isLoading || state != State.Disabled)
        {
            state = player.name == "Player_Red" ? State.Red : State.Blue;
            activatingPlayer = player;
            meshRenderer.material.SetFloat("_FillAmount", 0);
            Pooler.instance.DePop("Loading", fx);
            isLoading = false;
            player.GetComponent<PlayerController>().RestoreSpeed();
            ChangeTrap();
        }
        else
        {
            meshRenderer.material.SetFloat("_FillAmount", 0);
            Pooler.instance.DePop("Loading", fx);
            StopCoroutine(TrapSetCo());
        }
    }
    
    private void FutEffect()
    {
        fx = Pooler.instance.Pop("LOW_drunk");
        Pooler.instance.DelayedDePop(2, "LOW_drunk", fx);
        fx.transform.position = player.transform.position + Vector3.up;
        fx.transform.parent = player.transform;
        
        sploush = Pooler.instance.Pop("Sploush");
        Pooler.instance.DelayedDePop(2, "Sploush", sploush);
        sploush.transform.position = player.transform.position;
        sploush.transform.parent = player.transform;
        
        player.GetComponent<PlayerController>().Drunk(2,effectDuration);
    }
    private void FiletEffect()
    {
        fx = Pooler.instance.Pop("LOW_stunt");
        Pooler.instance.DelayedDePop(2, "LOW_stunt", fx);
        fx.transform.position = player.transform.position + Vector3.up;
        fx.transform.parent = player.transform;
        player.GetComponent<PlayerController>().Stunt(effectDuration, transform.position);
    }
    private void SceauEffect()
    {
        sploush = Pooler.instance.Pop("Sploush");
        Pooler.instance.DelayedDePop(2, "Sploush", sploush);
        sploush.transform.position = player.transform.position;
        sploush.transform.parent = player.transform;
        
        player.GetComponent<PlayerController>().Soapy(effectDuration);
    }
}
