using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

	[SerializeField] private PlayerController playerController;

	public float power = 0;
	public float cooldown = 0;
	public float weight;
	public bool isActive;
	private GameObject prefab;
	private GameObject weapon;

	private void Start()
	{
		//isActive = gameObject.active;
	}

	//Put AudioSource on prefabs
	public AudioClip attackSound;
	public ParticleSystem attackParticles;
	public AnimationClip animationClip;

	[SerializeField] private WeaponScriptableObject weaponSO;

	private void OnEnable()
	{
		prefab = weaponSO.prefab;
		power = weaponSO.power;
		cooldown = weaponSO.cooldown;
		weight = weaponSO.weight;
		animationClip = weaponSO.animationClip;
	}


}
