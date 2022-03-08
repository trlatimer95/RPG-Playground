using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableText : MonoBehaviour
{
    private Text text = null;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        text.text = "";
    }

    public void UpdateText(string newText)
    {
        if (text == null) return;
        text.text = newText;
    }
}
