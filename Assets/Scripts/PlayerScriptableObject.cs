using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerScriptableObject", menuName = "ScriptableObjects_Player1")]
public class PlayerScriptableObject : ScriptableObject
{
    public float playerSpeed = 1;
    public float knockbackSpeed = 1;
}
