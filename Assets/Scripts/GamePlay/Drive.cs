using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Sistem Rotasi & Udara Baru")]
    [SerializeField] private float _rotationSpeedTanah = 250f; // Kontrol rotasi standar saat ban menempel di tanah
    [SerializeField] private float _rotationSpeedUdara = 600f; // BARU: Lebih besar agar lincah/responsif saat terbang!
    [SerializeField] private LayerMask _groundLayer;           // Layer tanah untuk cek apakah player sedang terbang

    [Header("Sistem Gesek Rem (Drag)")]
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

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                _moveInput = 1f;
            }
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
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

        // Eksekusi Torsi Ban Belakang (RWD)
        _banBelakangRB.AddTorque(gayaTorsiRoda); 

        // --- BARU: LOGIKA ROTASI DINAMIS (TANAH VS UDARA) ---
        // Cek apakah ban belakang sedang menyentuh tanah menggunakan lingkaran sensor kecil (OverlapCircle)
        bool banMenempelTanah = IsGrounded();
        
        // Pilih kecepatan rotasi berdasarkan kondisi motor
        float speedRotasiAktif = banMenempelTanah ? _rotationSpeedTanah : _rotationSpeedUdara;

        // Eksekusi rotasi bodi motor
        _motorRb.AddTorque(_moveInput * speedRotasiAktif * Time.fixedDeltaTime); 
    }

    private bool IsGrounded()
    {
        // Mengecek area di bawah ban belakang dengan radius 0.5 unit
        // Sesuaikan nama _banBelakangRB jika diperlukan
        Collider2D hit = Physics2D.OverlapCircle(_banBelakangRB.position, 0.6f, _groundLayer);
        return hit != null;
    }

    public void AktifkanBoost()
    {
        if (_sedangBoost)
        {
            StopAllCoroutines();
        }
        StartCoroutine(ProcessBoostCoroutine());
    }

    private IEnumerator ProcessBoostCoroutine()
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