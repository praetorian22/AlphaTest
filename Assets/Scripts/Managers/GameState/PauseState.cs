using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : IGameState
{
    public void Enter(GameManager gameManager)
    {
        gameManager.sceneManager.ChangeScreen(scenes.pause);
        gameManager.controllerSquare.StopAllActiveSquare();
    }

    public void Exit(GameManager gameManager)
    {
        
    }
}
