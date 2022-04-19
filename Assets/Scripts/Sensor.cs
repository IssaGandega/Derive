using UnityEngine;
using UnityEngine.InputSystem;

public class Sensor : MonoBehaviour
{
    public bool isDetecting;
    public Material baseMaterial;
    private GameObject player;

    private void OnEnable()
    {
        baseMaterial = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        SwapColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerInput>())
        {
            player = other.gameObject;
            isDetecting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterController>())
        {
            GetComponent<MeshRenderer>().material = baseMaterial;
            isDetecting = false;
        }
    }

    private void SwapColor()
    {
        if (isDetecting && player.GetComponent<PlayerController>().interacting)
        {
            GetComponent<MeshRenderer>().material = player.GetComponent<MeshRenderer>().material;
        }
    }
}
