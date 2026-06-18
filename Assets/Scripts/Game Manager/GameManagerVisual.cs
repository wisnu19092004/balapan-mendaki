using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagerVisual : MonoBehaviour
{
    [Header("Setup Objek Utama")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private TextMeshProUGUI _jarakTeksUI;

    [Header("Setup UI Popup")]
    [SerializeField] private GameObject _panelSelesaiUI; 
    [SerializeField] private TextMeshProUGUI _teksPemenangUI;

    private float _jarakAwalX;

    void Start()
    {
        if (_playerTransform != null) _jarakAwalX = _playerTransform.position.x;
        if (_panelSelesaiUI != null) _panelSelesaiUI.SetActive(false);
    }

    void Update()
    {
        if (_playerTransform == null) return;

        // Hitung jarak horizontal saat ini untuk teks UI
        float jarakSaatIni = _playerTransform.position.x - _jarakAwalX;
        if (jarakSaatIni < 0) jarakSaatIni = 0;

        if (_jarakTeksUI != null)
        {
            _jarakTeksUI.text = "Jarak: " + jarakSaatIni.ToString("F0") + " m";
        }
    }

    public void TampilkanLayarSelesai(string namaPemenang)
    {
        if (_panelSelesaiUI != null)
        {
            _panelSelesaiUI.SetActive(true);
            _teksPemenangUI.text = namaPemenang;
            Time.timeScale = 0f; // Hentikan game saat ada yang finish
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}