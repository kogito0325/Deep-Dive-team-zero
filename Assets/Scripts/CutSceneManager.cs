using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    public GameObject[] cutScenes;
    public int idx;
    public int maxIdx;

    private void Start()
    {
        idx = 0;
        maxIdx = 20;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0
            && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (idx == 20)
            {
                FindAnyObjectByType<SoundManager>().SetAudioClipToBGM(1);
                SceneManager.LoadScene("MainScene");
            }
            else {cutScenes[idx++].SetActive(true);}
        }
    }
}
