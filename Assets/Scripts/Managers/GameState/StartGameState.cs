using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameState : IGameState
{
    public void Enter(GameManager gameManager)
    {
        gameManager.sceneManager.ChangeScreen(scenes.game);
        gameManager.StartGame();
    }

    public void Exit(GameManager gameManager)
    {
        
    }
}
