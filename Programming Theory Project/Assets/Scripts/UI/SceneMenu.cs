using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneMenu : MonoBehaviour
{    
    public void LinkBackButton()
    {
        GameManager.BackToMain();       
    }

    public void LinkStartGame()  
    {
        GameManager.Instance.StartGame();
    }

    public void LinkQuitGame()
    {
        GameManager.Instance.QuitGame();
    }
   
}
