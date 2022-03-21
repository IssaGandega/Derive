using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    
    public static GameManager instance;

    [SerializeField] 
    private AudioClip music;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    [SerializeField] private GameObject currentPlayer1;
    [SerializeField] private GameObject currentPlayer2;

    public GameObject introPlayers;
    
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
        AudioManager.PlayMusic(music, 0.25f);
        
        player1.GetComponent<PlayerInput>()
            .SwitchCurrentControlScheme(player1.GetComponent<PlayerInput>().defaultControlScheme, Keyboard.current);
        player2.GetComponent<PlayerInput>()
            .SwitchCurrentControlScheme(player2.GetComponent<PlayerInput>().defaultControlScheme, Keyboard.current);
    }

    public void PlayerControls(GameObject player1, GameObject player2)
    {
        currentPlayer1 = player1;
        currentPlayer2 = player2;
        introPlayers.SetActive(false);
        player1.GetComponent<PlayerInput>()
            .SwitchCurrentControlScheme(player1.GetComponent<PlayerInput>().defaultControlScheme, Keyboard.current);
        player2.GetComponent<PlayerInput>()
            .SwitchCurrentControlScheme(player2.GetComponent<PlayerInput>().defaultControlScheme, Keyboard.current);
    }

    public void ResetRound()
    {
        currentPlayer1.GetComponent<PlayerController>().ResetPlayers();
        currentPlayer2.GetComponent<PlayerController>().ResetPlayers();
    }

    public void GameReset()
    {
        player1.GetComponent<PlayerInput>()
            .SwitchCurrentControlScheme(player1.GetComponent<PlayerInput>().defaultControlScheme, Keyboard.current);
        player2.GetComponent<PlayerInput>()
            .SwitchCurrentControlScheme(player2.GetComponent<PlayerInput>().defaultControlScheme, Keyboard.current);
    }
}
