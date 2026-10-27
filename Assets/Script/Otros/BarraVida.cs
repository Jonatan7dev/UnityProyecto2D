using NUnit.Framework.Internal.Commands;
using UnityEngine;

using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Image ImageVida;
    private PlayerState player;
    private float vidaMax;
    void Start()
    {
        player = GameObject.Find("Knight").GetComponent<PlayerState>();
        vidaMax = player.hp;
    }

    
    void Update()
    {
        ImageVida.fillAmount = player.hp / vidaMax; 
    }
}
