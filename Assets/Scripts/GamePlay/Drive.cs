using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Memastikan pustaka Input System modern aktif

public class Drive : MonoBehaviour
{
    [Header("Setup Komponen Rigidbody")]
    [SerializeField] private Rigidbody2D _banDepanRB;
    [SerializeField] private Rigidbody2D _banBelakangRB;
    [SerializeField] private Rigidbody2D _motorRb; 

    [Header("Pengaturan Performa Standar")]
    [SerializeField] public float _speed = 1000f; 
    [SerializeField] private float _brakeSpeed = 800f; 
    [SerializeField] public float _maxSpeed = 35f;    
    [SerializeField] private Vector2 _centerOfMass;

    [Header("Sistem Gesek Rem Baru (Unity 6 Drag)")]
    [SerializeField] private float _gayaGesekRemMaksimal = 4f; 
    private float _dragNormalBodi; 

    [Header("Pengaturan Fitur Boost Nitro")]
    [SerializeField] private float _multiplierBoostSpeed = 1.8f; 
    [SerializeField] private float _multiplierMaxSpeed = 1.5f;   
    [SerializeField] private float _durasiBoost = 2f;            

    private float _moveInput; 
    private float _currentSpeedAktif;
    private float _currentMaxSpeedAktif;
    private bool _sedangBoost = false;

    private void Start()
    {
        _motorRb.centerOfMass = _centerOfMass;
        
        _currentSpeedAktif = _speed;
        _currentMaxSpeedAktif = _maxSpeed;

        if (_motorRb != null)
        {
            _dragNormalBodi = _motorRb.linearDamping;
        }
    }

    private void Update()
    {
        _moveInput = 0f;

        // PERBAIKAN UNITY 6: Menggunakan format pengecekan Input System yang lebih aman dan modern
        Keyboard keyboardAktif = Keyboard.current;
        if (keyboardAktif != null)
        {
            if (keyboardAktif.wKey.isPressed || keyboardAktif.upArrowKey.isPressed)
            {
                _moveInput = 1f;
            }
            else if (keyboardAktif.sKey.isPressed || keyboardAktif.downArrowKey.isPressed)
            {
                _moveInput = -1f;
            }
        }
    }

    private void FixedUpdate()
    {
        float currentHorizontalSpeed = _motorRb.linearVelocity.x;

        // --- SISTEM PEMBATAS KECEPATAN ---
        if (_moveInput > 0f && currentHorizontalSpeed >= _currentMaxSpeedAktif)
        {
            _moveInput = 0f; 
        }

        float gayaTorsiRoda = 0f;

        // --- LOGIKA GAYA GESEK DAN TORSI REM ---
        if (_moveInput > 0f)
        {
            _motorRb.linearDamping = _dragNormalBodi;
            gayaTorsiRoda = -_moveInput * _currentSpeedAktif * Time.fixedDeltaTime;
        }
        else if (_moveInput < 0f)
        {
            if (currentHorizontalSpeed > 0.5f)
            {
                _motorRb.linearDamping = _gayaGesekRemMaksimal;
                gayaTorsiRoda = -_moveInput * _brakeSpeed * Time.fixedDeltaTime;
            }
            else
            {
                _motorRb.linearDamping = _dragNormalBodi;
                gayaTorsiRoda = -_moveInput * _currentSpeedAktif * Time.fixedDeltaTime;
            }
        }
        else
        {
            _motorRb.linearDamping = _dragNormalBodi;
        }

        // Eksekusi Fisika RWD
        _banBelakangRB.AddTorque(gayaTorsiRoda); 
    }

    public void AktifkanBoost()
    {
        if (_sedangBoost)
        {
            StopAllCoroutines();
        }
        StartCoroutine(ProsesBoostCoroutine());
    }

    private IEnumerator ProsesBoostCoroutine()
    {
        _sedangBoost = true;
        _currentSpeedAktif = _speed * _multiplierBoostSpeed;
        _currentMaxSpeedAktif = _maxSpeed * _multiplierMaxSpeed;
        
        yield return new WaitForSeconds(_durasiBoost);

        _currentSpeedAktif = _speed;
        _currentMaxSpeedAktif = _maxSpeed;
        _sedangBoost = false;
    }
}