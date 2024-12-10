using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public List<GameObject> screens = new List<GameObject>();

    public void ChangeScreen(scenes screenName)
    {
        foreach (GameObject go in screens)
        {
            if (go.GetComponent<Scene>().scene == screenName) go.SetActive(true);
            else go.SetActive(false);
        }        
    }
}

public enum scenes
{
    menu,
    game,
    pause,
    end,
    levels,
    levelInfo,
}