using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Spawn Sección B")]
    public Transform startPosJugadorB;     // Posición del jugador en la sección B
    public Transform startPosCamaraB;      // Posición de la cámara en la sección B

    [Header("Debug / Testing")]
    public bool forceStartInB = false;

    private void Start()
    {
        bool bossDerrotado = GameManager.Instance.GetBossState("Nivel1");

        // Si está activado el modo test o el boss murió → ir a B
        if (forceStartInB || bossDerrotado)
        {
            SpawnInB();
        }
    }

    private void SpawnInB()
    {

        if (startPosJugadorB == null)
        {
            Debug.LogError("LevelManager: startPosJugadorB no está asignado en el inspector.");
            return;
        }

        if (startPosCamaraB == null)
        {
            Debug.LogError("LevelManager: startPosCamaraB no está asignado en el inspector.");
            return;
        }


        var player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("LevelManager: No se encontró un GameObject con tag 'Player'.");
            return;
        }

        player.transform.position = startPosJugadorB.position;

        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(
                startPosCamaraB.position.x,
                startPosCamaraB.position.y,
                Camera.main.transform.position.z  // mantenemos su Z original
            );
        }
    }
}

