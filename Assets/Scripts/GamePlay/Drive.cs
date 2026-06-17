using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drive : MonoBehaviour
{
    [Header("Setup Komponen Rigidbody")]
    [SerializeField] private Rigidbody2D _banDepanRB;    // Roda depan tetap dipasang untuk rotasi di udara dan menggelinding
    [SerializeField] private Rigidbody2D _banBelakangRB; // Menjadi satu-satunya roda penggerak (RWD)
    [SerializeField] private Rigidbody2D _motorRb; 

    [Header("Pengaturan Performa")]
    [SerializeField] private float _speed = 1000f; 
    [SerializeField] private float _brakeSpeed = 400f; // Kekuatan rem saat menekan tombol mundur
    [SerializeField] public float _maxSpeed = 50f;    // Batasan kecepatan maksimal kendaraan
    [SerializeField] private float _rotationSpeed = 500f; 
    [SerializeField] private Vector2 _centerOfMass;

    private float _moveInput; 

    private void Start()
    {
        // Mengubah titik berat motor sesuai nilai yang kita atur di Inspector
        _motorRb.centerOfMass = _centerOfMass;
    }

    private void Update()
    {
        _moveInput = 0f;

        // Membaca input langsung dari keyboard menggunakan New Input System
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                _moveInput = 1f; // Bergerak maju
            }
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                _moveInput = -1f; // Bergerak mundur / Mengerem
            }
        }
    }

    private void FixedUpdate()
    {
        // Mengambil kecepatan laju horizontal motor saat ini
        float currentHorizontalSpeed = _motorRb.linearVelocity.x;

        // --- SISTEM PEMBATAS KECEPATAN (MAX SPEED) ---
        if (_moveInput > 0f && currentHorizontalSpeed >= _maxSpeed)
        {
            _moveInput = 0f; 
        }

        // --- SISTEM PENGEREMAN DAN AKSELERASI ---
        float gayaTorsiRoda = 0f;

        if (_moveInput > 0f)
        {
            // Akselerasi maju normal
            gayaTorsiRoda = -_moveInput * _speed * Time.fixedDeltaTime;
        }
        else if (_moveInput < 0f)
        {
            // Jika motor sedang melaju ke depan lalu menekan S/Mundur, terapkan kekuatan rem kuat
            if (currentHorizontalSpeed > 0.5f)
            {
                gayaTorsiRoda = -_moveInput * _brakeSpeed * Time.fixedDeltaTime;
            }
            else
            {
                // Jika motor sudah berhenti atau mau mundur, gunakan kecepatan mundur normal
                gayaTorsiRoda = -_moveInput * _speed * Time.fixedDeltaTime;
            }
        }

        // --- EKSEKUSI FISIKA TORQUE (PENGGERAK RODA BELAKANG) ---
        // PERBAIKAN: _banDepanRB.AddTorque dihapus agar tidak ikut berputar sendiri saat digas
        _banBelakangRB.AddTorque(gayaTorsiRoda); 

        // Torsi rotasi bodi di udara tetap aktif menggunakan kedua input agar pemain bisa menyeimbangkan motor
        _motorRb.AddTorque(_moveInput * _rotationSpeed * Time.fixedDeltaTime); 
    }
}