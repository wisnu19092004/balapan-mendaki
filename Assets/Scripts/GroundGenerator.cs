using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GroundGenerator : MonoBehaviour
{
    [SerializeField] private SpriteShapeController _spriteShapeController;

    [Header("Pengaturan Dimensi")]
    [SerializeField, Range(3f, 100f)] private int _levelLength = 100; 
    [SerializeField, Range(1f, 50f)] private float _xMultiplier = 2f; 
    [SerializeField, Range(1f, 50f)] private float _yMultiplier = 5f; 
    
    [Header("Pengaturan Bentuk Bukit")]
    [SerializeField, Range(0f, 1f)] private float _curveSmoothness = 0.5f; 
    [SerializeField] private float _noiseStep = 0.2f; 
    [SerializeField] private float _bottom = 15f; 
    
    [SerializeField] private float _seedMataRantai = 20.5f; 

    private Vector3 _lastPos;

    // FUNGSI UTAMA: Hanya berjalan jika kamu menekan tombol "Bikin Tanah" di Inspector
    [ContextMenu("Bikin Tanah Manual")]
    public void GenerateGroundManual()
    {
        if (_spriteShapeController == null)
        {
            Debug.LogError("Sprite Shape Controller belum dimasukkan ke kolom!");
            return;
        }
        
        // Reset total spline lama biar tidak tumpang tindih
        _spriteShapeController.spline.Clear(); 

        for(int i = 0; i < _levelLength; i++)
        {
            float nilaiNoise = Mathf.PerlinNoise(_seedMataRantai, i * _noiseStep);
            _lastPos = transform.position + new Vector3(i * _xMultiplier, nilaiNoise * _yMultiplier);
            
            _spriteShapeController.spline.InsertPointAt(i, _lastPos); 

            if(i != 0 && i != _levelLength - 1)
            {
                _spriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous); 
                _spriteShapeController.spline.SetLeftTangent(i, Vector3.left * _xMultiplier * _curveSmoothness); 
                _spriteShapeController.spline.SetRightTangent(i, Vector3.right * _xMultiplier * _curveSmoothness); 
            }
        }
        
        // Buat penutup kotak bawah tanah
        _spriteShapeController.spline.InsertPointAt(_levelLength, new Vector3(_lastPos.x, transform.position.y - _bottom));
        _spriteShapeController.spline.InsertPointAt(_levelLength + 1, new Vector3(transform.position.x, transform.position.y - _bottom)); 
        
        // Paksa Sprite Shape untuk memperbarui visualnya di Editor saat ini juga
        _spriteShapeController.RefreshSpriteShape();
        
        Debug.Log("Sirkuit bukit berhasil di-generate secara permanen!");
    }
}