using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIHandler : MonoBehaviour
{

    public GameObject player;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public TextMeshProUGUI highScoreText;

   void Update()
   {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(pauseMenu.activeSelf){
                ResumeGame();
            }else{
                PauseGame();
            }
        }
   } 

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        if(player != null) player.GetComponent<ShipMovement>().GameOver();
        Time.timeScale = 1f;
        gameOverMenu.SetActive(true);
        highScoreText.text = "High Score: " + PlayerPrefs.GetFloat("HighScore").ToString("0") + " m";
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        gameOverMenu.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

}
