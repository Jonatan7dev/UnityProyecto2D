
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movimiento : MonoBehaviour
{
    [Header("Vida")]
    public int hp = 100;
    public int DmgPincho = 1;
    public int DmgLava = 1;

    [Header("Movimiento")]
    public float speed = 5f;

    [Header("Disparo")]
    public GameObject balaPrefab;
    public float distanciaDisparo = 1f;

    private float siguienteDanoLava;
    private bool muerto;

    private void Update()
    {
        if (muerto) return;

        Mover();
        Disparar();

        if (hp <= 0)
        {
            Morir();
        }
    }

    private void Mover()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 direccion = new Vector3(x, y, 0).normalized;

        transform.Translate(direccion * speed * Time.deltaTime);
    }

    private void Disparar()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if (balaPrefab == null)
        {
            Debug.LogWarning("No has asignado el prefab de la bala.");
            return;
        }

        Vector3 posicionBala = transform.position + Vector3.right * distanciaDisparo;

        Instantiate(balaPrefab, posicionBala, Quaternion.identity);
    }

    public void Vida(int dano)
    {
        if (muerto) return;

        hp = Mathf.Max(0, hp - dano);
        Debug.Log("Vida restante: " + hp);

        if (hp <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        if (muerto) return;

        muerto = true;
        Debug.Log("El personaje ha muerto.");
        enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pincho"))
        {
            Vida(DmgPincho);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Lava"))
        {
            if (Time.time >= siguienteDanoLava)
            {
                Vida(DmgLava);
                siguienteDanoLava = Time.time + 1f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bandera"))
        {
            SceneManager.LoadScene("Scene_one");
        }
    }
}

