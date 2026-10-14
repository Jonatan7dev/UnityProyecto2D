
using UnityEngine;

public class Bala : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 8f;

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            Destroy(gameObject);
        }
    }
}

