using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameState
{
    void Enter(GameManager gameManager);
    void Exit(GameManager gameManager);
}
