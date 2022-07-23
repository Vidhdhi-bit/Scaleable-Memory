using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoText : MonoBehaviour
{
    public static InfoText instance;

    public TextMeshProUGUI infotext;

    private void Awake()
    {
        instance = this;
        infotext.text = "";
        ShowMessage("");
    }

    public void ShowMessage(string _text)
    {
        infotext.text = _text;
        
    }

}
