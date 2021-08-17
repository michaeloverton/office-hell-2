using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject canvas;
    private bool isActive = false;
    public IntroScreen introScreen;

    // Update is called once per frame
    void Update()
    {
        if((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)) && !introScreen.getIsActive()) {
            isActive = !isActive;
            canvas.SetActive(isActive);

            if (isActive) {
                Cursor.lockState = CursorLockMode.None;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
            }
            
        }
    }

    public bool getIsActive() {
        return isActive;
    }

    public void disable() {
        isActive = false;
        canvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void leaveWork() {
        SceneManager.LoadScene("Menu");
    }
}
