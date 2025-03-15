using UnityEngine;
using UnityEngine.UI;

public class ReverseUIBtn : MonoBehaviour
{
    Text posTxt;
    int handPos;
    private void Start()
    {
        posTxt = GetComponentInChildren<Text>();
        SetHandPos();
    }
    public void ChangePos()
    {
        PlayerPrefs.SetInt("HandPos", PlayerPrefs.GetInt("HandPos") > 0 ? 0 : 1);
        SetHandPos();
    }

    private void SetHandPos()
    {
        handPos = PlayerPrefs.GetInt("HandPos", 0);
        posTxt.text = handPos > 0 ? "©Л" : "аб";
    }
}
