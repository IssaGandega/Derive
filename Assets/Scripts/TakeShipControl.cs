using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeShipControl : MonoBehaviour
{
    public bool isDetecting;
    
    
    private bool canChange = true;
    private int initGouvValue;
    private EncoderReader encoder;

    [SerializeField] private float rotationSpeed;
    [SerializeField] Material boatMat;
    [SerializeField] private GameObject uduino;
    
    [SerializeField] private GameObject traps;
    [SerializeField] private GameObject ropes;

    [SerializeField] private AudioClip turnSound;

    public List<GameObject> playersInside;
    private static readonly int LerpOutline = Shader.PropertyToID("_LerpOutline");
    private static readonly int WhichColor = Shader.PropertyToID("_WhichColor");

    private void Awake()
    {
        boatMat.SetFloat(LerpOutline, 0);
    }

    private void Start()
    {
        encoder = uduino.GetComponent<EncoderReader>();
        initGouvValue = uduino.GetComponent<EncoderReader>().gouvCurrentValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            initGouvValue = encoder.gouvCurrentValue;
            playersInside.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            initGouvValue = encoder.gouvCurrentValue;
            playersInside.Remove(other.gameObject);
        }
    }

    private void LateUpdate()
    {
        TakeControl();
    }

    private void TakeControl()
    {
        if (playersInside.Count == 1 && canChange) //&& (encoder.gouvCurrentValue > initGouvValue + 10 || encoder.gouvCurrentValue < initGouvValue - 10))
        {
            //To Remove
            if (playersInside[0].GetComponent<PlayerController>().isTurning)
            {
                initGouvValue = encoder.gouvCurrentValue;
                playersInside[0].GetComponent<PlayerController>().PlayAnimation("turn_runner", true);
                StartCoroutine(ChangeOwnership());
                playersInside[0].GetComponent<PlayerController>().isTurning = false;
            }
        }
    }

    private IEnumerator ChangeOwnership()
    {
        canChange = false;
        AudioManager.PlaySound(turnSound, 0.2f);
        StartCoroutine(RotateRudder());

        if (playersInside[0].name == "Player_Red")
        {
            boatMat.SetFloat(LerpOutline, 1);
            boatMat.SetFloat(WhichColor, 0);
            foreach (var rope in ropes.GetComponentsInChildren<RopesManager>())
            {
                if (rope.type == RopesManager.Type.Regular)
                {
                    rope.state = RopesManager.State.Red;
                }
            }
            foreach (var trap in traps.GetComponentsInChildren<TrapManager>())
            {
                if (trap.state == TrapManager.State.Disabled)
                {
                    trap.state = TrapManager.State.Red;
                    trap.activatingPlayer = playersInside[0];
                    trap.ChangeTrap();
                }
                else if (trap.state == TrapManager.State.Blue)
                {
                    trap.state = TrapManager.State.Disabled;
                    trap.activatingPlayer = null;
                    trap.ResetTrap();
                }
            }
        }
        else if (playersInside[0].name == "Player_Blue")
        {
            boatMat.SetFloat(LerpOutline, 1);
            boatMat.SetFloat(WhichColor, 1);
            foreach (var rope in ropes.GetComponentsInChildren<RopesManager>())
            {
                if (rope.type == RopesManager.Type.Regular)
                {
                    rope.state = RopesManager.State.Blue;
                }
            }
            foreach (var trap in traps.GetComponentsInChildren<TrapManager>())
            {
                if (trap.state == TrapManager.State.Disabled)
                {
                    trap.state = TrapManager.State.Blue;
                    trap.activatingPlayer = playersInside[0];
                    trap.ChangeTrap();
                }
                else if (trap.state == TrapManager.State.Red)
                {
                    trap.state = TrapManager.State.Disabled;
                    trap.activatingPlayer = null;
                    trap.ResetTrap();
                }
            }
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
