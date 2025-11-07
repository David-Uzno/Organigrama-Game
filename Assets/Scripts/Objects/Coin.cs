using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : FatherObject
{
    private MoneyCounter _counterMoney;

    [Header("Audio")]
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private float _volume = 1.0f;

    private void Start()
    {
        GameObject moneyCounterObject = GameObject.Find("MoneyCounter - Text (TMP)");

        if (moneyCounterObject != null)
        {
            // Obtener el Componente MoneyCounter
            _counterMoney = moneyCounterObject.GetComponent<MoneyCounter>();

            if (_counterMoney == null)
            {
                Debug.LogError("No se encontró el componente MoneyCounter en el objeto MoneyCounter - Text (TMP).");
            }
        }
        else
        {
            Debug.LogError("No se encontró el objeto MoneyCounter - Text (TMP) en la escena.");
        }
    }

    public override void Collect(Player player)
    {
        // Crear un GameObject Temporal para reproducir el sonido
        GameObject audioObject = new GameObject("CoinSound");
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = _audioClip;
        audioSource.volume = _volume;
        audioSource.Play();
        Destroy(audioObject, _audioClip.length);

        // Aumentar el Contador
        if (_counterMoney != null)
        {
            _counterMoney.Increase();
        }
        else
        {
            Debug.LogWarning("MoneyCounter no está asignado en Coin.");
        }
    }
}