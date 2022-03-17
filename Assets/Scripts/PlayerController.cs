using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerScriptableObject playerSO;
    
    private float playerSpeed;
    private float knockbackSpeed;

    [Space]
    public Animator animations;
    [SerializeField] 
    private Rigidbody rb;
    
    [Space]
    public GameObject hand;
    public bool interacting;

    private float oldPlayerSpeed;
    public float effectTime;
    private bool dead;
    private Vector2 movementInput = Vector2.zero;
    private Vector3 playerMovementInput;
    private Vector3 moveVector;
    public Vector3 destination;
    private bool isAttacked;
    public bool isDrunk;
    public bool isSoapy;
    private string currentState;
    private bool animationLocked;
    public string weaponName;

    private bool disarmed;
    
    [SerializeField]
    private Transform respawn;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        playerSpeed = playerSO.playerSpeed;
        knockbackSpeed = playerSO.knockbackSpeed;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isDrunk)
        {
            movementInput = context.ReadValue<Vector2>();
            playerMovementInput = new Vector3(-movementInput.x, 0, -movementInput.y);
        }

        else if (isSoapy)
        {
            movementInput = context.ReadValue<Vector2>();
        }
        
        else
        {
            movementInput = context.ReadValue<Vector2>();
            playerMovementInput = new Vector3(movementInput.x, 0, movementInput.y);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && !interacting)
        {
            interacting = true;
            StartCoroutine(InteractingTime());
        }
    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (hand.GetComponentInChildren<WeaponController>() != null)
            {
                PlayAnimation("attack_" + weaponName, true);
            }
        }
    }

    public IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(hand.GetComponentInChildren<WeaponController>().cooldown);
        //animations.SetBool("Attack", false);
    }
    
    private IEnumerator InteractingTime()
    {
        yield return new WaitForSeconds(0.1f);

        interacting = false; 
    }
    
    public void Struggle()
    {
        effectTime -= 0.5f;
    }

    
    private IEnumerator KnockbackHit(Vector3 hit, float power)
    {
        if (hand.GetComponentInChildren<WeaponController>() != null)
        {
            hand.GetComponentInChildren<WeaponController>().DisableWeaponMesh();
            disarmed = true;
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
        
        if (disarmed)
        {
            StartCoroutine(GetWeaponBack());
            disarmed = false;
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

        if (other.transform.CompareTag("Weapon"))
        {
            StartCoroutine(KnockbackHit(other.transform.position, other.transform.GetComponent<WeaponController>().power));
        }
        
        if (other.transform.CompareTag("Water"))
        {
            StartCoroutine(DeathAnimation());
        }
    }

    private IEnumerator DeathAnimation()
    {
        if (hand.GetComponentInChildren<WeaponController>() != null)
        {
            hand.GetComponentInChildren<WeaponController>().DisableWeapon();
        }
        
        PlayAnimation("drowning", true);
        var splash = Pooler.instance.Pop("Plouf");
        splash.transform.position = transform.position;
        Pooler.instance.DelayedDePop(1, "Plouf", splash);
        yield return new WaitForSeconds(1);
        PlayAnimation("idle", false);
        destination = transform.position;
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
        trap.GetComponent<MeshRenderer>().enabled = true;
        trap.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(PlayerStunt(pos));
    }

    public void Soapy(float time)
    {
        if (hand.GetComponentInChildren<WeaponController>() != null)
        {
            hand.GetComponentInChildren<WeaponController>().DisableWeaponMesh();
            disarmed = true;
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
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
        PlayAnimation("stun_filet", true);
        StopSpeed();
        
        yield return new WaitForSeconds(1);

        if (effectTime <= 0)
        {
            animationLocked = false;
            RestoreSpeed();
        }
        else
        {
            StartCoroutine(PlayerStunt(pos));
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
            animationLocked = false;
            playerMovementInput = new Vector3(movementInput.x, 0, movementInput.y);
            if (disarmed)
            {
                hand.GetComponentInChildren<WeaponController>().DisableWeaponMesh();
                disarmed = false;
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
        animationLocked = locked;
        currentState = newState;
    }

    public void UpdateState(string stateName)
    {
        currentState = stateName;
        animationLocked = false;
    }

    private void Update()
    {
        MovePlayer();

        if (playerMovementInput == Vector3.zero)
        {
            if (!animationLocked) PlayAnimation("idle", false);
        }
        
        if (playerMovementInput != Vector3.zero)
        {
            if(!animationLocked) PlayAnimation("run", false);
            gameObject.transform.forward = playerMovementInput.normalized;
        }

    }
}
