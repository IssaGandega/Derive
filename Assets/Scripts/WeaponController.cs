using System;
using System.Collections;
using System.Collections.Generic;
using Uduino;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

	[SerializeField] private PlayerController playerController;

	public float power = 0;
	public float weight;
	private GameObject weapon;
	

	//Put AudioSource on prefabs
	public AudioClip attackSound;

	
	[SerializeField] private WeaponScriptableObject weaponSO;

	private void OnEnable()
	{
		power = weaponSO.power;
		weight = weaponSO.weight;
		attackSound = weaponSO.attackSound;
	}

	public void DisableWeapon()
	{
		gameObject.SetActive(!gameObject.activeSelf);
	}

	public void DisableWeaponMesh()
	{
		GetComponent<MeshRenderer>().enabled = !GetComponent<MeshRenderer>().enabled;
	}

}
