using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsMapState : IGameState
{
    public void Enter(GameManager gameManager)
    {
        gameManager.sceneManager.ChangeScreen(scenes.levels);
    }

    public void Exit(GameManager gameManager)
    {
        
    }
}
