using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Pengaturan Panel UI")]
    [SerializeField] private GameObject _panelPengaturanUI; // Tarik panel pengaturanmu ke sini jika ada

    private const string LEVEL_TERAKHIR_KEY = "LevelTerakhirDisimpan";

    private void Start()
    {
        // Pastikan game berjalan dengan waktu normal saat kembali ke main menu
        Time.timeScale = 1f;

        // Tutup panel pengaturan di awal game agar rapi
        if (_panelPengaturanUI != null)
        {
            _panelPengaturanUI.SetActive(false);
        }
    }

    // --- FUNGSI UNTUK TOMBOL: MULAI PETUALANGAN ---
    public void MulaiPetualangan()
    {
        // Membaca level terakhir yang disimpan di memori. 
        // Jika pemain baru pertama kali main, standarnya akan memuat index scene ke-1 (Level 1).
        int indexLevelTujuan = PlayerPrefs.GetInt(LEVEL_TERAKHIR_KEY, 1);

        Debug.Log("Memuat kemajuan petualangan! Membuka Scene Index: " + indexLevelTujuan);
        SceneManager.LoadScene(indexLevelTujuan);
    }

    // --- FUNGSI UNTUK TOMBOL: PENGATURAN ---
    public void BukaPengaturan()
    {
        if (_panelPengaturanUI != null)
        {
            // Jika panel sedang aktif maka ditutup, jika sedang tertutup maka dibuka
            _panelPengaturanUI.SetActive(!_panelPengaturanUI.activeSelf);
        }
        else
        {
            Debug.LogWarning("Panel Pengaturan UI belum ditarik ke dalam kolom Inspector!");
        }
    }

    // --- FUNGSI TAMBAHAN: CARA MENYIMPAN PROGRESS (PENTING) ---
    // Panggil fungsi ini dari skrip FinishLine.cs kamu saat Player menang di level tertentu!
    public static void SimpanProgressLevel(int indexLevelSekarang)
    {
        // Simpan index scene berikutnya agar saat klik Mulai langsung lanjut
        int levelBerikutnya = indexLevelSekarang + 1;
        
        // Hanya simpan jika level berikutnya lebih tinggi dari rekor sebelumnya
        int rekorLama = PlayerPrefs.GetInt(LEVEL_TERAKHIR_KEY, 1);
        if (levelBerikutnya > rekorLama)
        {
            PlayerPrefs.SetInt(LEVEL_TERAKHIR_KEY, levelBerikutnya);
            PlayerPrefs.Save(); // Tulis data secara permanen ke hardisk
            Debug.Log("Progress petualangan disimpan! Level selanjutnya: " + levelBerikutnya);
        }
    }
}