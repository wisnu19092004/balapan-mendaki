using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFuzzyController : MonoBehaviour
{
    [Header("Setup Komponen Rigidbody NPC")]
    [SerializeField] private Rigidbody2D _banDepanRB;
    [SerializeField] private Rigidbody2D _banBelakangRB;
    [SerializeField] private Rigidbody2D _motorRb;
    [SerializeField] private Transform _playerTransform; // Tarik objek bodi Player asli ke sini di Inspector
    
    [Header("Spesifikasi Performa (Samakan dengan Player)")]
    [SerializeField] private float _speed = 150f;
    [SerializeField] private float _rotationSpeed = 360f;
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

        // --- ATUR TINGKAT TRANSPARANSI DI SINI ---
        SpriteRenderer[] semuaSprite = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in semuaSprite)
        {
            Color warnaSaatIni = sprite.color;
            warnaSaatIni.a = 0.75f; // UBAH KE 0.75f agar lebih tebal dan jelas kelihatan
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
        // 3. Eksekusi Fisika (Menggunakan logika torsi yang sama persis dengan skrip Drive Player)
        _banDepanRB.AddTorque(-_moveInputAI * _speed * Time.fixedDeltaTime);
        _banBelakangRB.AddTorque(-_moveInputAI * _speed * Time.fixedDeltaTime);
        _motorRb.AddTorque(_moveInputAI * _rotationSpeed * Time.fixedDeltaTime);
    }

    // --- SUB-RUTIN LOGIKA FUZZY ---

    private float AmbilSudutKemiringan()
    {
        // Mendapatkan sudut rotasi Z dalam rentang -180 sampai 180 derajat
        float sudut = transform.eulerAngles.z;
        if (sudut > 180) sudut -= 360;
        return sudut;
    }

    private float CekKondisiTanahDiDepan()
    {
        // Menembakkan sinar sensor ke bawah depan untuk membaca kontur jalanan terdekat
        RaycastHit2D hit = Physics2D.Raycast(_sensorPosisiDepan.position, Vector2.down, _jarakCekSensor, _groundLayer);
        if (hit.collider != null)
        {
            return hit.distance; // Semakin kecil, berarti bukit di depan semakin dekat/curam naik
        }
        return _jarakCekSensor; // Kondisi tidak mendeteksi tanah dekat (sedang terbang/udara)
    }

    private float ProsesKeputusanFuzzy(float sudut, float jarak)
    {
        // --- FUZZIFIKASI AGRESIF (Batas toleransi bahaya diperlebar) ---
        // NPC sekarang lebih berani menahan posisi mendongak/menungging sebelum panik
        bool posisiAman = sudut >= -18f && sudut <= 18f; 
        bool motorMendongakBahaya = sudut > 35f;   // Dulu 22f, sekarang baru ngerem kalau mau terbalik banget
        bool motorMenunggingBahaya = sudut < -35f; // Dulu -22f, sekarang baru gas kalau mau nyungsep banget
        
        bool bukitCuramDekat = jarak < 2.0f;
        bool sedangTerbangTinggi = jarak >= _jarakCekSensor - 0.5f;

        // --- FITUR KARET GELANG (RUBBER BANDING / NITRO AI) ---
        // Jika posisi NPC berada di belakang Player, naikkan kecepatannya secara agresif
        float faktorAgresif = 1.0f;
        if (_playerTransform != null && transform.position.x < _playerTransform.position.x)
        {
            // NPC mendapat suntikan kecepatan ekstra (Nitro virtual 130%) untuk mengejar Player
            faktorAgresif = 1f; 
        }

        // --- RULE BASE AGRESIF & DEFUZZIFIKASI ---

        // Aturan 1: Kritis Mau Terbalik ke Belakang (Hampir Backflip jatuh telentang)
        if (motorMendongakBahaya)
        {
            return -1f; // Rem penuh untuk menyelamatkan diri
        }

        // Aturan 2: Kritis Mau Terbalik ke Depan (Hampir Frontflip jatuh tengkurap)
        if (motorMenunggingBahaya)
        {
            return 1f * faktorAgresif; // Gas penuh untuk menaikkan moncong
        }

        // Aturan 3: Sedang Melayang Terbang di Udara
        // NPC Agresif tidak akan melepas gas di udara! Dia tetap menekan gas tipis agar saat mendarat langsung melesat
        if (sedangTerbangTinggi)
        {
            if (sudut > 8f) return -0.3f;  // Koreksi rem super tipis jika terlalu mendongak
            if (sudut < -8f) return 0.5f * faktorAgresif; // Koreksi gas jika menukik
            return 0.8f * faktorAgresif;   // Tetap gas pol di udara jika posisi motor lurus!
        }

        // Aturan 4: Menghadapi Tanjakan Curam di depan mata
        // NPC Agresif tidak akan takut! Dia tetap menerjang tanjakan dengan gas tinggi (+0.9f)
        if (posisiAman && bukitCuramDekat)
        {
            return 0.9f * faktorAgresif; 
        }

        // Aturan 5: Kondisi Jalan Normal / Rata / Landai
        // GAS TANPA AMPUN!
        if (posisiAman)
        {
            return 1f * faktorAgresif;
        }

        return 0.7f * faktorAgresif;
    }

    private void OnDrawGizmos()
    {
        // Menampilkan visual garis sensor berwarna merah di tab Scene Unity
        if (_sensorPosisiDepan != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_sensorPosisiDepan.position, Vector2.down * _jarakCekSensor);
        }
    }
}