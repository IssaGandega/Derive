using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class SpawnerManager : MonoBehaviour
{
    public GameObject[] weapons;

    public int selectedWeapon = 0;
    
    private GameObject hand;
    private GameObject prefab;
    public bool isDetecting;
    private GameObject player;

    private GameObject weaponHolder;
    private static readonly int Attack = Animator.StringToHash("Attack");
    public GameObject[] armes;


    private void Start()
    {
        SpawnRandomWeapon();
        StartCoroutine(RerollWeapons());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerInput>())
        {
            weaponHolder = other.GetComponentInChildren<PlayerController>().hand;
            isDetecting = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerInput>())
        {
            weaponHolder = null;
            isDetecting = false;
        }
    }

    private void Update()
    {
        prefab.transform.Rotate(new Vector3(0f, 1, 0f) * Time.deltaTime * 100);
        if (weaponHolder != null)
        {
            PickupWeapon();
        }
    }

    private void SpawnRandomWeapon()
    {
        int test = Random.Range(0, weapons.Length);
        prefab = weapons[test];
        prefab = Pooler.instance.Pop(prefab.name);
        prefab.transform.position = transform.position + Vector3.up*3f;
        GetComponentInChildren<ParticleSystem>().Play();
    }
    
    private IEnumerator RerollWeapons()
    {
        yield return new WaitForSeconds(5);
        Pooler.instance.DePop(prefab.name.Split('(')[0], prefab);
        SpawnRandomWeapon();
        StartCoroutine(RerollWeapons());
    }

    void PickupWeapon()
    {
        if (isDetecting && player.GetComponent<PlayerController>().interacting)
        {
            foreach (Transform weapon in weaponHolder.transform)
            {
                if (weapon.gameObject.name == prefab.name.Replace("(Clone)", ""))
                {
                    StartCoroutine(GetWeapon(weapon));
                    //player.GetComponent<Animator>().Play("pick_up");
                    player.GetComponent<PlayerController>().PlayAnimation("pick_up", true);
                    player.GetComponent<PlayerController>().weaponName = weapon.name;
                    //player.GetComponent<Animator>().SetBool(weapon.name, true);
                }
                else
                { 
                    weapon.gameObject.SetActive(false);
                    //player.GetComponent<Animator>().SetBool(weapon.name, false);
                }
            }
            StopCoroutine(player.GetComponent<PlayerController>().AttackCooldown());
            //player.GetComponent<PlayerController>().animations.SetBool(Attack, false);
        }

        player.GetComponent<PlayerController>().interacting = false;
    }

    private IEnumerator GetWeapon(Transform weapon)
    {
        yield return new WaitForSeconds(0.5f);
        weapon.gameObject.SetActive(true);
    }
}
