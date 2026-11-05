using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Button playBtn;
    public Button settingsBtn;
    public Button exitBtn;

    private void Start()
    {
        playBtn.onClick.AddListener(StartGame);
        settingsBtn.onClick.AddListener(OpenSettings);
        exitBtn.onClick.AddListener(ExitGame);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}