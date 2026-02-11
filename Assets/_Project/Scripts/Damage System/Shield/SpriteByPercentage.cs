using System;
using UnityEngine;

[Serializable]
public class SpriteByPercentage
{
    [SerializeField, Range(0f, 1f)] private float minPercentage = 0.75f;
    [SerializeField] private Sprite sprite;

    public float MinPercentage => minPercentage;
    public Sprite Sprite => sprite;
}
