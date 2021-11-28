using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
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
        Debug.Log($"NetworkServer.active: {NetworkServer.active}");
        Debug.Log($"NetworkClient.isConnected: {NetworkClient.isConnected}");
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
}
