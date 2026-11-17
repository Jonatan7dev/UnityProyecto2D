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

    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager;

    private PlayerAnimation anim;

    public bool EstaMuerto { get; private set; }

    private float siguienteDanoLava;

    private void Start()
    {
        MostrarTexto();
        if (hp <= 0) Morir();
    }

    private void Awake()
    {
        anim = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        if (!EstaMuerto && hp <= 0) Morir();
    }

    public void Vida(int dano)
    {
        if (EstaMuerto || dano <= 0)
            return;

        hp = Mathf.Max(0, hp - dano);

        Debug.Log("Vida restante: " + hp);

        MostrarTexto();

        // Si el daño es mortal, gestionar la muerte.
        if (hp <= 0)
        {
            Morir();
            return;
        }

        // Reproducir la animación al recibir daño.
        anim?.RecibirGolpe();
    }

    private void Morir()
    {
        if (EstaMuerto)
            return;

        EstaMuerto = true;

        // Reproducir la animación de muerte.
        anim?.AnimDead();

        // Avisar al GameManager.
        if (gameManager != null)
            gameManager.GameOver();

        Debug.Log("El personaje ha muerto.");
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
            SceneManager.LoadScene("Level_1");
        else if (!EstaMuerto && other.CompareTag("Bandera2"))
            SceneManager.LoadScene("Menu");
                
    }

    // Conservar aquí si algún botón de UI usa este método.
    public void FuncionBoton(int id)
    {
        Debug.Log("Botón por Script y su botón es " + id);
    }
}
