
using UnityEngine;

public class Bala : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 8f;

    private float direccionX = 1f;

    public int dmgBala = 5;

    public void EstablecerDireccion(float x)
    {
        direccionX = x;
    }

    private void Update()
    {
        transform.Translate(
            Vector3.right * direccionX * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<BlocksController>().TakeHp(dmgBala);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
}


