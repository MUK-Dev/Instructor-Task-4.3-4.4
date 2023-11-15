using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private GameObject grenadeBody;
    [SerializeField] private float timerMax = 10f;

    private Rigidbody rb;
    private float timer = 0f;
    float m_MaxDistance;

    private bool hasBlown = false;
    Collider m_Collider;
    RaycastHit m_Hit;
    bool m_HitDetect;

    private void Start()
    {
        m_MaxDistance = 700.0f;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!hasBlown) timer += Time.deltaTime;

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
}
