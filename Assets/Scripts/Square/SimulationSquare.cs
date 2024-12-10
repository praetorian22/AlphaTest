using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSquare : MonoBehaviour
{
    private Coroutine moveCoro;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Vector2 speedDefault;
    public Vector2 directiron;
    public Vector2 speed;
    public Vector2 SpeedDef => speedDefault;

    public void StartMoveSquare()
    {
        if (moveCoro != null) StopCoroutine(moveCoro);
        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        while (true)
        {
            _rigidbody.velocity = speed * directiron;
            yield return null;
        }
    }
}
