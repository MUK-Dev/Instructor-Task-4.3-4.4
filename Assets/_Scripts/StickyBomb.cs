using UnityEngine;

public class StickyBomb : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private GameObject grenadeBody;
    [SerializeField] private float timerMax = 10f;

    private Rigidbody rb;
    private float timer = 0f;
    float m_MaxDistance;

    private bool hasBlown = false;
    private bool shouldStartTimer = false;

    private void Start()
    {
        m_MaxDistance = 700.0f;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!hasBlown && shouldStartTimer) timer += Time.deltaTime;

        if (timer >= timerMax && !hasBlown)
        {
            rb.freezeRotation = true;
            hasBlown = true;
            Instantiate(explosion, transform);
            Destroy(grenadeBody);
            Destroy(gameObject, 10f);
            RaycastHit[] hits = Physics.BoxCastAll(transform.position + Vector3.up * 0.3f,
            transform.localScale * 2f, transform.position,
                transform.rotation, m_MaxDistance);
            float knockBackForce = 1000;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.ApplyDamage(transform, knockBackForce);
                }
                else if (hit.collider.gameObject.TryGetComponent<Player>(out var player))
                {
                    player.ApplyDamage(transform, knockBackForce);
                }
            }

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //? Draw a cube that extends to where the hit exists
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.3f, transform.localScale * 3.5f);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            transform.SetParent(other.gameObject.transform);
            rb.freezeRotation = true;
            rb.isKinematic = true;
        }
        shouldStartTimer = true;
    }
}
