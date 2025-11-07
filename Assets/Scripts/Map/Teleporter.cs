using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform _playerTeleportPosition;
    [SerializeField] private Transform _cameraTeleportPosition;

    // Variables para los GameObjects a activar y desactivar
    [SerializeField] private GameObject[] _objectsToActivate;
    [SerializeField] private GameObject[] _objectsToDeactivate;

    [SerializeField] private float _activationDelay = 0f;
    [SerializeField] private float _deactivationDelay = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Obtiene el transform del jugador y de la cámara
            Transform playerTransform = other.transform;
            Transform cameraTransform = Camera.main.transform;

            // Teletransporta al jugador y la cámara a la posición del teletransportador
            playerTransform.position = _playerTeleportPosition.position;
            cameraTransform.position = new Vector3(_cameraTeleportPosition.position.x, _cameraTeleportPosition.position.y, cameraTransform.position.z);

            // Activa GameObjects
            StartCoroutine(ActivateObjectsAfterDelay());

            // Desactiva GameObjects
            StartCoroutine(DeactivateObjectsAfterDelay());
        }
    }

    private IEnumerator DeactivateObjectsAfterDelay()
    {
        // Espera el tiempo especificado
        yield return new WaitForSeconds(_deactivationDelay);

        // Desactiva GameObjects
        foreach (var obj in _objectsToDeactivate)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    private IEnumerator ActivateObjectsAfterDelay()
    {
        // Espera el tiempo especificado
        yield return new WaitForSeconds(_activationDelay);

        // Desactiva GameObjects
        foreach (var obj in _objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}