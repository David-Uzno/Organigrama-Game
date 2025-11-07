using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables
    [Header("Player Lifes")]
    [SerializeField] private Image[] _heartImages;
    [SerializeField] private Sprite[] _heartStatuses;
    [SerializeField] private int _currentHeartCount;
    [SerializeField] private int _playerLifes = 6;
    private static int _minHeartCount = 3;
    private static int _maxHeartCount = 5;
    private int _maxLifeMultiplier = 2;

    [Header("Pause Options")]
    [SerializeField] private bool _isPauseEnabled;
    [SerializeField] private GameObject _pauseMenu;

    [Header("Virtual Cursor Options")]
    [SerializeField] private bool _isVirtualCursorEnabled;
    [SerializeField] private GameObject _virtualCursorGameObject;
    [SerializeField] private RectTransform _virtualCursorRectTransform;

    [Header("Scene Management")]
    #if UNITY_EDITOR
    [SerializeField] private List<UnityEditor.SceneAsset> scenesToDestroyAssets;
    #endif
    private List<string> scenesToDestroyNames = new List<string>();
    #endregion

    #region Singleton
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ConvertSceneAssetsToNames();
        }
        else
        {
            Debug.Log("¡Más de un GameManager en la escena!");
            Destroy(gameObject);
        }

        CheckSceneDelete();
    }
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        Heart.OnHeartCollected += UpdateHeartsCurrents;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        Heart.OnHeartCollected -= UpdateHeartsCurrents;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Verificar escena actual después de cargar
        CheckSceneDelete();
    }

    private void Start()
    {
        // Asegura que _currentHeartCount y _playerLifes estén dentro de los límites establecidos
        _currentHeartCount = Mathf.Clamp(_currentHeartCount, _minHeartCount, _maxHeartCount);
        _playerLifes = Mathf.Clamp(_playerLifes, 1, _currentHeartCount * 2);

        // Asignación de maxLifeMultiplier al número de elementos de _heartStatuses
        _maxLifeMultiplier = Mathf.Max(0, _heartStatuses.Length - 1);

        // Actualización de corazones en el HUD
        UpdateHeartsCurrents();

        ApplyOptions();
    }
    #endregion

    #region Options
    private void ApplyOptions()
    {
        _pauseMenu.SetActive(_isPauseEnabled);
        _virtualCursorGameObject.SetActive(_isVirtualCursorEnabled);
    }
    #endregion

    #region Cursor
    public RectTransform GetVirtualCursor()
    {
        return _virtualCursorRectTransform;
    }
    #endregion

    #region Scenes
    private void ConvertSceneAssetsToNames()
    {
        scenesToDestroyNames = new List<string>();

        #if UNITY_EDITOR
        foreach (var sceneAsset in scenesToDestroyAssets)
        {
            if (sceneAsset != null)
            {
                scenesToDestroyNames.Add(sceneAsset.name);
            }
        }
        #endif
    }

    public void CheckSceneDelete()
    {
        // Eliminar GameManager si la escena actual está en la lista
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (scenesToDestroyNames.Contains(currentSceneName))
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Player Lifes
    public int GetMaxLife()
    {
        return _currentHeartCount * _maxLifeMultiplier;
    }

    private void UpdateHeartsCurrents()
    {
        int lifePointsRemaining = _playerLifes;

        for (int i = 0; i < _maxHeartCount; i++)
        {
            if (i < _currentHeartCount)
            {
                // Actualizar sprites de corazones según la vida restante
                _heartImages[i].enabled = true;
                _heartImages[i].sprite = GetStatusHearts(lifePointsRemaining);
                lifePointsRemaining -= _maxLifeMultiplier;
            }
            else
            {
                // Desactivar corazones adicionales
                _heartImages[i].enabled = false;
            }
        }
    }

    private Sprite GetStatusHearts(int lifePoints)
    {
        // Obtener el sprite correspondiente al estado actual de los corazones
        int statusIndex = Mathf.Clamp(lifePoints, 0, _heartStatuses.Length - 1);
        return _heartStatuses[statusIndex];
    }

    public void LoseLife()
    {
        _playerLifes = Mathf.Max(0, _playerLifes - 1);
        UpdateHeartsCurrents();
    }

    public void RecoverLife(int amount)
    {
        int maxLife = GetMaxLife();
        _playerLifes = Mathf.Min(_playerLifes + amount, maxLife);
        UpdateHeartsCurrents();
    }
    #endregion
}