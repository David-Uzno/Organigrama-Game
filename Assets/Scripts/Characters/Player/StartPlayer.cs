using UnityEngine;

public class StartPlayer : MonoBehaviour
{
    [SerializeField] private CharacterData _characterData;

    private void Start()
    {
        if (PlayerAlreadyExists())
        {
            Debug.LogWarning("Ya existe un GameObject con tag 'Player'. No se instanciará otro.");
            Destroy(gameObject);
            return;
        }

        InstantiateSelectedPlayer();
        Destroy(gameObject);
    }

    private bool PlayerAlreadyExists()
    {
        return GameObject.FindGameObjectWithTag("Player") != null;
    }

    private void InstantiateSelectedPlayer()
    {
        int index = PlayerPrefs.GetInt("PlayerIndex");
        GameObject playerPrefab = GetPlayerPrefab(index);
        Instantiate(playerPrefab, transform.position, Quaternion.identity);
    }

    private GameObject GetPlayerPrefab(int index)
    {
        if (_characterData == null || _characterData.Characters == null || _characterData.Characters.Count == 0)
        {
            Debug.LogWarning("CharacterData no está asignado o la lista está vacía. No se puede instanciar el jugador.");
            return null;
        }

        if (index >= 0 && index < _characterData.Characters.Count)
        {
            return _characterData.Characters[index]._player;
        }

        Debug.LogWarning("Índice fuera de rango. Usando prefab por defecto para el jugador.");
        return _characterData.Characters[0]._player;
    }
}
