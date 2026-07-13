using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFuzzyController : MonoBehaviour
{
    [Header("Setup Komponen Rigidbody NPC")]
    [SerializeField] private Rigidbody2D _banDepanRB;    // Roda depan bebas
    [SerializeField] private Rigidbody2D _banBelakangRB; // Roda penggerak utama (RWD)
    [SerializeField] private Rigidbody2D _motorRb;
    [SerializeField] private Transform _playerTransform; 
    
    [Header("Spesifikasi Performa (Samakan dengan Player)")]
    [SerializeField] private float _speed = 1000f;        
    [SerializeField] private float _maxSpeed = 35f;       
    [SerializeField] private float _rotationSpeed = 400f; 
    [SerializeField] private Vector2 _centerOfMass;

    [Header("Sistem Gesek Rem (Sesuai Player)")]
    [SerializeField] private float _gayaGesekRemMaksimal = 4f;

    [Header("Sensor Pemantau Jalan")]
    [SerializeField] private Transform _sensorPosisiDepan;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _jarakCekSensor = 6f;

    private float _moveInputAI; 
    private float _seed;

    // --- TAMBAHAN UNTUK INTEGRASI FINISH LINE ---
    private bool _sudahFinish = false;

    void Start()
    {
        _motorRb.centerOfMass = _centerOfMass;
        _seed = Random.Range(0f, 100f);

        // --- ATUR TINGKAT TRANSPARANSI ---
        SpriteRenderer[] semuaSprite = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in semuaSprite)
        {
            Color warnaSaatIni = sprite.color;
            warnaSaatIni.a = 0.9f; 
            sprite.color = warnaSaatIni;
        }
    }

    private void Update()
    {
        // Jika sudah finish, hentikan evaluasi keputusan Fuzzy AI
        if (_sudahFinish) return;

        // 1. Ambil data kondisi nyata (Crisp Inputs)
        float sudutKemiringan = AmbilSudutKemiringan();
        float jarakKeTanah = CekKondisiTanahDiDepan();

        // 2. Evaluasi Aturan Fuzzy (Fuzzy Rules Evaluator) untuk menentukan _moveInputAI
        _moveInputAI = ProsesKeputusanFuzzy(sudutKemiringan, jarakKeTanah);
    }

    private void FixedUpdate()
    {
        // JIKA SUDAH FINISH: Eksekusi Rem Otomatis untuk NPC
        if (_sudahFinish)
        {
            AutoBrakeAtFinish();
            return;
        }

        // Mengambil kecepatan laju horizontal (maju) NPC saat ini
        float currentHorizontalSpeed = _motorRb.linearVelocity.x;

        // --- SISTEM PEMBATAS KECEPATAN (MAX SPEED) NPC ---
        if (_moveInputAI > 0f && currentHorizontalSpeed >= _maxSpeed)
        {
            _moveInputAI = 0f;
        }

        // Hitung gaya torsi roda berdasarkan keputusan Fuzzy
        float gayaTorsiRoda = -_moveInputAI * _speed * Time.fixedDeltaTime;

        // --- EKSEKUSI FISIKA ---
        _banBelakangRB.AddTorque(gayaTorsiRoda);

        // Torsi rotasi bodi di udara
        _motorRb.AddTorque(_moveInputAI * _rotationSpeed * Time.fixedDeltaTime);
    }

    // --- FUNGSI KHUSUS UNTUK DIPANGGIL DARI FINISHLINE.CS ---
    public void MatikanKontrolDanRem()
    {
        _sudahFinish = true;
        _moveInputAI = 0f;
        this.enabled = false; // Matikan skrip AI agar tidak memproses keputusan lagi
    }

    private void AutoBrakeAtFinish()
    {
        if (_motorRb != null)
        {
            // Terapkan gaya gesek pengereman bodi pada NPC
            _motorRb.linearDamping = _gayaGesekRemMaksimal;
        }

        // Melambatkan putaran kedua roda NPC secara perlahan
        if (_banBelakangRB != null)
        {
            _banBelakangRB.angularVelocity = Mathf.Lerp(_banBelakangRB.angularVelocity, 0f, Time.fixedDeltaTime * 3f);
        }
        if (_banDepanRB != null)
        {
            _banDepanRB.angularVelocity = Mathf.Lerp(_banDepanRB.angularVelocity, 0f, Time.fixedDeltaTime * 3f);
        }
    }

    // --- SUB-RUTIN LOGIKA FUZZY ---

    private float AmbilSudutKemiringan()
    {
        float sudut = transform.eulerAngles.z;
        if (sudut > 180) sudut -= 360;
        return sudut;
    }

    private float CekKondisiTanahDiDepan()
    {
        RaycastHit2D hit = Physics2D.Raycast(_sensorPosisiDepan.position, Vector2.down, _jarakCekSensor, _groundLayer);
        if (hit.collider != null)
        {
            return hit.distance;
        }
        return _jarakCekSensor;
    }

    private float ProsesKeputusanFuzzy(float sudut, float jarak)
    {
        // --- FUZZIFIKASI AGRESIF ---
        bool posisiAman = sudut >= -18f && sudut <= 18f; 
        bool motorMendongakBahaya = sudut > 35f;   
        bool motorMenunggingBahaya = sudut < -35f; 
        
        bool bukitCuramDekat = jarak < 2.0f;
        bool sedangTerbangTinggi = jarak >= _jarakCekSensor - 0.5f;

        // --- FITUR KARET GELANG (RUBBER BANDING / NITRO AI) ---
        float faktorAgresif = 1.0f;
        if (_playerTransform != null && transform.position.x < _playerTransform.position.x)
        {
            faktorAgresif = 1.3f; 
        }

        // --- RULE BASE AGRESIF & DEFUZZIFIKASI ---

        // Aturan 1: Kritis Mau Terbalik ke Belakang
        if (motorMendongakBahaya)
        {
            return -1f; 
        }

        // Aturan 2: Kritis Mau Terbalik ke Depan
        if (motorMenunggingBahaya)
        {
            return 1f * faktorAgresif; 
        }

        // Aturan 3: Sedang Melayang Terbang di Udara
        if (sedangTerbangTinggi)
        {
            if (sudut > 8f) return -0.3f;  
            if (sudut < -8f) return 0.5f * faktorAgresif; 
            return 0.8f * faktorAgresif;   
        }

        // Aturan 4: Menghadapi Tanjakan Curam
        if (posisiAman && bukitCuramDekat)
        {
            return 0.9f * faktorAgresif; 
        }

        // Aturan 5: Kondisi Jalan Normal / Rata / Landai
        if (posisiAman)
        {
            return 1f * faktorAgresif;
        }

        return 0.7f * faktorAgresif;
    }

    private void OnDrawGizmos()
    {
        if (_sensorPosisiDepan != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_sensorPosisiDepan.position, Vector2.down * _jarakCekSensor);
        }
    }
}