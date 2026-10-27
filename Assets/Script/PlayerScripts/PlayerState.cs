using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    [Header("Vida")]
    public int hp = 100;
    public int DmgPincho = 1;
    public int DmgLava = 1;

    [Header("UI")]
    public Text texto;

    public bool EstaMuerto { get; private set; }

    private float siguienteDanoLava;

    private void Start()
    {
        MostrarTexto();
        if (hp <= 0) Morir();
    }

    private void Update()
    {
        if (!EstaMuerto && hp <= 0) Morir();
    }

    public void Vida(int dano)
    {
        if (EstaMuerto) return;

        hp = Mathf.Max(0, hp - dano);
        Debug.Log("Vida restante: " + hp);
        MostrarTexto();

        if (hp <= 0) Morir();
    }

    private void Morir()
    {
        if (EstaMuerto) return;
        EstaMuerto = true;
        Debug.Log("El personaje ha muerto.");
        // PlayerMovement y PlayerAttack comprueban EstaMuerto antes de actuar.
    }

    private void MostrarTexto()
    {
        if (texto != null) texto.text = "HP: " + hp;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pincho")) Vida(DmgPincho);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Lava") && Time.time >= siguienteDanoLava)
        {
            Vida(DmgLava);
            siguienteDanoLava = Time.time + 1f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!EstaMuerto && other.CompareTag("Bandera"))
            SceneManager.LoadScene("Scene_one");
    }

    // Conservar aquí si algún botón de UI usa este método.
    public void FuncionBoton(int id)
    {
        Debug.Log("Botón por Script y su botón es " + id);
    }
}
