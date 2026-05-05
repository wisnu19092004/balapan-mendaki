using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayJarakTeks : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _jarakTeks;
    [SerializeField] private Transform _playerTrans;

    private Vector2 _startPosition;

    private void Start()
    {
        _startPosition = _playerTrans.position;
    }

    private void Update()
    {
        Vector2 distance = (Vector2)_playerTrans.position - _startPosition;
        distance.y = 0;
        if(distance.x < 0)
        {
            distance.x = 0;
        }
        _jarakTeks.text= distance.x.ToString("F0")+ " m";
    }
}
