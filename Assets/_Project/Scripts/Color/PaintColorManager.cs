using System.Collections.Generic;
using inkolorgames;
using UnityEngine;

public class PaintColorManager : MonoSingleton<PaintColorManager>
{
    [SerializeField] private List<Color> paintColors;

    public Color GetRandomColor() => paintColors[Random.Range(0,paintColors.Count - 1)];
}
