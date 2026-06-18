using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private bool _sudahAdaPemenang = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Jika sudah ada yang finish duluan, abaikan siapa pun yang masuk setelahnya
        if (_sudahAdaPemenang) return;

        // Cek apakah yang menyentuh area sensor adalah Player
        if (other.CompareTag("Player"))
        {
            _sudahAdaPemenang = true;
            Debug.Log("PLAYER MENANG!");
            FindObjectOfType<GameManagerVisual>().TampilkanLayarSelesai("PLAYER MENANG!");
        }
        // Cek apakah yang menyentuh area sensor adalah NPC
        else if (other.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            _sudahAdaPemenang = true;
            Debug.Log("NPC MENANG!");
            FindObjectOfType<GameManagerVisual>().TampilkanLayarSelesai("NPC MENANG!");
        }
    }
}