using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    private bool isPlaying = false;
    public void Play()
    {
        if (isPlaying) return;
        isPlaying = true;
        BlackScreen.instance.done += LoadLevel;
        BlackScreen.instance.FadeToBlack();
    }

    private void LoadLevel()
    {
        BlackScreen.instance.done -= LoadLevel;
        SceneManager.LoadScene(1);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }
    }
}
