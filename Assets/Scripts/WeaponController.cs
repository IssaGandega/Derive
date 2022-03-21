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
	public float cooldown;
	private GameObject weapon;
	public float inertia;
	

	//Put AudioSource on prefabs
	public AudioClip attackSound;

	
	[SerializeField] private WeaponScriptableObject weaponSO;

	private void OnEnable()
	{
		power = weaponSO.power;
		weight = weaponSO.weight;
		attackSound = weaponSO.attackSound;
		cooldown = weaponSO.cooldown;
		inertia = weaponSO.inertia;
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
