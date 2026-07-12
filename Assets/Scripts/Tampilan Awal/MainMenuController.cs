using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Pengaturan Panel UI")]
    [SerializeField] private GameObject _panelPengaturanUI; // Tarik Panel Pengaturan ke sini di Inspector

    private const string LEVEL_TERAKHIR_KEY = "LevelTerakhirDisimpan";

    private void Start()
    {
        // Pastikan waktu game berjalan normal (1f) saat kembali ke Main Menu
        Time.timeScale = 1f;

        // Tutup panel pengaturan di awal game
        if (_panelPengaturanUI != null)
        {
            _panelPengaturanUI.SetActive(false);
        }
    }

    // --- FUNGSI UNTUK TOMBOL: MULAI PETUALANGAN ---
    public void MulaiPetualangan()
    {
        // Membaca level terakhir dari PlayerPrefs (Default: 1 jika baru pertama main)
        int indexLevelTujuan = PlayerPrefs.GetInt(LEVEL_TERAKHIR_KEY, 1);

        Debug.Log("Memuat gameplay! Buka Scene Index: " + indexLevelTujuan);
        
        // Pindah langsung ke scene gameplay
        SceneManager.LoadScene(indexLevelTujuan);
    }

    public void BukaLevelPage()
    {
        // Ganti "LevelPage" dengan nama exact dari Scene halaman level kamu
        SceneManager.LoadScene("Level"); 
    }

    // --- FUNGSI UNTUK TOMBOL: PENGATURAN (TOGGLE BUKA/TUTUP) ---
    public void BukaPengaturan()
    {
        if (_panelPengaturanUI != null)
        {
            // Buka jika tertutup, tutup jika terbuka
            _panelPengaturanUI.SetActive(!_panelPengaturanUI.activeSelf);
        }
        else
        {
            Debug.LogWarning("Panel Pengaturan UI belum ditarik ke dalam kolom Inspector!");
        }
    }

    // --- FUNGSI KHUSUS TOMBOL SILANG (X) PADA PANEL PENGATURAN ---
    public void TutupPengaturan()
    {
        if (_panelPengaturanUI != null)
        {
            _panelPengaturanUI.SetActive(false);
        }
    }

    public void KeluarGame()
    {
        Debug.Log("Game ditutup!");

        // Menutup aplikasi saat game sudah di-build (EXE / APK / Android / Windows)
        Application.Quit();

        // Menghentikan mode Play jika sedang diuji coba langsung di dalam Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // --- FUNGSI MENYIMPAN PROGRESS (Dipanggil saat menang di FinishLine.cs) ---
    public static void SimpanProgressLevel(int indexLevelSekarang)
    {
        int levelBerikutnya = indexLevelSekarang + 1;
        int rekorLama = PlayerPrefs.GetInt(LEVEL_TERAKHIR_KEY, 1);

        if (levelBerikutnya > rekorLama)
        {
            PlayerPrefs.SetInt(LEVEL_TERAKHIR_KEY, levelBerikutnya);
            PlayerPrefs.Save();
            Debug.Log("Progress disimpan! Level selanjutnya: " + levelBerikutnya);
        }
    }
}