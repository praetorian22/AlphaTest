using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGameState : IGameState
{
    public void Enter(GameManager gameManager)
    {
        gameManager.GameOver();
        gameManager.ChangeState(gameManager.GetState<StartGameState>());
    }

    public void Exit(GameManager gameManager)
    {

    }
}
