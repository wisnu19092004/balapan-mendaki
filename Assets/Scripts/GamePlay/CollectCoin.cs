using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 100; // Jumlah koin yang didapat

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Memeriksa apakah yang menabrak memiliki tag "Player" (Bodi Mobil)
        if (other.CompareTag("Player"))
        {
            // Di sini nanti kamu bisa memanggil fungsi tambah skor/koin
            Debug.Log("Koin diambil! + " + coinValue);
            
            // Hancurkan objek koin dari game
            Destroy(gameObject);
        }
    }
}