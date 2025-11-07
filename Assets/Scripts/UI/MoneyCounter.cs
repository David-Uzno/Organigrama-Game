using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _moneyCounterText;
    int _moneyCounter = 0;

    public void Increase()
    {
        _moneyCounter++;
        _moneyCounterText.text = $"{_moneyCounter}";
    }
}