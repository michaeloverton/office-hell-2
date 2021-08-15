using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource playSound;

    public void PlayGame() {
        StartCoroutine (loadScene());
    }

    IEnumerator loadScene() {
        playSound.Play();
        yield return new WaitWhile (()=> playSound.isPlaying);
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
