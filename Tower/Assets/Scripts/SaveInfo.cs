using System;
using UnityEngine;

[Serializable]
public class FigureSaveInfo
{
    public Vector3 position;
    public Color color;
    public EFigureType figureType;

    public FigureSaveInfo(Figure figure) 
    {
        position = figure.transform.localPosition;
        color = figure.FigureImage.color;
        figureType = figure.FigureType;
    }
}
