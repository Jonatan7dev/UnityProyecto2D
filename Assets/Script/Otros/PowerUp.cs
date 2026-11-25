
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Power Up Type")]
    public bool healing;
    public bool strength;
    public bool shield;

    [Header("Power Up Values")]
    public float healingAmount = 25f;
    public float strengthAmount = 10f;
    public float shieldAmount = 50f;

    [Header("Floating")]
    [SerializeField] private float floatAmplitude = 0.2f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        float newY = initialPosition.y +
            Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        transform.position = new Vector3(
            initialPosition.x,
            newY,
            initialPosition.z
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectPowerUp(other.gameObject);
        }
    }

    private void CollectPowerUp(GameObject player)
    {
        if (healing)
        {
            Debug.Log("Healing: " + healingAmount);
            // Aplicar curación al jugador
        }
        else if (strength)
        {
            Debug.Log("Strength: " + strengthAmount);
            // Aumentar la fuerza del jugador
        }
        else if (shield)
        {
            Debug.Log("Shield: " + shieldAmount);
            // Aplicar escudo al jugador
        }

        Destroy(gameObject);
    }
}
