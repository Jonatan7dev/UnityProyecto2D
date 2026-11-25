
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Chest")]
    [SerializeField] private Animator animator;

    private bool playerNearby;
    private bool opened;

    [Header("Power Ups")]
    [SerializeField] private GameObject[] powerUps;
    [SerializeField] private Transform spawnPoint;

    private void Update()
    {
        if (playerNearby && !opened && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        opened = true;

        // Reproducir la animación de apertura
        animator.SetTrigger("Open");

        // Generar un Power-Up aleatorio
        SpawnPowerUp();
    }

    private void SpawnPowerUp()
    {
        if (powerUps.Length == 0)
        {
            Debug.LogWarning("No hay Power-Ups asignados al cofre.");
            return;
        }

        int randomIndex = Random.Range(0, powerUps.Length);

        Instantiate(
            powerUps[randomIndex],
            spawnPoint.position,
            Quaternion.identity
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}
