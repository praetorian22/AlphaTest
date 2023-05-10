using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSquare : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    public Vector2 speed;
    public Vector2 directiron;

    public void StartMoveSquare()
    {
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
