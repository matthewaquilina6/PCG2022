using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Objective : MonoBehaviour
{
    public static int x = 0;
    void Start()
    {
        x = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Car")
        {
            if (x == 1)
            {
                Scene scene = SceneManager.GetActiveScene();
                if (scene.name == "1")
                {
                    SceneManager.LoadScene("2");
                }

                if (scene.name == "2")
                {
                    SceneManager.LoadScene("3");
                }

                if (scene.name == "3")
                {
                    Application.Quit();
                }

            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Car")
        {
            x = 1;
        }
    }
}
