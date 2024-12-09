using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : IGameState
{
    public void Enter(GameManager gameManager)
    {
        gameManager.sceneManager.ChangeScreen(scenes.menu);
    }

    public void Exit(GameManager gameManager)
    {
        
    }
}
