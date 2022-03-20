using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerScriptableObject playerSO;

    [Space]
    [SerializeField] private Animator animations;
    [SerializeField] private Rigidbody rb;
    
    [Space]
    [SerializeField] private GameObject hitFXParent;
    [SerializeField] private Transform respawn;

    [Space] 
    [SerializeField] private AudioClip hurtSound;
    [Range(0.0f, 1.0f)] 
    public float hurtVolume;
    
    [Space] 
    [SerializeField] private AudioClip splashSound;
    [Range(0.0f, 1.0f)] 
    public float splashVolume;
    
    [Space] 
    [SerializeField] private AudioClip moveSound;

    [Space] 
    [SerializeField] private AudioClip drunkSound;
    [Range(0.0f, 1.0f)] 
    public float drunkVolume;

    [Space]
    public bool interacting;
    public GameObject hand;

    private GameObject weaponFX;
    private GameObject hitFX;
    private GameObject hitFX2;
    private WeaponController weapon;
    private float playerSpeed;
    private float knockbackSpeed;
    private float oldPlayerSpeed;
    private float effectTime;
    
    private bool isDead;
    private bool isAttacking;
    private bool isAttacked;
    private bool animationIsLocked;
    private bool isDisarmed;
    public bool isDrunk;
    public bool isSoapy;
    private bool isStunt;
    
    private Vector2 movementInput = Vector2.zero;
    private Vector3 playerMovementInput;
    private Vector3 moveVector;
    public Vector3 destination;
    
    private string currentState;
    public string weaponName;

    public bool isTurning;
    


    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        playerSpeed = playerSO.playerSpeed;
        knockbackSpeed = playerSO.knockbackSpeed;
    }

    public void OnMove(InputAction.CallbackContext context)
    {

        if (isDrunk && !isAttacking)
        {
            movementInput = context.ReadValue<Vector2>();
            playerMovementInput = new Vector3(-movementInput.x, 0, -movementInput.y);
        }

        else if (isSoapy && !isAttacking)
        {
            movementInput = context.ReadValue<Vector2>();
        }
        
        else
        {
            if (!isAttacking)
            {
                movementInput = context.ReadValue<Vector2>();
                playerMovementInput = new Vector3(movementInput.x, 0, movementInput.y);
            }
        }
        AudioManager.PlaySound(moveSound, Random.Range(0.1f, 0.4f));
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && !interacting)
        {
            interacting = true;
            StartCoroutine(InteractingTime());
        }
    }

    public void OnTurn(InputAction.CallbackContext context)
    {
        isTurning = true;
        StartCoroutine(InteractingTime());
    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (hand.GetComponentInChildren<WeaponController>() != null)
            {
                if (!isAttacking)
                {
                    isAttacking = true;
                    StartCoroutine(AttackCooldown());
                    playerMovementInput = Vector3.zero;
                    weaponFX = Pooler.instance.Pop("FX_" + weaponName);
                    Pooler.instance.DelayedDePop(1, "FX_" + weaponName, weaponFX);

                    weaponFX.transform.parent = gameObject.transform;
                    weaponFX.transform.rotation = new Quaternion(0, 0, 0, 0);
                    
                    weaponFX.transform.position = hitFXParent.transform.position - Vector3.up*5;
                    AudioManager.PlaySound(hand.GetComponentInChildren<WeaponController>().attackSound, Random.Range(0.3f, 0.6f));
                    PlayAnimation("attack_" + weaponName, true);
                    
                }
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(2);
        isAttacking = false;
    }

    private IEnumerator InteractingTime()
    {
        yield return new WaitForSeconds(0.1f);

        interacting = false;
        isTurning = false;
    }
    
    public void Struggle()
    {
        effectTime -= 0.5f;
    }

    
    private IEnumerator KnockbackHit(Vector3 hit, float power)
    {
        AudioManager.PlaySound(hurtSound, hurtVolume);
        if (hand.GetComponentInChildren<WeaponController>() != null)
        {
            hand.GetComponentInChildren<WeaponController>().DisableWeaponMesh();
            isDisarmed = true;
        }

        if (isStunt)
        {
            RestoreSpeed();
        }
        
        
        PlayAnimation("hurt", true);
        isAttacked = true;
        
        destination = transform.position +
          new Vector3(
              (transform.position.x - hit.x) *  power,
              0,
              (transform.position.z - hit.z) * power
          );
        
        
        while ((isAttacked  && (destination - transform.position).magnitude > 4))
        {
            transform.position = Vector3.Lerp(transform.position, destination, knockbackSpeed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        
        if (isDisarmed)
        {
            StartCoroutine(GetWeaponBack());
            isDisarmed = false;
        }        
        
        isAttacked = false;
    }

    private IEnumerator GetWeaponBack()
    {
        yield return new WaitForSeconds(0.7f);
        hand.GetComponentInChildren<WeaponController>().DisableWeaponMesh();
    }

    private void OnCollisionEnter(Collision other)
    {
        if ((other.transform.CompareTag("Trap") || other.transform.CompareTag("Spawner")) && isAttacked)
        {
            StopAllCoroutines();
            isAttacked = false;
        }

        if (other.transform.CompareTag("Water"))
        {
            StartCoroutine(DeathAnimation());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Weapon"))
        {
            hitFX = Pooler.instance.Pop("Hit1");
            hitFX2 = Pooler.instance.Pop("Hit2");

            hitFX.transform.parent = hitFXParent.transform;
            hitFX2.transform.parent = hitFXParent.transform;
            
            hitFX.transform.position = hitFXParent.transform.position;
            hitFX2.transform.position = hitFXParent.transform.position;
            hitFX2.transform.rotation = new Quaternion(0, 0, 0, 0);
            
            Pooler.instance.DelayedDePop(2, "Hit1", hitFX);
            Pooler.instance.DelayedDePop(2, "Hit2", hitFX2);
            
            hitFX2.transform.rotation = Quaternion.LookRotation(transform.position - other.transform.parent.parent.position);

            StartCoroutine(KnockbackHit(other.transform.parent.parent.position, other.transform.GetComponent<WeaponController>().power));
        }
    }

    private IEnumerator DeathAnimation()
    {
        if (hand.GetComponentInChildren<WeaponController>() != null)
        {
            hand.GetComponentInChildren<WeaponController>().DisableWeapon();
        }
        
        PlayAnimation("drowning", true);
        AudioManager.PlaySound(splashSound, splashVolume);
        var splash = Pooler.instance.Pop("Plouf");
        splash.transform.position = transform.position;
        Pooler.instance.DelayedDePop(1, "Plouf", splash);
        destination = transform.position;
        
        yield return new WaitForSeconds(1);
        PlayAnimation("idle", false);
        var nb = Random.Range(0, respawn.transform.childCount);
        transform.position = respawn.GetChild(nb).position;
        

        effectTime = 0;
    }

    private void MovePlayer()
    {
        if (hand.GetComponentInChildren<WeaponController>() != null)
        {
            moveVector = playerMovementInput * (playerSpeed - hand.GetComponentInChildren<WeaponController>().weight);
        }
        else
        {
            moveVector = playerMovementInput * playerSpeed;
        }
        rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
    }

    public void Drunk(float speed, float time)
    {
        AudioManager.PlaySound(drunkSound, drunkVolume);
        effectTime = time;
        playerSpeed = speed;
        isDrunk = true;
        StartCoroutine(PlayerDrunk());
    }
    
    private IEnumerator PlayerDrunk()
    {
        yield return new WaitForSeconds(1);

        if (effectTime <= 0)
        {
            isDrunk = false;
            RestoreSpeed();
        }

        else
        {
            effectTime --;
            StartCoroutine(PlayerDrunk());
        }

    }

    public void Stunt(float time, Vector3 pos, GameObject trap)
    {
        effectTime = time;
        isStunt = true;
        StopSpeed();
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
        PlayAnimation("stun_filet", true);
        trap.GetComponent<MeshRenderer>().enabled = true;
        trap.transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(PlayerStunt(pos));
    }

    public void Soapy(float time)
    {
        if (hand.GetComponentInChildren<WeaponController>() != null)
        {
            weapon = hand.GetComponentInChildren<WeaponController>();
            weapon.DisableWeapon();
            isDisarmed = true;
        }
        effectTime = time;
        PlayAnimation("slide", true);
        StartCoroutine(PlayerSoapy());
    }

    public void StopSpeed()
    {
        playerSpeed = 0;
    }

    public void RestoreSpeed()
    {
        playerSpeed = playerSO.playerSpeed;
    }

    private IEnumerator PlayerStunt(Vector3 pos)
    {

        yield return new WaitForSeconds(1);

        if (effectTime <= 0)
        {
            animationIsLocked = false;
            RestoreSpeed();
        }
        else
        {
            StartCoroutine(PlayerStunt(pos));
            isStunt = false;
            effectTime--;
        }
        
    }

    private IEnumerator PlayerSoapy()
    {
        isSoapy = true;
        
        yield return new WaitForSeconds(1);
        if (effectTime <= 0)
        {
            isSoapy = false;
            animationIsLocked = false;
            playerMovementInput = new Vector3(movementInput.x, 0, movementInput.y);
            if (isDisarmed)
            {
                weapon.DisableWeapon();
                isDisarmed = false;
            }
        }
        else
        {
            StartCoroutine(PlayerSoapy());
            effectTime--;
        }
        
    }
    
    public void PlayAnimation (string newState, bool locked)
    {
        if (currentState == newState) return;
        GetComponent<Animator>().Play(newState);
        animationIsLocked = locked;
        currentState = newState;
    }

    public void UpdateState(string stateName)
    {
        currentState = stateName;
        animationIsLocked = false;
        isAttacking = false;
    }

    private void Update()
    {
        MovePlayer();

        if (playerMovementInput == Vector3.zero)
        {
            AudioManager.StopSound(moveSound);
            if (!animationIsLocked) PlayAnimation("idle", false);
        }
        
        if (playerMovementInput != Vector3.zero)
        {
            if(!animationIsLocked) PlayAnimation("run", false);
            gameObject.transform.forward = playerMovementInput.normalized;
        }

    }
}
