using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GeneralBtnScript : MonoBehaviour
{
    public void StartGame()
    {
        FindAnyObjectByType<SoundManager>().SetAudioClipToBGM(1);
        SceneManager.LoadScene("MainScene");
    }

    public void GoToCutScene()
    {
        SceneManager.LoadScene("CutScene");
    }

    public void GoToTitle()
    {
        Destroy(FindAnyObjectByType<SoundManager>().gameObject);
        SceneManager.LoadScene("TitleScene");
    }

    public void CloseUI(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void OpenUI(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ChangeToWindow()
    {
        Screen.fullScreen = false;
    }

    public void ChangeToFullScreen()
    {
        Screen.fullScreen = true;
    }

    public void SetNowMobileNum(int num)
    {
        FindAnyObjectByType<PlayerController>().nowMobileNum = num;
    }
}
