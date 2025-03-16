using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimController : IAnimController
{
    private const float _animTime = 0.5f;
    private const float _xOffset = 0.3f;
    private const float _yOffset = 1f;
    private const float _yOffsetOnJump = 3f;
    private const float _interfvalOnMessage = 0.5f;

    public void MoveToTrashholdAnim(Figure figure, Vector3 trashholdPosition, TweenCallback endAction)
    {       
        figure.transform.DOMove(trashholdPosition, _animTime);
        figure.transform.DOScale(Vector3.zero, _animTime);
        figure.transform.DOShakeRotation(_animTime).OnComplete(endAction);
    }

    public void MoveBackAnim(Figure figure, bool isInTower, Vector3 startPosition, TweenCallback endAction)
    {
        if (isInTower)
        {
            figure.transform.DOMove(startPosition, _animTime).OnComplete(endAction);
        }
        else
        {
            figure.transform.DOScale(Vector3.zero, _animTime);
            figure.transform.DOMove(startPosition, _animTime).OnComplete(endAction);
        }
    }

    public void MoveToTowerAnim(Figure figure, Figure topFigure, TweenCallback endAction)
    {
        var endPosition = new Vector3(topFigure.transform.position.x + Random.Range(-_xOffset, _xOffset), topFigure.transform.position.y + _yOffset);

        var sequence = DOTween.Sequence();

        sequence.Append(figure.transform.DOMoveY(topFigure.transform.position.y + _yOffsetOnJump, _animTime));
        sequence.Append(figure.transform.DOMove(endPosition, _animTime));
        sequence.Play().OnComplete(endAction);
    }

    public void MoveFiguresDown(List<Figure> figures, int startIndex, TweenCallback endAction)
    {
        var sequence = DOTween.Sequence();

        for(int i = startIndex; i < figures.Count; i++)
        {
            sequence.Append(figures[i].transform.DOMoveY(figures[i].transform.position.y - _yOffset, _animTime / 2));
        }
        
        sequence.Play().OnComplete(endAction);
    }

    public void ShowMessageAnim(TextMeshProUGUI textField, TweenCallback endAction)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(textField.DOFade(1, _animTime));
        sequence.AppendInterval(_interfvalOnMessage);
        sequence.Append(textField.DOFade(0, _animTime));
        sequence.Play().OnComplete(endAction);
    }
}
