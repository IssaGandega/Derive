using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapScriptableObject", menuName = "ScriptableObjects_Trap1")]
public class TrapScriptableObject : ScriptableObject
{
    public float effectDuration = 3;
    public float resetTime = 3;
}
