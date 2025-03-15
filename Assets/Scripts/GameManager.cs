using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject black;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(black.activeSelf && Time.timeScale == 0)
            {
                black.SetActive(false);
                Time.timeScale = 1;
            }
            else if(Time.timeScale == 1)
            {
                black.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }
}
