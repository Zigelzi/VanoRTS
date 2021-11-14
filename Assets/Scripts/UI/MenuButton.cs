using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{

    public void PlayGame()
    {
        int startingScene = 1;
        SceneManager.LoadScene(startingScene);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
