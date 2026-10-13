using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float timeToFadeIn;
    [SerializeField] private float timeToFadeOut;

    [SerializeField] private bool isFading;

    public void Start()
    {
        text.canvasRenderer.SetAlpha(0);
    }
    public void Fade(int value, float time)
    {
        if(value == 1) { text.canvasRenderer.SetAlpha(0); text.CrossFadeAlpha(1f, time, false); }
        else { text.canvasRenderer.SetAlpha(1); text.CrossFadeAlpha(0f, time, false); }
    }
}
