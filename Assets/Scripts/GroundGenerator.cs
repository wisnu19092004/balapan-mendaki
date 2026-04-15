using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


[ExecuteInEditMode]
public class GroundGenerator : MonoBehaviour
{
    [SerializeField] private SpriteShapeController _spriteShapeController;

    [SerializeField, Range(3f, 100f)] private int _levelLength = 50; // Length of the ground in units
    [SerializeField, Range(1f, 50f)] private float _xMultiplier = 2f; // Multiplier for the x-axis to stretch the ground
    [SerializeField, Range(1f, 50f)] private float _yMultiplier = 2f; // Multiplier for the y-axis to stretch the ground
    [SerializeField, Range(0f, 1f)] private float _curveSmoothness = 0.5f; // Smoothness of the curve, where 0 is a straight line and 1 is a very smooth curve
    [SerializeField] private float _noiseStep = 0.5f; // Step size for the Perlin noise, which controls the frequency of the terrain features
    [SerializeField] private float _bottom = 10f; // Y-coordinate for the bottom of the ground, which determines how low the terrain can go

    private Vector3 _lasPos;

    public void OnValidate()
    {
        _spriteShapeController.spline.Clear(); // Clear existing points from the spline

        for(int i = 0; i < _levelLength; i++)
        {
            _lastPos = transform.position + new Vector3(i * _xMultiplier, Mathf.PerlinNoise(0, i * _noiseStep) * _yMultiplier);// Calculate the position of the next point using Perlin noise for a natural terrain effect
            spriteShapeController.spline.InsertPointAt(i, _lastPos); // Insert the new point into the spline at the calculated position

            if(i != 0 && 1 != _levelLength - 1)
            {
                _spriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous); // Set the tangent mode to continuous for smooth curves between points
                _spriteShapeController.spline.SetLeftTangen(i, Vector3.left *_xMultiplier * _curveSmoothness); // Set the left tangent to create a smooth curve based on the x multiplier and curve smoothness
                _spriteShapeController.spline.SetRightTangen(i, Vector3.right *_xMultiplier * _curveSmoothness); // Set the right tangent to create a smooth curve based on the x multiplier and curve smoothness
            }
        }
        _spriteShapeController.spline.InsertPointAt()
    }
}
