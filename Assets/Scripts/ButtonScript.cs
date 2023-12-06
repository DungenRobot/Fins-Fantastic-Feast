using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public string scene_to_switch_to;

    public void click()
    {
        if (scene_to_switch_to == "quit")
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(scene_to_switch_to);
        }
    }

}
