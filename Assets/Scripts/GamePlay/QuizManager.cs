using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance;

    [Header("Pengaturan UI Pop-up Kuis")]
    [SerializeField] private GameObject _panelKuisUI;
    [SerializeField] private TextMeshProUGUI _teksPertanyaan;
    [SerializeField] private Button[] _tombolOpsiJawaban; // Minimal 2-4 tombol jawaban

    [Header("Data Kuis Level Ini")]
    [SerializeField] private string _pertanyaanText = "Apa arti dari rambu turunan curam?";
    [SerializeField] private string[] _pilihanJawaban = { "Turunan Curam", "Tanjakan Curam", "Jalan Licin", "Batas Kecepatan" };
    [SerializeField] private int _indexJawabanBenar = 0; // Pilihan ke-0 adalah "Turunan Curam"

    [Header("Status Skor & Bintang")]
    public int JumlahBintang { get; private set; } = 0;

    private GameObject _rambuAktif;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (_panelKuisUI != null) _panelKuisUI.SetActive(false);
    }

    // Dipanggil otomatis oleh Rambu Lalu Lintas saat ditabrak Player
    public void TampilkanKuis(GameObject rambu)
    {
        _rambuAktif = rambu;
        Time.timeScale = 0f; // JEDA GAME
        _panelKuisUI.SetActive(true);

        _teksPertanyaan.text = _pertanyaanText;

        for (int i = 0; i < _tombolOpsiJawaban.Length; i++)
        {
            if (i < _pilihanJawaban.Length)
            {
                _tombolOpsiJawaban[i].gameObject.SetActive(true);
                int index = i; // Copy lokal untuk listener button
                _tombolOpsiJawaban[i].GetComponentInChildren<TextMeshProUGUI>().text = _pilihanJawaban[i];
                _tombolOpsiJawaban[i].onClick.RemoveAllListeners();
                _tombolOpsiJawaban[i].onClick.AddListener(() => JawabKuis(index));
            }
            else
            {
                _tombolOpsiJawaban[i].gameObject.SetActive(false);
            }
        }
    }

    private void JawabKuis(int indexPilihan)
    {
        if (indexPilihan == _indexJawabanBenar)
        {
            Debug.Log("Jawaban BENAR! +1 Bintang");
            JumlahBintang = Mathf.Min(JumlahBintang + 1, 3); // Maksimal 3 bintang
        }
        else
        {
            Debug.Log("Jawaban SALAH!");
        }

        // Hilangkan rambu yang sudah dilewati agar tidak terpicu 2x
        if (_rambuAktif != null) _rambuAktif.SetActive(false);

        _panelKuisUI.SetActive(false);
        Time.timeScale = 1f; // LANJUTKAN GAME
    }
}