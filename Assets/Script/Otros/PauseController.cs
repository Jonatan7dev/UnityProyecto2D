using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseBtn;

    public void PauseBtn()
    {
        Time.timeScale = 0;
        pauseBtn.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void ResumeBtn()
    {
        Time.timeScale = 1;
        pauseBtn.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void RestartBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void GoToMenuBtn()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }

    public void ExitBtn()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif  
    }
}
