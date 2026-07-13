using System.Collections;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private bool _npcSudahFinish = false;
    private bool _playerSudahFinish = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. JIKA NPC MENYENTUH GARIS FINISH DULUAN
        if (other.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            if (!_npcSudahFinish)
            {
                _npcSudahFinish = true;
                Debug.Log("NPC berhasil Finish duluan!");

                // Rem NPC secara otomatis
                MonoBehaviour npcDrive = other.GetComponentInParent<MonoBehaviour>();
                if (npcDrive != null) npcDrive.SendMessage("MatikanKontrolDanRem", SendMessageOptions.DontRequireReceiver);
            }
        }
        // 2. JIKA PLAYER MENYENTUH GARIS FINISH
        else if (other.CompareTag("Player"))
        {
            if (!_playerSudahFinish)
            {
                _playerSudahFinish = true;
                Debug.Log("Player berhasil Finish!");

                // Rem Player secara otomatis & matikan input tombolnya
                Drive playerDrive = other.GetComponentInParent<Drive>();
                if (playerDrive != null) playerDrive.MatikanKontrolDanRem();

                // Cek apakah Player Menang atau Kalah
                bool playerMenang = !_npcSudahFinish; // Menang jika NPC BELUM finish duluan

                // Jalankan jeda sedikit agar efek rem terasa alami sebelum Pop-Up muncul
                StartCoroutine(ProsesSelesaiBalapan(playerMenang));
            }
        }
    }

    private IEnumerator ProsesSelesaiBalapan(bool playerMenang)
    {
        // Tunggu 1.5 detik agar pemain melihat kendaraannya mengerem anggun di balik garis finish
        yield return new WaitForSeconds(1.5f);

        if (playerMenang)
        {
            Debug.Log("TAMPILKAN POP-UP MENANG");
            if (LevelFinishManager.Instance != null) LevelFinishManager.Instance.PlayerMenang();
        }
        else
        {
            Debug.Log("TAMPILKAN POP-UP KALAH");
            if (LevelFinishManager.Instance != null) LevelFinishManager.Instance.PlayerKalah();
        }
    }
}