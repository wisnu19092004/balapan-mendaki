using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private bool _sudahAdaPemenang = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_sudahAdaPemenang) return;

        // Cek apakah yang menyentuh garis finish adalah Player
        if (other.CompareTag("Player"))
        {
            _sudahAdaPemenang = true;
            Debug.Log("PLAYER MENANG!");
            FindObjectOfType<GameManagerVisual>().TampilkanLayarSelesai("PLAYER MENANG!");
        }
        // Cek apakah yang menyentuh garis finish adalah NPC
        else if (other.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            _sudahAdaPemenang = true;
            Debug.Log("NPC GHOST MENANG!");
            FindObjectOfType<GameManagerVisual>().TampilkanLayarSelesai("NPC GHOST MENANG!");
        }
    }
}