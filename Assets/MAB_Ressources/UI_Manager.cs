using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    [SerializeField] private AudioClip startSound;
    [SerializeField] private GameObject pooler;


    //Levels
    [SerializeField] private GameObject[] levels;
    
    //Data
    [SerializeField] private Data data;
    public byte[] playerspoints;

    //General UI
    [SerializeField] private GameObject[] UIScreen;

    //End Round UI
    [SerializeField] private GameObject[] playerRuban;
    [SerializeField] private TextMeshProUGUI[] currentRoundText;
    [SerializeField] private Color[] pointColor; // 0 = Win; 1 = Loose
    [SerializeField] private Image[] bluePointImages, redPointImages; // 0 - 2 Blue Winner else red winner

    //End Game UI
    [SerializeField] private TextMeshProUGUI[] resultText; // 0 == Blue player and 1 == Red Player
    [SerializeField] private Image[] bluePlayerPoint, redPlayerPoint;
    
    //Ready UI
    [SerializeField] private GameObject[] bluePlayerOutline, redPlayerOutline;
    private bool warningPlayed;
    public bool[] playersReady; // 0 = Blue; 1 = Red
    private Coroutine warningCoroutine;

    public GameObject currentLD;
    public bool blockPlayers;

    void Start()
    {
        if (instance != null) Destroy(this);
        else instance = this;
        playersReady = new bool[2];
        if (data.currentRound == 0)
        {
            UIScreen[0].SetActive(true);
        }
        foreach (int i in data.whoWonRound)
        {
            if (i == 1) playerspoints[0]++;
            else if (i == 2) playerspoints[1]++;
        }

        //StartCoroutine(DebugCoroutine());
    }
    
    /*
    private IEnumerator DebugCoroutine()
    {
        if (data.currentRound == 0)
        {
            yield return new WaitForSeconds(3);
            while (true)
            {
                yield return null;
                PlayerReady(1);
                if (UnityEngine.Random.Range(0, 1001) == 500) PlayerReady(2);
            }
        }
        for (int i = 0; i < 1; i++)
        {
            yield return new WaitForSeconds(5);
            EndRound(UnityEngine.Random.Range(1, 3));
        }
    }*/

    public void PlayerReady(int player)
    {
        if (!warningPlayed)
        {
            if (warningCoroutine != null) return;
            warningCoroutine = StartCoroutine(StartWarning());
            return;
        }

        if (player == 1 && !playersReady[0])
        {
            playersReady[0] = true;
            foreach (GameObject obj in bluePlayerOutline)
            {
                obj.SetActive(true);
            }
        }
        if (player == 2 && !playersReady[1])
        {
            playersReady[1] = true;
            foreach (GameObject obj in redPlayerOutline)
            {
                obj.SetActive(true);
            }
        }

        if (playersReady[0] && playersReady[1])
        {
            StartCoroutine(StartGame());
        }
    }

    private void ResetUI()
    {
        foreach (var ui in UIScreen)
        {
            ui.SetActive(false);
        }
    }

    private IEnumerator StartWarning()
    {
        UIScreen[5].SetActive(true);
        yield return new WaitForSeconds(1);
        UIScreen[0].SetActive(false);
        UIScreen[1].SetActive(true);
        yield return new WaitForSeconds(5);
        UIScreen[1].SetActive(false);
        UIScreen[2].SetActive(true);
        warningPlayed = true;
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(5);
        data.level = Random.Range(0, 3);
        data.currentRound = 1;
        levels[data.level].SetActive(true);
        currentLD = levels[data.level];
        GameManager.instance.PlayerControls(currentLD.GetComponent<InfosLD>().bluePlayer, currentLD.GetComponent<InfosLD>().redPlayer);
        AudioManager.PlaySound(startSound, 0.3f);
        ResetUI();
        //SceneManager.LoadScene(data.level);
    }
    
    public void EndRound(int winner)
    {
        //Update data
        playerspoints[winner - 1]++;
        data.whoWonRound[data.currentRound - 1] = winner;

        if (playerspoints[0] == 3 || playerspoints[1] == 3)
        {
            StartCoroutine(DisplayGameResultCoroutine((byte) winner));
        }
        else StartCoroutine(DisplayEndRoundCoroutine((byte) winner));
    }

    //Called to display round results
    private IEnumerator DisplayEndRoundCoroutine(byte winner)
    {
        blockPlayers = true;
        //Display end round UI
        for (int i = 0; i < bluePointImages.Length / 2; i++)
        {
            if (winner == 1)
            {
                if (i < playerspoints[0]) bluePointImages[i].color = pointColor[0];
                else bluePointImages[i].color = pointColor[1];
                if (i < playerspoints[1]) redPointImages[i + 3].color = pointColor[0];
                else redPointImages[i + 3].color = pointColor[1];
            }
            else
            {
                if (i < playerspoints[1]) redPointImages[i].color = pointColor[0];
                else redPointImages[i].color = pointColor[1];
                if (i < playerspoints[0]) bluePointImages[i + 3].color = pointColor[0];
                else bluePointImages[i + 3].color = pointColor[1];
            }
        }

        if (winner == 1)
        {
            currentRoundText[0].text = "ROUND " + data.currentRound.ToString();
            currentRoundText[1].text = "ROUND " + data.currentRound.ToString();
            playerRuban[0].SetActive(true);
        }
        else
        {
            currentRoundText[2].text = "ROUND " + data.currentRound.ToString();
            currentRoundText[3].text = "ROUND " + data.currentRound.ToString();
            playerRuban[1].SetActive(true);
        }

        UIScreen[3].SetActive(true);
        yield return new WaitForSeconds(5);

        data.currentRound++;
        ResetUI();
        GameManager.instance.ResetRound();
        AudioManager.PlaySound(startSound);
        playerRuban[0].SetActive(false);
        playerRuban[1].SetActive(false);
        blockPlayers = false;
    }

    //is called to display who won and who loose.
    private IEnumerator DisplayGameResultCoroutine(byte winner)
    {
        AudioManager.PlaySound(startSound);
        if (winner == 1) 
        {
            resultText[0].text = "VICTORY";
            resultText[1].text = "DEFEAT";
        }
        else
        {
            resultText[1].text = "VICTORY";
            resultText[0].text = "DEFEAT";
        }

        for (int i = 0; i < bluePlayerPoint.Length; i++)
        {
            if (data.whoWonRound[i] == 1)
            {
                bluePlayerPoint[i].color = pointColor[0];
                redPlayerPoint[i].color = pointColor[1];
            }
            else if (data.whoWonRound[i] == 2)
            {
                redPlayerPoint[i].color = pointColor[0];
                bluePlayerPoint[i].color = pointColor[1];
            }
            else
            {
                redPlayerPoint[i].color = pointColor[1];
                bluePlayerPoint[i].color = pointColor[1];
            }
        }
        UIScreen[4].SetActive(true);
        yield return new WaitForSeconds(5);
        UIScreen[5].SetActive(true);
        yield return new WaitForSeconds(1f);
        data.currentRound = 0;
        for (int i = 0; i < data.whoWonRound.Length; i++)
        {
            data.whoWonRound[i] = 0;
        }
        
        playersReady[0] = false;
        playersReady[1] = false;
        playerspoints[0] = 0;
        playerspoints[1] = 0;
        warningPlayed = false;
        warningCoroutine = null;
        foreach (GameObject obj in redPlayerOutline)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in bluePlayerOutline)
        {
            obj.SetActive(false);
        }
        GameManager.instance.introPlayers.SetActive(true);
        GameManager.instance.GameReset();
        foreach (Transform pool in pooler.transform)
        {
            if (pool.gameObject.activeSelf)
            {
                Pooler.instance.DePop(pool.name.Replace("(Clone)", ""), pool.gameObject);
            }
        }
        ResetUI();
        
        if (data.currentRound == 0)
        {
            UIScreen[0].SetActive(true);
        }
        foreach (int i in data.whoWonRound)
        {
            if (i == 1) playerspoints[0]++;
            else if (i == 2) playerspoints[1]++;
        }
        
        currentLD.SetActive(false);
    }  
}