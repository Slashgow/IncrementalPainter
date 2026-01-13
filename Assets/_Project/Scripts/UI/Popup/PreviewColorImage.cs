using UnityEngine;
using UnityEngine.UI;

public class PreviewColorImage : MonoBehaviour
{
    [SerializeField] private Image image;
    public void Initialize(Color color) => image.color = color;
}
