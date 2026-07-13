using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelFinishManager : MonoBehaviour
{
    public static LevelFinishManager Instance;

    [Header("--- UI Panel Hasil Balapan ---")]
    [SerializeField] private GameObject _panelWinUI;   // Panel Menang
    [SerializeField] private GameObject _panelLoseUI;  // Panel Kalah / Try Again

    [Header("--- UI Teks Menang ---")]
    [SerializeField] private TextMeshProUGUI _teksKoinDapat;
    [SerializeField] private TextMeshProUGUI _teksTotalKoin;
    [SerializeField] private GameObject[] _iconBintangUI; // Array 3 Icon Bintang

    private int _koinDikumpulkanLevelIni = 0;
    private bool _balapanSelesai = false; // Penanda agar garis finish tidak terpicu 2x

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // Pastikan kedua panel tertutup saat awal balapan
        if (_panelWinUI != null) _panelWinUI.SetActive(false);
        if (_panelLoseUI != null) _panelLoseUI.SetActive(false);
    }

    public void TambahKoin(int jumlah)
    {
        _koinDikumpulkanLevelIni += jumlah;
    }

    // --- DIPANGGIL SAAT PLAYER FINISH PERTAMA ---
    public void PlayerMenang()
    {
        if (_balapanSelesai) return; // Jika NPC sudah finish duluan, abaikan
        _balapanSelesai = true;

        Time.timeScale = 0f; // Hentikan game

        int bintangDidapat = QuizManager.Instance.JumlahBintang;
        if (bintangDidapat == 0) bintangDidapat = 1; // Minimal 1 bintang jika menang

        int totalKoinHasilPerkalian = _koinDikumpulkanLevelIni * bintangDidapat;

        // Simpan Total Koin
        int totalKoinLama = PlayerPrefs.GetInt("TotalKoinPemain", 0);
        int totalKoinBaru = totalKoinLama + totalKoinHasilPerkalian;
        PlayerPrefs.SetInt("TotalKoinPemain", totalKoinBaru);

        // Simpan Bintang Level
        int indexLevel = SceneManager.GetActiveScene().buildIndex;
        string keyBintang = "Level_" + indexLevel + "_Bintang";
        int bintangLama = PlayerPrefs.GetInt(keyBintang, 0);

        if (bintangDidapat > bintangLama)
        {
            PlayerPrefs.SetInt(keyBintang, bintangDidapat);
        }

        // Buka Level Selanjutnya
        MainMenuController.SimpanProgressLevel(indexLevel);
        PlayerPrefs.Save();

        // Tampilkan Panel Menang
        _panelWinUI.SetActive(true);
        _teksKoinDapat.text = "+ " + totalKoinHasilPerkalian + " Koin";
        _teksTotalKoin.text = "Total Koin: " + totalKoinBaru;

        for (int i = 0; i < _iconBintangUI.Length; i++)
        {
            _iconBintangUI[i].SetActive(i < bintangDidapat);
        }
    }

    // --- DIPANGGIL SAAT NPC FINISH PERTAMA ---
    public void PlayerKalah()
    {
        if (_balapanSelesai) return; // Jika Player sudah finish duluan, abaikan
        _balapanSelesai = true;

        Time.timeScale = 0f; // Hentikan game

        Debug.Log("NPC Menang! Player Kalah.");

        // Tampilkan Panel Kalah
        if (_panelLoseUI != null)
        {
            _panelLoseUI.SetActive(true);
        }
    }

    // --- FUNGSI NAVIGASI TOMBOL POP-UP ---
    public void UlangiLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload Scene
    }

    public void KembaliKeMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Home"); // Ganti "Home" sesuai nama scene Main Menu kamu
    }
}