using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float bombForce = 10f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private GameObject bomb;
    [SerializeField] private GameObject stickyBomb;
    [SerializeField] private GameObject mine;
    [SerializeField] private Transform handSlot;
    [SerializeField] private int health = 10;
    private Animator animator;
    private Rigidbody rb;
    private bool isDead = false;

    private int availableStickyGernades = 0;
    private int availableMines = 0;
    private bool hasMultiBombActive = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isDead) return;

        Movement();
        ThrowGernade();
        Punch();
    }

    private void Punch()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            animator.SetTrigger("Punch");
            float capsuleCastRadius = 0.7f;
            float capsuleCastMaxDistance = 1f;

            if (Physics.CapsuleCast(transform.position, transform.up * 10, capsuleCastRadius, transform.forward, out RaycastHit hit, capsuleCastMaxDistance))
            {
                if (hit.collider.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.ApplyDamage(transform, 5);
                }
            }
        }
    }

    IEnumerator RemoveMultiBombEffect()
    {
        yield return new WaitForSeconds(10);
        hasMultiBombActive = false;
    }

    private void ThrowGernade()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("ThrowGernade");
        }
    }

    private void GernadeCreation()
    {
        if (hasMultiBombActive)
        {
            GameObject bomb1 = Instantiate(bomb, handSlot.position, handSlot.rotation);
            Rigidbody bomb1RigidBody = bomb1.GetComponent<Rigidbody>();
            bomb1RigidBody.AddForce(transform.forward * bombForce, ForceMode.Impulse);

            GameObject newBomb2 = Instantiate(bomb, handSlot.position, handSlot.rotation);
            Rigidbody bomb2RigidBody = newBomb2.GetComponent<Rigidbody>();
            bomb2RigidBody.AddForce((transform.forward + Vector3.left) * bombForce, ForceMode.Impulse);

            GameObject newBomb3 = Instantiate(bomb, handSlot.position, handSlot.rotation);
            Rigidbody bomb3RigidBody = newBomb3.GetComponent<Rigidbody>();
            bomb3RigidBody.AddForce((transform.forward + Vector3.right) * bombForce, ForceMode.Impulse);
        }
        else if (availableStickyGernades > 0)
        {
            GameObject newBomb = Instantiate(stickyBomb, handSlot.position, handSlot.rotation);
            Rigidbody bombRigidBody = newBomb.GetComponent<Rigidbody>();
            bombRigidBody.AddForce(transform.forward * bombForce, ForceMode.Impulse);
            availableStickyGernades--;
        }
        else if (availableMines > 0)
        {
            Vector3 spawningPosition = new Vector3(transform.position.x, 0.2f, transform.position.z);
            Instantiate(mine, spawningPosition, transform.rotation);
            availableMines--;
        }
        else
        {
            GameObject newBomb = Instantiate(bomb, handSlot.position, handSlot.rotation);
            Rigidbody bombRigidBody = newBomb.GetComponent<Rigidbody>();
            bombRigidBody.AddForce(transform.forward * bombForce, ForceMode.Impulse);
        }
    }

    private void Movement()
    {
        Vector2 inputVector = new Vector2(0, 0);
        bool isRunning = false;

        if (Input.GetKey(KeyCode.W))
        {
            inputVector.y = +1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x = +1;
        }

        if (inputVector == Vector2.zero)
        {
            isRunning = false;
        }
        else
        {
            isRunning = true;
        }

        animator.SetBool("IsRunning", isRunning);

        inputVector = inputVector.normalized;
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        transform.position += moveDir * speed * Time.deltaTime;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
    }

    public void ApplyDamage(Transform damagingObject, float knockBackForce)
    {
        health--;

        if (health < 1)
        {
            isDead = true;
            animator.SetBool("IsDead", isDead);
            Destroy(gameObject, 3);
        }
        else
        {
            rb.AddExplosionForce(knockBackForce, damagingObject.position, 10f, .1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "StickyGernades":
                availableStickyGernades += 3;
                Destroy(other.gameObject);
                break;
            case "Mines":
                availableMines += 3;
                Destroy(other.gameObject);
                break;
            case "Health":
                health++;
                Destroy(other.gameObject);
                break;
            case "MultiBomb":
                hasMultiBombActive = true;
                StartCoroutine(RemoveMultiBombEffect());
                Destroy(other.gameObject);
                break;
            default:
                break;
        }
    }

}
