using UnityEngine;
using UnityEngine.UI;

public class LevelButtonDisplay : MonoBehaviour
{
    [SerializeField] private int _targetLevelIndex = 1; // 1 untuk Level 1, 2 untuk Level 2, dst.
    [SerializeField] private Image[] _bintangImages;   // Tarik 3 gambar bintang di atas tombol ke sini
    [SerializeField] private Sprite _spriteBintangKuning;
    [SerializeField] private Sprite _spriteBintangAbu;

    private void Start()
    {
        int bintangTerdaftar = PlayerPrefs.GetInt("Level_" + _targetLevelIndex + "_Bintang", 0);

        for (int i = 0; i < _bintangImages.Length; i++)
        {
            if (i < bintangTerdaftar)
            {
                _bintangImages[i].sprite = _spriteBintangKuning; // Bintang aktif
            }
            else
            {
                _bintangImages[i].sprite = _spriteBintangAbu;    // Bintang mati/kosong
            }
        }
    }
}