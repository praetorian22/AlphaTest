using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : IGameState
{
    public void Enter(GameManager gameManager)
    {
        gameManager.sceneManager.ChangeScreen(scenes.game);
        gameManager.controllerSquare.ResumeMoveAllActiveSquare();
    }

    public void Exit(GameManager gameManager)
    {
        
    }

}
