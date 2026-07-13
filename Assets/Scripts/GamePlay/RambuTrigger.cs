using UnityEngine;

public class RambuTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            QuizManager.Instance.TampilkanKuis(gameObject);
        }
    }
}