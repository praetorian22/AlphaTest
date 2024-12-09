using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolSquare
{
    private Dictionary<int, Queue<Square>> _poolSquare = new Dictionary<int, Queue<Square>>();    

    public void CreatePool(List<Square> squares)
    {
        if (!_poolSquare.ContainsKey(squares[0].dataSquare.ID))
        {
            _poolSquare.Add(squares[0].dataSquare.ID, new Queue<Square>());

            for (int i = 0; i < squares.Count; i++)
            {
                _poolSquare[squares[0].dataSquare.ID].Enqueue(squares[i]);
            }
        }
    }
    
    public Square TakeNextSquare(int key)
    {
        if (_poolSquare.ContainsKey(key))
        {
            Square square = _poolSquare[key].Dequeue();
            return square;
        }
        return null;
    }

    public void ReturnToPool(Square square)
    {
        if (_poolSquare.ContainsKey(square.dataSquare.ID))
        {
            _poolSquare[square.dataSquare.ID].Enqueue(square);
        }
    }
}
