
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Interfaz")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Game Over")]
    [SerializeField] private float retrasoGameOver = 1f;

    private bool juegoTerminado = false;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER SE HA EJECUTADO");

        if (juegoTerminado)
            return;

        juegoTerminado = true;

        Time.timeScale = 0f;

        StartCoroutine(MostrarGameOver());
    }

    private IEnumerator MostrarGameOver()
    {
        yield return new WaitForSecondsRealtime(retrasoGameOver);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Menu");
    }
}
