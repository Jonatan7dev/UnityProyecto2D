using UnityEngine;
using UnityEngine.UI;

public class EnemyState : MonoBehaviour
{
    [Header("Vida")]
    public int hp = 30;

    [Header("UI")]
    public Text texto;

    public bool EstaMuerto { get; private set; }

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

        if (hp <= 0) Morir();
    }

    public void MostrarTexto()
    {
        if (texto != null) texto.text = "Hp: " + hp;
    }

    private void Morir()
    {
        if (EstaMuerto) return;

        EstaMuerto = true;
        Destroy(gameObject);
    }
}
