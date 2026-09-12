using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public string nomeDaCena;

    public void Restart()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Sair()
    {
        Application.Quit();
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(nomeDaCena);
        Time.timeScale = 1f;
    }
}
