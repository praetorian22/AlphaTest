using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverState : IGameState
{
    public void Enter(GameManager gameManager)
    {
        gameManager.sceneManager.ChangeScreen(scenes.end);        
    }

    public void Exit(GameManager gameManager)
    {
        
    }
}
