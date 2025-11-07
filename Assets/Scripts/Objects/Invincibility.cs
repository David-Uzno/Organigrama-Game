using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invincibility : FatherObject
{
    [SerializeField] private Color _invincibleColor = Color.cyan;
    [SerializeField] private float _invincibilityDuration = 5f;

    public override void Collect(Player player)
    {
        // Activar invencibilidad
        player.SetInvincibility(true, _invincibleColor);

        // Crear GameObject temporal
        GameObject tempObject = new GameObject("TempInvincibility");
        TempInvincibility tempScript = tempObject.AddComponent<TempInvincibility>();
        tempScript.Initialize(player, _invincibilityDuration);
    }
}

public class TempInvincibility : MonoBehaviour
{
    private Player _player;
    private float _duration;

    public void Initialize(Player player, float duration)
    {
        _player = player;
        _duration = duration;

        StartCoroutine(RestorePlayerAfterDelay());
    }

    private IEnumerator RestorePlayerAfterDelay()
    {
        yield return new WaitForSeconds(_duration);

        if (_player != null)
        {
            // Desactivar invencibilidad
            _player.SetInvincibility(false, _player._originalColor);
        }

        // Destruir GameObject temporal
        Destroy(gameObject);
    }
}