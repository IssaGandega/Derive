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
    [SerializeField] private AudioClip setSound;

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
    public Material childMat;
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
    
    private static readonly int FillAmount = Shader.PropertyToID("_FillAmount");
    private static readonly int LerpOutline = Shader.PropertyToID("_LerpOutline");
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

    private void OnEnable()
    {
        state = State.Disabled;
        gameObject.GetComponent<MeshRenderer>().materials[1].SetFloat(LerpOutline, 0);
        actualMat = Instantiate(trapMat);
        childMat = Instantiate(transform.GetChild(0).GetComponent<MeshRenderer>().material);
        transform.GetChild(0).GetComponent<MeshRenderer>().material = childMat;
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
                    childMat.SetColor(OutlineColor, new Color(0, 0, 10,1));
                    break;
                case State.Red:
                    childMat.SetColor(OutlineColor, new Color(10, 0,0,1));
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
            AudioManager.PlaySound(setSound, 0.2f);
            player.GetComponent<PlayerController>().StopSpeed();
            player.GetComponent<PlayerController>().PlayAnimation("pick_up", true);
            activatingPlayer = player;
            isLoading = true;
            fx = Pooler.instance.Pop("Loading");
            Pooler.instance.DelayedDePop(2, "Loading", fx);
            fx.transform.position = transform.position + Vector3.up*5;
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
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
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
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
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

        if ((meshRenderer.material.GetFloat(FillAmount) < 1 && isLoading))
        {
            // Ici modif durée activation piège
            meshRenderer.material.SetFloat(FillAmount, meshRenderer.material.GetFloat(FillAmount) + 0.03f);
            StartCoroutine(TrapSetCo());
        }
        
        else if (isLoading || state != State.Disabled)
        {
            state = player.name == "Player_Red" ? State.Red : State.Blue;
            activatingPlayer = player;
            meshRenderer.material.SetFloat(FillAmount, 0);
            Pooler.instance.DePop("Loading", fx);
            isLoading = false;
            player.GetComponent<PlayerController>().RestoreSpeed();
            ChangeTrap();
        }
        else
        {
            meshRenderer.material.SetFloat(FillAmount, 0);
            Pooler.instance.DePop("Loading", fx);
            StopCoroutine(TrapSetCo());
        }
    }
    
    private void FutEffect()
    {
        fx = Pooler.instance.Pop("Drunk_Master");
        Pooler.instance.DelayedDePop(2, "Drunk_Master", fx);
        fx.transform.position = player.transform.position + Vector3.up*6;
        fx.transform.parent = player.transform;
        
        sploush = Pooler.instance.Pop("Sploush");
        Pooler.instance.DelayedDePop(2, "Sploush", sploush);
        sploush.transform.position = player.transform.position;
        sploush.transform.parent = player.transform;
        
        // Effet de la bière modif speed
        player.GetComponent<PlayerController>().Drunk(5,effectDuration);
    }
    private void FiletEffect()
    {
        fx = Pooler.instance.Pop("LOW_stun_Fx");
        Pooler.instance.DelayedDePop(2, "LOW_stun_Fx", fx);
        fx.transform.position = player.transform.position + Vector3.up*6;
        fx.transform.parent = player.transform;
        
        player.GetComponent<PlayerController>().Stunt(effectDuration, transform.position, gameObject);
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
