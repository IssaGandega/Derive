using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/file", fileName = "new data")]
public class Data : ScriptableObject
{
    public int level;
    public int currentRound;
    public int[] whoWonRound;
}
