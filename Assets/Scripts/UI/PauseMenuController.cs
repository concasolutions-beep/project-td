using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuRoot;

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToggleMenu()
    {
        if (menuRoot.activeSelf)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        menuRoot.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        menuRoot.SetActive(false);
        Time.timeScale = 1f;
    }
}
