using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
    private void Start() {
        Debug.Log("MainMenuController created");
    }

    public void OnClickClient() {
        Debug.Log("Client chosen");
        SceneManager.LoadScene("Client");
    }

    public void OnClickHost() {
        Debug.Log("Host chosen");
        SceneManager.LoadScene("Host");
    }

    public void OnClickGoBackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}