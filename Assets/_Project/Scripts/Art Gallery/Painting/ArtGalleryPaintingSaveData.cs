using System;
using UnityEngine;

[Serializable]
public class ArtGalleryPaintingSaveData
{
    public string author;
    public string title;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public ArtGalleryPaintingSaveData() { }

    public ArtGalleryPaintingSaveData(Transform t, string author, string title)
    {
        position = t.position;
        rotation = t.rotation;
        scale = t.localScale;
        this.author = author;
        this.title = title;
    }

    public void ApplyTo(Transform t)
    {
        t.position = position;
        t.rotation = rotation;
        t.localScale = scale;
    }
}