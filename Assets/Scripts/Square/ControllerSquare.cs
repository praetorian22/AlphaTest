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
        square.destroyEvent -= ReturnToPool;
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
        square.destroyEvent += ReturnToPool;
        return square;
    }
    public void SetPosition(Square square, int maxAlphaCount)
    {
        Vector3 positionRandomX = Vector3.zero;
        if (maxAlphaCount == 1)
        {
            positionRandomX = Camera.main.ViewportToWorldPoint(new Vector2(UnityEngine.Random.Range(0.1f, 0.9f), 1));            
        }
        else
        {
            if (maxAlphaCount == 2)
            {
                positionRandomX = Camera.main.ViewportToWorldPoint(new Vector2(UnityEngine.Random.Range(0.15f, 0.85f), 1));
            }
            else
            {
                if (maxAlphaCount == 3)
                {
                    positionRandomX = Camera.main.ViewportToWorldPoint(new Vector2(UnityEngine.Random.Range(0.2f, 0.8f), 1));
                }
                else
                {
                    if (maxAlphaCount == 4)
                    {
                        positionRandomX = Camera.main.ViewportToWorldPoint(new Vector2(UnityEngine.Random.Range(0.25f, 0.75f), 1));
                    }
                    else
                    {
                        if (maxAlphaCount == 5)
                        {
                            positionRandomX = Camera.main.ViewportToWorldPoint(new Vector2(UnityEngine.Random.Range(0.3f, 0.7f), 1));
                        }
                        else
                        {
                            if (maxAlphaCount == 6)
                            {
                                positionRandomX = Camera.main.ViewportToWorldPoint(new Vector2(UnityEngine.Random.Range(0.35f, 0.65f), 1));
                            }
                            else
                            {
                                if (maxAlphaCount == 7)
                                {
                                    positionRandomX = Camera.main.ViewportToWorldPoint(new Vector2(UnityEngine.Random.Range(0.4f, 0.6f), 1));
                                }
                                else
                                {
                                    if (maxAlphaCount > 7)
                                    {
                                        positionRandomX = Camera.main.ViewportToWorldPoint(new Vector2(UnityEngine.Random.Range(0.45f, 0.55f), 1));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        square.gameObject.transform.position = new Vector3(positionRandomX.x, positionRandomX.y, square.gameObject.transform.position.z);
    }
    public bool CheckAlpha(string alpha, out Square squareR)
    {
        foreach (Square square in _activeSquare)
        {
            if (square.dataSquare.Alpha.IndexOf(alpha) == 0)
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
