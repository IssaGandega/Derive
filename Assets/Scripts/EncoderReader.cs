using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Uduino;

public class EncoderReader : MonoBehaviour
{
    [SerializeField] private GameObject bluePlayer;
    [SerializeField] private GameObject redPlayer;
    
    
    public int BlueEncoderInitialeValue, BlueEncoderCurrentValue;
    public int RedEncoderInitialeValue, RedEncoderCurrentValue;
    public int gouvInitialeValue, gouvCurrentValue;
    public bool gouvTurned = false;

    void Start()
    {
        UduinoManager.Instance.OnDataReceived += DataReveived;
    }

    // Ne cherche pas à lire les valeurs envoyé par Arduino.
    private void Update()
    {
        UpdateRead();
        if (gouvCurrentValue != gouvInitialeValue)
        {
            gouvInitialeValue = gouvCurrentValue;
            gouvTurned = true;
        }
        else gouvTurned = false;
    }

    //Récupère les valeurs envoyé par Arduino
    void DataReveived(string data, UduinoDevice board)
    {
        //Arduino détecte la valeur de l'encodeur est l'envoie sous forme de texte qui contient deux informations le numéro du joueur et la valeur de l'envodeur
        // Exemple : Si arduino envoie 1 198 alors le premier chiffre indique que c'est le joueur 1 et le reste c'est la valeur de l'encodeur.
        // Si le nombre envoyé c'est 3 990 alors c'est le gouvernail qui a comme valeur 990
        
        //Récupère la valeur d'un encodeur
        string dataTemp = "";
        for (int i = 2; i < data.Length; i++)
        {
            dataTemp += data[i];
        }
        //Vérifie quelle encodeur à envoyé la valeur
        if (data[0] == '1')
        {
            BlueEncoderCurrentValue = int.Parse(dataTemp);
        }
        else if (data[0] == '2')
        {
            RedEncoderCurrentValue = int.Parse(dataTemp);
        }
        else
        {
            gouvCurrentValue = int.Parse(dataTemp);
        }
    }
    public void StartRead(bool firstPlayer)
    { 
        if (firstPlayer) BlueEncoderInitialeValue = BlueEncoderCurrentValue;
        else RedEncoderInitialeValue = RedEncoderCurrentValue;    
    }

    public void UpdateRead()
    {
        if (BlueEncoderCurrentValue >= BlueEncoderInitialeValue + 40)
        {
            BlueEncoderInitialeValue = BlueEncoderCurrentValue;
            bluePlayer.GetComponent<PlayerController>().Struggle();
        }
        else if (BlueEncoderCurrentValue <= BlueEncoderInitialeValue - 40)
        {
            BlueEncoderInitialeValue = BlueEncoderCurrentValue;
            bluePlayer.GetComponent<PlayerController>().Struggle();
        }

        if (RedEncoderCurrentValue >= RedEncoderInitialeValue + 40)
        {
            RedEncoderInitialeValue = RedEncoderCurrentValue;
            redPlayer.GetComponent<PlayerController>().Struggle();
        }
        else if(RedEncoderCurrentValue <= RedEncoderInitialeValue - 40)
        {
            RedEncoderInitialeValue = RedEncoderCurrentValue;
            redPlayer.GetComponent<PlayerController>().Struggle();
        }
    }
}
