using System;
using UnityEngine;

[Serializable]
public struct FunctionPower : IFunction
{
    [Tooltip("f(x) = a*(b^x)")]
    [SerializeField, Range(-10f, 10f)] private float a;
    public float A => a;
    [SerializeField, Range(0f, 10f)] private float b;
    public float B => b;
    public float Evaluate(float x) => a * Mathf.Pow(b, x);
}
