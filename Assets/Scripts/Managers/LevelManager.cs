using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Spawn de Sección B (desbloqueado al matar al boss)")]
    public Transform startPosB;

    private void Start()
    {
        // Si el boss no está derrotado, no hacemos nada
        if (!GameManager.Instance.GetBossState("Nivel1"))
            return;

        // Si está derrotado → spawn en B
        SpawnInB();
    }

    private void SpawnInB()
    {
        if (startPosB == null)
        {
            Debug.LogError("LevelManager: StartPosB no está asignado en el inspector.");
            return;
        }

        // Buscar al jugador
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("LevelManager: No se encontró un GameObject con tag 'Player'.");
            return;
        }

        // Mover jugador
        player.transform.position = startPosB.position;

        // Mover cámara
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(
                startPosB.position.x,
                startPosB.position.y,
                Camera.main.transform.position.z
            );
        }
    }
}
