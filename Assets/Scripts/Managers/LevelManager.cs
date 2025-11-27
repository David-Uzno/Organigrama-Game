using System.Collections.Generic;
using UnityEngine;
public class LevelManager : MonoBehaviour
{
    [Header("Qué boss habilita este spawn")]
    public string bossIDRequired = "Nivel1";   // ejemplo: "Nivel2", "BossFinal"

    [Header("Spawn de esta escena")]
    public Transform playerSpawn;
    public Transform cameraSpawn;

    [Header("Debug")]
    public bool forceSpawn = false;

    private void Start()
    {
        bool defeated = GameManager.Instance.GetBossState(bossIDRequired);

        if (forceSpawn || defeated)
        {
            ApplySpawn();
        }
    }

    private void ApplySpawn()
    {
        if (playerSpawn == null || cameraSpawn == null)
        {
            Debug.LogError("LevelManager: No asignaste playerSpawn o cameraSpawn.");
            return;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("LevelManager: No se encontró Player en la escena.");
            return;
        }

        // Mover jugador
        player.transform.position = playerSpawn.position;

        // Mover cámara
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(
                cameraSpawn.position.x,
                cameraSpawn.position.y,
                Camera.main.transform.position.z
            );
        }
    }
}
