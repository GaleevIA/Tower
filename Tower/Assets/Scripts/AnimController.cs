using DG.Tweening;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnimController
{
    private const float _animTime = 0.5f;
    private const float _xOffset = 0.3f;
    private const float _yOffset = 1f;

    AnimController(GameController gameController, MessageController messageController) 
    {
        gameController.OnMoveToTrashhold += MoveToTrashholdAnim;
        gameController.OnMoveBack += MoveBackAnim;
        gameController.OnMoveToTower += MoveToTowerAnim;
        messageController.OnShowMessage += ShowMessageAnim;
    }

    private void MoveToTrashholdAnim(Figure figure, Vector3 trashholdPosition, TweenCallback endAction)
    {
        figure.transform.DOScale(Vector3.zero, _animTime);
        figure.transform.DOMove(trashholdPosition, _animTime);
        figure.transform.DOShakeRotation(_animTime).OnComplete(endAction);
    }

    private void MoveBackAnim(Figure figure, bool isInTower, Vector3 startPosition, TweenCallback endAction)
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

    private void MoveToTowerAnim(Figure figure, Figure topFigure, TweenCallback endAction)
    {
        var endPosition = new Vector3(topFigure.transform.position.x + UnityEngine.Random.Range(-_xOffset, _xOffset), topFigure.transform.position.y + _yOffset);

        var sequence = DOTween.Sequence();

        sequence.Append(figure.transform.DOMoveY(topFigure.transform.position.y + 5, _animTime));
        sequence.Append(figure.transform.DOMove(endPosition, _animTime));
        sequence.Play().OnComplete(endAction);
    }

    private void ShowMessageAnim(TextMeshProUGUI textField)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(textField.DOFade(1, _animTime));
        sequence.AppendInterval(0.5f);
        sequence.Append(textField.DOFade(0, _animTime));

        sequence.Play();
    }
}
