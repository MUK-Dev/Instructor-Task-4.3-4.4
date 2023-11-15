using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject bomb;
    [SerializeField] private Transform handSlot;
    [SerializeField] private float bombForce = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float health = 6f;
    [SerializeField] private GameObject testingVisual;

    private Animator animator;
    private GameObject player;
    private Rigidbody rb;
    private float gernadeThrowingInterval = 0f;
    private bool isDead = false;
    private float punchDelay = 0f;
    private float punchDelayMax = 2f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
    }

    public void UpdateTestVisual()
    {
        testingVisual.SetActive(!testingVisual.activeSelf);
    }

    private void Update()
    {
        if (isDead || player == null) return;
        float distanceFromPlayer = Vector3.Distance(player.transform.position, transform.position);
        Movement(distanceFromPlayer);
        ThrowGernade();
        Punch(distanceFromPlayer);
    }

    private void Punch(float distanceFromPlayer)
    {
        punchDelay -= Time.deltaTime;

        if (distanceFromPlayer < 2 && punchDelay <= 0)
        {
            punchDelay = punchDelayMax;

            animator.SetTrigger("Punch");
            float capsuleCastRadius = 0.7f;
            float capsuleCastMaxDistance = 1f;

            if (Physics.CapsuleCast(transform.position, transform.up * 10, capsuleCastRadius, transform.forward, out RaycastHit hit, capsuleCastMaxDistance))
            {
                if (hit.collider.gameObject.TryGetComponent<Player>(out Player player))
                {
                    // enemy.UpdateTestVisual();
                    player.ApplyDamage(transform, 2);
                }
            }
        }
    }

    private void ThrowGernade()
    {
        gernadeThrowingInterval -= Time.deltaTime;
        if (gernadeThrowingInterval <= 0)
        {
            gernadeThrowingInterval = Random.Range(4f, 6f);
            animator.SetTrigger("ThrowGernade");
        }
    }

    private void GernadeCreation()
    {
        GameObject newBomb = Instantiate(bomb, handSlot.position, handSlot.rotation);
        Rigidbody bombRigidBody = newBomb.GetComponent<Rigidbody>();
        bombRigidBody.AddForce(transform.forward * bombForce, ForceMode.Impulse);
    }

    private void Movement(float distanceFromPlayer)
    {
        if (player == null) return;

        bool isRunning = false;

        Vector3 directionTowardsPlayer = player.transform.position - transform.position;

        if (distanceFromPlayer > 7)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        transform.forward = Vector3.Slerp(transform.forward, directionTowardsPlayer, Time.deltaTime * rotationSpeed);
        animator.SetBool("IsRunning", isRunning);
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
}
