using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drive : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _banDepanRB;// SerializeField allows you to set the value of this private variable in the Unity Editor
    [SerializeField] private Rigidbody2D _banBelakangRB;// SerializeField allows you to set the value of this private variable in the Unity Editor
    [SerializeField] private Rigidbody2D _motorRb; // SerializeField allows you to set the value of this private variable in the Unity Editor
    [SerializeField] private float _speed = 150f; // SerializeField allows you to set the value of this private variable in the Unity Editor
    [SerializeField] private float _rotationSpeed = 360f; // SerializeField allows you to set the value of this private variable in the Unity Editor
    [SerializeField] private Vector2 _centerOfMass;

    private float _moveInput; // This variable will store the input from the player

    private void Start()
    {
        // Mengubah titik berat motor sesuai nilai yang kita atur di Inspector
        _motorRb.centerOfMass = _centerOfMass;
    }

    private void Update()
    {
        _moveInput = 0f;

        // Membaca input langsung dari keyboard menggunakan New Input System
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                _moveInput = 1f; // Bergerak maju
            }
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                _moveInput = -1f; // Bergerak mundur
            }
        }
    }

    private void FixedUpdate()
    {
        _banDepanRB.AddTorque(-_moveInput * _speed * Time.fixedDeltaTime); // Apply torque to the front wheel based on player input
        _banBelakangRB.AddTorque(-_moveInput * _speed * Time.fixedDeltaTime); // Apply torque to the rear wheel based on player input
        _motorRb.AddTorque(_moveInput * _rotationSpeed * Time.fixedDeltaTime); // Apply torque to the motor based on player input
    }
}
