using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSquare
{
    private PoolSquare _poolSquare = new PoolSquare();    
    private List<Square> _activeSquare = new List<Square>();
    private Transform _parentForPool;
    
   

    public void Init(List<Square> squareObjects, Transform parent)
    {        
        _poolSquare.CreatePool(squareObjects);
        _parentForPool = parent;
    }

    public void ReturnToPool(Square square)
    {
        square.damageEvent -= ReturnToPool;
        _poolSquare.ReturnToPool(square);
        square.gameObject.SetActive(false);
        square.gameObject.transform.parent = _parentForPool;
        _activeSquare.Remove(square);
    }

    public void ReturnToPoolAllSquare()
    {
        List<Square> squaresActiveCopy = new List<Square>(_activeSquare);
        for(int i = 0; i < squaresActiveCopy.Count; i++)
        {
            ReturnToPool(squaresActiveCopy[i]);
        }
    }

    public Square TakeNextSquareInPool(int key, Vector3 position)
    {
        Square square = _poolSquare.TakeNextSquare(key);
        square.gameObject.transform.position = position;
        _activeSquare.Add(square);
        square.damageEvent += ReturnToPool;
        return square;
    }
    public bool CheckAlpha(string alpha, out Square squareR)
    {
        foreach (Square square in _activeSquare)
        {
            if (square.dataSquare.Alpha == alpha)
            {
                squareR = square;
                return true;
            }
        }
        squareR = null;
        return false;
    }

    public void StopAllActiveSquare()
    {
        foreach (Square square in _activeSquare)
        {
            square.StopMove();
        }
    }

    public void ResumeMoveAllActiveSquare()
    {
        foreach (Square square in _activeSquare)
        {
            square.ResumeMove();
        }
    }
}
