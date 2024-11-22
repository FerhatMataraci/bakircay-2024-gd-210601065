using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject GameModeUI;

    public void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            GameModeUI.SetActive(false);
        }
    }
    public void PlayGame()
    {
        GameModeUI.SetActive(true);
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game!"); 
        Application.Quit();     
    }
    
    public void MidTerm(){
        SceneManager.LoadScene("GD");
    }

     public void Final(){
        SceneManager.LoadScene("Level1");
    }
    
}
