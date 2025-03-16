using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IAnimController
{
    public void MoveToTrashholdAnim(Figure figure, Vector3 trashholdPosition, TweenCallback endAction);
    public void MoveBackAnim(Figure figure, bool isInTower, Vector3 startPosition, TweenCallback endAction);
    public void MoveToTowerAnim(Figure figure, Figure topFigure, TweenCallback endAction);
    public void MoveFiguresDown(List<Figure> figures, int startIndex, TweenCallback endAction);
    public void ShowMessageAnim(TextMeshProUGUI textField, TweenCallback endAction);
}
