using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Wajib diimport untuk menggunakan TextMeshPro

public class SpeedometerUI : MonoBehaviour
{
    [Header("Setup Komponen")]
    [SerializeField] private Rigidbody2D _motorRb;          // Tarik objek bodi motor Player ke sini
    [SerializeField] private TextMeshProUGUI _speedText;    // Tarik objek teks UI TMP ke sini

    [Header("Pengaturan Tampilan")]
    [SerializeField] private float _pengaliKecepatan = 4f;  // Biar angkanya kelihatan masuk akal (contoh: 25 x 4 = 100 km/jam)
    [SerializeField] private string _satuanKecepatan = " km/h";

    void Update()
    {
        if (_motorRb == null || _speedText == null) return;

        // 1. Ambil kecepatan absolut horizontal (X) dari motor agar saat mundur angkanya tetap positif
        float kecepatanRiil = Mathf.Abs(_motorRb.linearVelocity.x);

        // 2. Kalikan dengan angka pengali agar visualnya pas seperti game balap asli
        float kecepatanTampilan = kecepatanRiil * _pengaliKecepatan;

        // 3. Bulatkan angka desimalnya menjadi angka bulat (tanpa koma)
        int kecepatanBulat = Mathf.RoundToInt(kecepatanTampilan);

        // 4. Masukkan angka tersebut ke dalam teks UI
        _speedText.text = kecepatanBulat.ToString() + _satuanKecepatan;
    }
}