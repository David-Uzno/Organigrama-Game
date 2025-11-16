using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    public string sceneToLoad; // Nombre de la escena (debe estar en Build Settings)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && sceneToLoad != "")
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}