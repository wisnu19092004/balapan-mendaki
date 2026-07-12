using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownManager : MonoBehaviour
{
    [Header("Pengaturan Gambar UI")]
    [SerializeField] private Image _countdownImageDisplay; 
    [SerializeField] private Sprite _sprite3;              
    [SerializeField] private Sprite _sprite2;              
    [SerializeField] private Sprite _sprite1;              
    [SerializeField] private Sprite _spriteGo;             

    [Header("Pengaturan Audio Visual")]
    [SerializeField] private AudioSource _audioSource;     
    [SerializeField] private AudioClip _soundBeepDetik;    
    [SerializeField] private AudioClip _soundGo;           

    [Header("Daftar Kendaraan (Player & NPC)")]
    // Pakai List agar bisa menampung skrip Drive Player DAN skrip AI/Drive NPC sekaligus!
    [SerializeField] private List<MonoBehaviour> _daftarKendaraan = new List<MonoBehaviour>(); 

    private void Start()
    {
        // 1. Matikan semua kontrol kendaraan (Player & NPC) di awal
        SetSemuaKendaraanAktif(false);

        // 2. Jalankan Coroutine hitung mundur visual & audio
        StartCoroutine(ProsesHitungMundurVisual());
    }

    private IEnumerator ProsesHitungMundurVisual()
    {
        // --- HITUNGAN 3 ---
        TampilkanAset(_sprite3, _soundBeepDetik);
        yield return new WaitForSeconds(1f);

        // --- HITUNGAN 2 ---
        TampilkanAset(_sprite2, _soundBeepDetik);
        yield return new WaitForSeconds(1f);

        // --- HITUNGAN 1 ---
        TampilkanAset(_sprite1, _soundBeepDetik);
        yield return new WaitForSeconds(1f);

        // --- GO! ---
        TampilkanAset(_spriteGo, _soundGo);

        // 3. Aktifkan kembali semua kontrol kendaraan (Player & NPC tancap gas bareng!)
        SetSemuaKendaraanAktif(true);

        // Tunggu 1 detik lalu sembunyikan gambar GO!
        yield return new WaitForSeconds(1f);

        if (_countdownImageDisplay != null)
        {
            _countdownImageDisplay.gameObject.SetActive(false);
        }
    }

    private void TampilkanAset(Sprite gambar, AudioClip suara)
    {
        if (_countdownImageDisplay != null && gambar != null)
        {
            _countdownImageDisplay.sprite = gambar;
            _countdownImageDisplay.SetNativeSize(); 
        }

        if (_audioSource != null && suara != null)
        {
            _audioSource.PlayOneShot(suara);
        }
    }

    // Fungsi pembantu untuk mengaktifkan/mematikan semua skrip di dalam List
    private void SetSemuaKendaraanAktif(bool status)
    {
        foreach (MonoBehaviour skripKendaraan in _daftarKendaraan)
        {
            if (skripKendaraan != null)
            {
                skripKendaraan.enabled = status;
            }
        }
    }
}