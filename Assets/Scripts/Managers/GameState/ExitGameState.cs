using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGameState : IGameState
{
    public void Enter(GameManager gameManager)
    {
        gameManager.GameOver();
        gameManager.ChangeState(gameManager.GetState<MainMenuState>());
    }

    public void Exit(GameManager gameManager)
    {
        
    }    
}
