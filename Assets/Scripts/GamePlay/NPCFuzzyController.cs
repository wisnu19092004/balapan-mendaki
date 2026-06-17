using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFuzzyController : MonoBehaviour
{
    [Header("Setup Komponen Rigidbody NPC")]
    [SerializeField] private Rigidbody2D _banDepanRB;    // Roda depan bebas (tidak diberi torsi penggerak di tanah)
    [SerializeField] private Rigidbody2D _banBelakangRB; // Satu-satunya roda penggerak utama (RWD)
    [SerializeField] private Rigidbody2D _motorRb;
    [SerializeField] private Transform _playerTransform; // Tarik objek bodi Player asli ke sini di Inspector
    
    [Header("Spesifikasi Performa (Samakan dengan Player)")]
    [SerializeField] private float _speed = 1000f;        // Naikan agar seimbang dengan keganasan Player baru
    [SerializeField] private float _maxSpeed = 35f;       // BARU: Batasan kecepatan maksimal kendaraan NPC
    [SerializeField] private float _rotationSpeed = 400f; // Kecepatan rotasi udara penyeimbang
    [SerializeField] private Vector2 _centerOfMass;

    [Header("Sensor Pemantau Jalan")]
    [SerializeField] private Transform _sensorPosisiDepan;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _jarakCekSensor = 6f;

    private float _moveInputAI; // Pengganti input keyboard virtual untuk AI (-1f hingga 1f)
    private float _seed;

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
        // 1. Ambil data kondisi nyata (Crisp Inputs)
        float sudutKemiringan = AmbilSudutKemiringan();
        float jarakKeTanah = CekKondisiTanahDiDepan();

        // 2. Evaluasi Aturan Fuzzy (Fuzzy Rules Evaluator) untuk menentukan _moveInputAI
        _moveInputAI = ProsesKeputusanFuzzy(sudutKemiringan, jarakKeTanah);
    }

    private void FixedUpdate()
    {
        // Mengambil kecepatan laju horizontal (maju) NPC saat ini
        float currentHorizontalSpeed = _motorRb.linearVelocity.x;

        // --- BARU: SISTEM PEMBATAS KECEPATAN (MAX SPEED) NPC ---
        // Jika AI berniat ngegas maju tetapi kecepatannya sudah menyentuh/melebihi batas, kunci gasnya ke 0
        if (_moveInputAI > 0f && currentHorizontalSpeed >= _maxSpeed)
        {
            _moveInputAI = 0f;
        }

        // Hitung gaya torsi roda berdasarkan keputusan Fuzzy
        float gayaTorsiRoda = -_moveInputAI * _speed * Time.fixedDeltaTime;

        // --- EKSEKUSI FISIKA (ADIL & RELEVAN DENGAN PLAYER) ---
        // PERBAIKAN: Hanya roda belakang (_banBelakangRB) yang diberi torsi penggerak di tanah (RWD)
        _banBelakangRB.AddTorque(gayaTorsiRoda);

        // Torsi rotasi bodi di udara tetap menggunakan kedua roda/bodi agar stabil saat melompat
        _motorRb.AddTorque(_moveInputAI * _rotationSpeed * Time.fixedDeltaTime);
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
            // Jika tertinggal di belakang Player, AI mengamuk menggunakan multiplier 1.3f (130%)
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