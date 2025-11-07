using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GigantRacoonBoss : MonoBehaviour, IDamageable
{
    #region Variables
    [Header("Life and Scene Management")]
    [SerializeField] private float _life = 25f;
    [SerializeField] private float _delayBeforeLoad = 2f;

    [Header("Damage Response")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _damageDuration = 0.05f;
    [SerializeField] private int _flashCount = 3;

    [Header("Jump")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _jumpHeight = 10f;
    [SerializeField] private float _jumpDuration = 0.5f;

    [Header("Timers")]
    [SerializeField] private float _minJumpDelay = 1f;
    [SerializeField] private float _maxJumpDelay = 3f;
    [SerializeField] private float _minXAlignDelay = 1f;
    [SerializeField] private float _maxXAlignDelay = 3f;
    [SerializeField] private float _delayAfterJumpSound = 0.25f;

    [Header("Others")]
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioJump;

    private bool _canJump = true;
    #endregion

    #region Unity Methods
    private void Start()
    {
        if (_playerTransform == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                _playerTransform = playerObject.transform;
                Debug.LogWarning($"{nameof(_playerTransform)} no estaba referenciado. Se buscó y asignó un GameObject con el tag 'Player'.");
            }
            else
            {
                Debug.LogWarning("No se encontró ningún GameObject con el tag 'Player'.");
            }
        }

        ScheduleAndJump();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.LoseLife();
        }
    }
    #endregion

    #region Jump
    private void ScheduleAndJump()
    {
        float jumpDelay = Random.Range(_minJumpDelay, _maxJumpDelay);

        StartCoroutine(ScheduleAndJumpRoutine(jumpDelay));
    }

    private IEnumerator ScheduleAndJumpRoutine(float jumpDelay)
    {
        yield return new WaitForSeconds(jumpDelay);

        if (_canJump)
        {
            yield return new WaitForSeconds(_delayAfterJumpSound);

            Vector3 startPosition = transform.position;
            Vector3 endPosition = startPosition + Vector3.up * _jumpHeight;

            // Transición Y Suave con Vector3.Lerp
            _audioJump.Play();
            StartCoroutine(MoveSmoothly(startPosition, endPosition, _jumpDuration));

            // Posición X Siguiendo al Jugador
            float alignDelay = Random.Range(_minXAlignDelay, _maxXAlignDelay);
            Invoke(nameof(AlignWithPlayerX), alignDelay);

            // Posición Y Siguiendo al Jugador
            Invoke(nameof(AlignAndDescend), alignDelay + _jumpDuration);
        }
    }

    private IEnumerator MoveSmoothly(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float timeElapsed = 0f;

        _animator.SetBool("Jump", true);

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        _animator.SetBool("Jump", false);
    }

    private void AlignWithPlayerX()
    {
        Vector3 newPosition = transform.position;
        newPosition.x = _playerTransform.position.x;
        transform.position = newPosition;
    }

    private void AlignAndDescend()
    {
        _audioJump.Play();
        StartCoroutine(AlignAndDescendRoutine());
    }

    private IEnumerator AlignAndDescendRoutine()
    {
        yield return new WaitForSeconds(_delayAfterJumpSound);

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition;
        endPosition.y = _playerTransform.position.y;

        // Transición Y Suave con Vector3.Lerp
        StartCoroutine(MoveSmoothly(startPosition, endPosition, _jumpDuration));

        _canJump = true;
        Invoke(nameof(ScheduleAndJump), _jumpDuration);
    }
    #endregion

    #region Damage Handling
    public void TakeDamage(float damage)
    {
        _life -= damage;

        if (_life > 0)
        {
            StartCoroutine(ShowDamageBlink());
        }
        else
        {
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator ShowDamageBlink()
    {
        if (_spriteRenderer != null)
        {
            for (int i = 0; i < _flashCount; i++)
            {
                // Ocultar Sprite
                _spriteRenderer.enabled = false;
                yield return new WaitForSeconds(_damageDuration);

                // Mostrar Sprite
                _spriteRenderer.enabled = true;
                yield return new WaitForSeconds(_damageDuration);
            }
        }
    }

    private IEnumerator HandleDeath()
    {
        // GameObject Temporal
        GameObject tempObject = new GameObject("TempObjectForSceneLoad");
        GigantRacoonBoss tempScript = tempObject.AddComponent<GigantRacoonBoss>();

        // Corrutina de la Instancia Temporal
        tempScript.StartCoroutine(tempScript.LoadSceneAfterDelay());

        // Destruye el GameObject Original
        Destroy(gameObject);

        yield return null;
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(_delayBeforeLoad);
        SceneManager.LoadScene("Winner");

        // Destruye el GameObject Temporal
        Destroy(gameObject);
    }
    #endregion
}