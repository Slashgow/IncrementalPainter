using System;
using UnityEngine;

[Serializable]
public struct FunctionAffine : IFunction
{
    [SerializeField, Range(-200f, 200f)] private float a;
    public float A => a;

    [SerializeField, Range(-200f, 200f)] private float b;
    public float B => b;

    public float Evaluate(float x) => a * x + b;

}
