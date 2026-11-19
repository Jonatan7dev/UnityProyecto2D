using UnityEngine;
using UnityEngine.UI;

public class EnemyState : MonoBehaviour
{
    public int hp = 30;
    public Text texto;
    public bool EstaMuerto { get; private set; }
    public EnemyAnimation Animacion { get; private set; }

    private void Awake()
    {
        Animacion = GetComponent<EnemyAnimation>();
        if (!Animacion && TryGetComponent<Animator>(out _))
            Animacion = gameObject.AddComponent<EnemyAnimation>();
    }

    private void Start()
    {
        MostrarTexto();
        if (hp <= 0) Morir();
    }

    public void TakeHp(int dmg)
    {
        if (EstaMuerto || dmg <= 0) return;
        hp = Mathf.Max(0, hp - dmg);
        MostrarTexto();
        if (hp == 0) Morir();
        else Animacion?.Reproducir(EstadoEnemy.HIT);
    }

    public void MostrarTexto()
    {
        if (texto) texto.text = "Hp: " + hp;
    }

    private void Morir()
    {
        if (EstaMuerto) return;
        EstaMuerto = true;
        if (TryGetComponent(out Rigidbody2D cuerpo)) cuerpo.simulated = false;
        Destroy(gameObject, Animacion?.Reproducir(EstadoEnemy.DIE) ?? 0f);
    }
}
