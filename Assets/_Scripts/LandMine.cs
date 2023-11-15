using UnityEngine;

public class LandMine : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private GameObject grenadeBody;

    private float timer = 0f;
    float m_MaxDistance;

    private bool hasBlown = false;

    private void Start()
    {
        m_MaxDistance = 700.0f;
    }

    private void BlowUp()
    {
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            BlowUp();
        }
    }
}
