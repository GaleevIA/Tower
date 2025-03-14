using UnityEngine;
using UnityEngine.UI;

public class Figure : MonoBehaviour
{
    private Image _image;
    private bool _isPlaced;

    public bool IsPlaced 
    { 
        get { return _isPlaced; } 
        set { _isPlaced = value; } 
    }
    public Image FigureImage => _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void SetColor(Color color)
    {
        _image.color = color;
    }
}
