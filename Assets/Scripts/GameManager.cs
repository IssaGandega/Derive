using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    
    private static GameManager instance;
    
    [SerializeField] 
    private GameObject player1;
    [SerializeField] 
    private GameObject player2;
    [SerializeField] 
    private AudioClip music;
    
    private void Awake()
    {
        AudioManager.Register();
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        AudioManager.PlaySound(music, 0.25f);
        player1.GetComponent<PlayerInput>()
            .SwitchCurrentControlScheme(player1.GetComponent<PlayerInput>().defaultControlScheme, Keyboard.current);
        player2.GetComponent<PlayerInput>()
            .SwitchCurrentControlScheme(player2.GetComponent<PlayerInput>().defaultControlScheme, Keyboard.current);
    }
}
