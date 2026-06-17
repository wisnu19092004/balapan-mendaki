using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagerVisual : MonoBehaviour
{
    [Header("Setup Objek Utama")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _finishLineTransform; // Tarik objek GarisFinish dari Hierarchy ke sini
    
    [Header("Pengaturan Target Jarak")]
    [SerializeField] private float _targetJarakFinish = 1000f; // Jarak finish dalam meter
    [SerializeField] private TextMeshProUGUI _jarakTeksUI;

    [Header("Setup UI Popup")]
    [SerializeField] private GameObject _panelSelesaiUI; 
    [SerializeField] private TextMeshProUGUI _teksPemenangUI;

    private bool _finishSudahDitempatkan = false;
    private float _jarakAwalX;

    void Start()
    {
        if (_playerTransform != null) _jarakAwalX = _playerTransform.position.x;
        if (_panelSelesaiUI != null) _panelSelesaiUI.SetActive(false);

        // Sembunyikan garis finish terlebih dahulu saat game baru mulai agar tidak kelihatan di area start
        if (_finishLineTransform != null)
        {
            _finishLineTransform.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (_playerTransform == null) return;

        // Hitung jarak horizontal saat ini
        float jarakSaatIni = _playerTransform.position.x - _jarakAwalX;
        if (jarakSaatIni < 0) jarakSaatIni = 0;

        // Tampilkan meteran di UI
        if (_jarakTeksUI != null)
        {
            _jarakTeksUI.text = "Jarak: " + jarakSaatIni.ToString("F0") + " m / " + _targetJarakFinish + " m";
        }

        // Pindahkan garis finish ke posisi target ketika player sudah setengah jalan (misal di 80% perjalanan)
        // Ini agar garis finish sudah berdiri kokoh di aspal sebelum player atau NPC sampai di sana
        if (jarakSaatIni >= _targetJarakFinish * 0.8f && !_finishSudahDitempatkan)
        {
            _finishSudahDitempatkan = true;
            TempatkanGarisFinish();
        }
    }

    void TempatkanGarisFinish()
    {
        if (_finishLineTransform == null) return;

        // Hitung posisi X absolut tempat garis finish harus dipasang
        float xFinish = _jarakAwalX + _targetJarakFinish;

        // PERBAIKAN: Samakan tinggi Y bendera dengan tinggi Y motor player agar pas di aspal
        float yFinish = _playerTransform.position.y; 

        // Pindahkan posisinya dan aktifkan objeknya di game
        _finishLineTransform.position = new Vector3(xFinish, yFinish, 0f);
        _finishLineTransform.gameObject.SetActive(true);
        
        Debug.Log("Garis Finish berhasil dipasang di koordinat X: " + xFinish + " dan Y: " + yFinish);
    }

    public void TampilkanLayarSelesai(string namaPemenang)
    {
        if (_panelSelesaiUI != null)
        {
            _panelSelesaiUI.SetActive(true);
            _teksPemenangUI.text = namaPemenang;
            Time.timeScale = 0f; // Hentikan game
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}