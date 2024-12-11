using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLevelState : IGameState
{
    public void Enter(GameManager gameManager)
    {
        gameManager.sceneManager.ChangeScreen(scenes.levelInfo);
    }

    public void Exit(GameManager gameManager)
    {
        
    }
}
