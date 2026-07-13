using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelController : MonoBehaviour
{
    public static FuelController instance; // Singleton instance of the FuelController

    [Header("Setup UI & Performa")]
    [SerializeField] private Image _fuelImage; // Reference to the UI Image component that represents the fuel bar
    [SerializeField, Range(0.1f, 5f)] private float _fuelDrainSpeed = 1f; // Speed at which fuel drains
    [SerializeField] private float _maxFuelAmount = 100f; // Maximum fuel amount
    [SerializeField] private Gradient _fuelGradient; // Gradient to change the fuel bar color based on fuel level

    [Header("Setup Referensi Player (Untuk Boost)")]
    [SerializeField] private Drive _playerDrive; // Tarik objek Player yang memiliki skrip Drive ke sini di Inspector

    private float _currentFuelAmount; // Current fuel amount

    private void Awake()
    {
        if(instance == null) // Check if an instance already exists
        {
            instance = this; // If not, set this as the instance
        }
    }

    private void Start()
    {
        _currentFuelAmount = _maxFuelAmount; // Initialize fuel to maximum at the start
        UpdateUI(); // Update the UI to reflect the initial fuel amount

        // Auto-target: Jika lupa ditarik di Inspector, kode akan mencoba mencari skrip Drive Player otomatis
        if (_playerDrive == null)
        {
            _playerDrive = FindObjectOfType<Drive>();
        }
    }

    private void Update()
    {
        _currentFuelAmount -= Time.deltaTime * _fuelDrainSpeed; // Decrease fuel based on time and drain speed
        UpdateUI(); // Update the UI to reflect the current fuel amount

        if(_currentFuelAmount <= 0f)
        {
            GameManager.instance.GameOver();
        }
    }

    private void UpdateUI()
    {
        _fuelImage.fillAmount = (_currentFuelAmount / _maxFuelAmount); // Update the fuel bar fill amount based on the current fuel percentage
        _fuelImage.color = _fuelGradient.Evaluate(_fuelImage.fillAmount); // Update the fuel bar color based on the current fuel percentage using the gradient
    }

    // FUNGSI UTAMA: Dipanggil oleh item jerigen di jalan saat ditabrak Player
    public void FillFuel()
    {
        _currentFuelAmount = _maxFuelAmount; // Refill fuel to maximum
        UpdateUI(); // Update the UI to reflect the refilled fuel amount

        // --- KODE BARU: PICU BOOST PADA MOTOR PLAYER ---
        if (_playerDrive != null)
        {
            _playerDrive.AktifkanBoost();
        }
        else
        {
            Debug.LogWarning("Skrip Drive Player tidak ditemukan di FuelController! Efek Boost gagal aktif.");
        }
    }
}