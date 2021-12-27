using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class UI_GameMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;

    void Start()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex == 0)
        {
            mainMenuPanel = transform.Find("MainMenu").gameObject;
        }
    }

    public void HostGame()
    {
        mainMenuPanel.SetActive(false);
        
        NetworkManager.singleton.StartHost();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        int startingScene = 1;
        SceneManager.LoadScene(startingScene);
    }

    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }

        SceneManager.LoadScene(0);
    }
}
