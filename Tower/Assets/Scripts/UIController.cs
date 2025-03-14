using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using ModestTree;

public class UIController : MonoBehaviour
{
    private const string _towerZoneTag = "TowerZone";
    private const string _trashholdTag = "Trashhold";
    private const string _figureTag = "Figure";

    [SerializeField]
    private GameObject _scrollViewContent;
    [SerializeField]
    private GameObject _gameField;
    [SerializeField]
    private GameObject _trashhold;
    [SerializeField]
    private GameObject _towerZone;
    [SerializeField]
    private LayerMask _layerMask;

    private List<Figure> _spawnedFigures = new();
    private Figure _currentFigure;
    private List<Color> _existedColors = new();
    private List<Figure> _figuresInTower = new();

    public void UpdateUI(GameConfig gameConfig)
    {
        GenerateFigures(gameConfig);
        MakeSubscribes();
    }

    public void MakeSubscribes()
    {     
        foreach(var cube in _spawnedFigures)
        {
            cube.OnMouseDragAsObservable()
                .Subscribe(_ => OnDragStart(cube))
                .AddTo(this);
        }

        Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonUp(0))
                .Subscribe(_ => OnDragEnd())
                .AddTo(this);

        Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButton(0))
                .Select(_ => Input.mousePosition)
                .Subscribe(x => OnDrag(x))
                .AddTo(this);
    }

    private void GenerateFigures(GameConfig gameConfig)
    {
        foreach (var item in gameConfig.figuresForSpawn)
        {
            for (int i = 0; i < item.count; i++)
            {
                var newColor = Color.white;

                while (true)
                {
                    newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

                    if (!_existedColors.Contains(newColor))
                        break;
                }
                _existedColors.Add(newColor);

                var cube = Instantiate(item.prefab, _scrollViewContent.transform);
                cube.SetColor(newColor);

                _spawnedFigures.Add(cube);
            }
        }
    }

    private void OnDragStart(Figure cube)
    {
        if (_currentFigure != null)
            return;

        _currentFigure = Instantiate(cube, _gameField.transform);
    }

    private void OnDrag(Vector3 point)
    {
        if (_currentFigure is null)
            return;

        var a = Camera.main.ScreenToWorldPoint(point);

        _currentFigure.transform.position = new Vector3(a.x, a.y);
    }

    private void OnDragEnd()
    {
        if (_currentFigure == null)
            return;

        var result = new List<Collider2D>();

        if (Physics2D.OverlapCollider(_currentFigure.GetComponent<BoxCollider2D>(), new ContactFilter2D(), result) != 0)
        {
            if (result[0].tag == _trashholdTag)
            {
                MoveToTrashholdAnim(_currentFigure);
            }
            else if (result[0].tag == _towerZoneTag
                && _figuresInTower.IsEmpty())
            {
                PlaceFigure();
            }
            else if (result[0].tag == _figureTag
                && _figuresInTower.Contains(result[0].GetComponent<Figure>()))
            {
                var lastFigure = _figuresInTower.Last();

                PlaceFigure();

                MoveToTowerAnim(_figuresInTower.Last(), lastFigure);              
            }
            else
            {
                MoveBackAnim(_currentFigure);
            }
        }
        else
        {
            MoveBackAnim(_currentFigure);
        }    
    }

    private void PlaceFigure()
    {
        _currentFigure.IsPlaced = true;

        _figuresInTower.Add(_currentFigure);

        _currentFigure = null;
    }

    private void DestroyFigure()
    {
        Destroy(_currentFigure.gameObject);

        _currentFigure = null;
    }

    private void OnDestroy()
    {
        foreach (var item in _spawnedFigures)
        {
            Destroy(item);
        }

        _spawnedFigures = null;
    }

    private void MoveToTrashholdAnim(Figure figure)
    {
        figure.transform.DOShakeRotation(0.5f).OnComplete(DestroyFigure);
    }

    private void MoveBackAnim(Figure figure)
    {
        figure.transform.DOMove(_scrollViewContent.transform.position, 0.5f).OnComplete(DestroyFigure);
    }

    private void MoveToTowerAnim(Figure figure, Figure topFigure)
    {
        var topFigureTransform = topFigure.FigureImage.rectTransform;

        //var width = topFigureTransform.rect.width;
        var height = topFigureTransform.rect.height;

        //var endPosition = new Vector3(topFigure.transform.position.x + Random.Range(-width / 2, width / 2), topFigure.transform.position.y + height / 2);

        var sequence = DOTween.Sequence();

        sequence.Append(figure.transform.DOMoveY(topFigure.transform.position.y + 5, 0.5f));
        sequence.Append(figure.transform.DOMove(topFigure.transform.position, 0.5f));
        sequence.Play();
    }
}

