using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Figure : MonoBehaviour
{
    [SerializeField]
    private EFigureType figureType;

    private Image _image;
    private BoxCollider2D _boxCollider;

    public Image FigureImage => _image;
    public BoxCollider2D BoxCollider => _boxCollider;
    public EFigureType FigureType => figureType;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public void SetColor(Color color)
    {
        _image.color = color;
    }
}
