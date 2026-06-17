using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Wajib untuk mengakses UI Image

public class SpeedometerAnalogUI : MonoBehaviour
{
    [Header("Setup Komponen")]
    [SerializeField] private Rigidbody2D _motorRb;      // Tarik bodi motor asli ke sini
    [SerializeField] private Image _jarumNeedle;        // Tarik objek Image Jarum ke sini
    [SerializeField] private Drive _playerDrive;        // Tarik skrip Drive player ke sini untuk ambil data Max Speed

    [Header("Pengaturan Sudut Jarum (Derajat)")]
    // Angka-angka ini harus disesuaikan dengan desain piringan gambar image_10.png
    // Di piringan itu, angka '0' ada di kiri bawah, dan '140' di kanan bawah.
    [SerializeField] private float _sudutRotasiSaatNol = 135f;   // Posisi jarum menunjuk angka '0' di gambar (Z rotasi positif)
    [SerializeField] private float _sudutRotasiSaatMax = -135f;  // Posisi jarum menunjuk angka '140' di gambar (Z rotasi negatif)

    void Update()
    {
        if (_motorRb == null || _jarumNeedle == null || _playerDrive == null) return;

        // 1. Ambil kecepatan absolut laju motor saat ini (X)
        float kecepatanRiil = Mathf.Abs(_motorRb.linearVelocity.x);

        // 2. Ambil batas kecepatan maksimal dari skrip Drive player
        // Kita butuh variabel _maxSpeed di skrip Drive dibuat 'public' atau pakai Getter.
        // Jika belum public, silakan buka skrip Drive.cs dan ubah: 
        // [SerializeField] private float _maxSpeed = 25f;  -->  public float _maxSpeed = 25f;
        float topSpeed = _playerDrive._maxSpeed; 

        // 3. Hitung persentase kecepatan saat ini dibanding Top Speed (rentang 0.0 sampai 1.0)
        float persentaseKecepatan = Mathf.Clamp01(kecepatanRiil / topSpeed);

        // 4. Konversi persentase menjadi sudut rotasi Z (menggunakan Lerp/Linear Interpolation)
        // Lerp akan menghitung angka di antara _sudutRotasiSaatNol dan _sudutRotasiSaatMax berdasarkan persentase
        float sudutZTarget = Mathf.Lerp(_sudutRotasiSaatNol, _sudutRotasiSaatMax, persentaseKecepatan);

        // 5. Terapkan rotasi secara mulus ke Jarum
        // Quaternion.Euler mengubah sudut derajat (float) menjadi data Rotasi 3D (Quaternion)
        _jarumNeedle.transform.rotation = Quaternion.Euler(0, 0, sudutZTarget);
    }
}