using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using System.Linq;
using ModestTree;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private const string _towerZoneTag = "TowerZone";
    private const string _trashholdTag = "Trashhold";
    private const string _figureTag = "Figure";
    private const float _animTime = 0.5f;
    private const float _xOffset = 0.3f;
    private const float _yOffset = 1f;

    [SerializeField]
    private ScrollRect _scrollRect;
    [SerializeField]
    private GameObject _scrollViewContent;
    [SerializeField]
    private GameObject _gameField;
    [SerializeField]
    private GameObject _trashhold;
    [SerializeField]
    private GameObject _towerZone;

    private List<Figure> _spawnedFigures = new();
    private Figure _currentFigure;
    private Vector3 _startPosition;
    private List<Color> _existedColors = new();
    private List<Figure> _figuresInTower = new();

    public void UpdateUI(GameConfig gameConfig)
    {
        GenerateFigures(gameConfig);
        MakeSubscribes();
    }

    public void MakeSubscribes()
    {     
        foreach(var figure in _spawnedFigures)
        {
            figure.OnMouseDragAsObservable()             
                .Subscribe(_ => OnDragStart(figure))
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

    private void OnDragStart(Figure figure)
    {
        if (_currentFigure != null)
            return;

        _scrollRect.enabled = false;

        _currentFigure = Instantiate(figure, _gameField.transform);
        _startPosition = figure.transform.position;
    }

    private void OnDragStartFromTower(Figure figure)
    {
        if (_currentFigure != null)
            return;

        _currentFigure = figure;
        _startPosition = _currentFigure.transform.position;
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
        _scrollRect.enabled = true;

        if (_currentFigure == null)
            return;

        var result = new List<Collider2D>();

        if (Physics2D.OverlapCollider(_currentFigure.GetComponent<BoxCollider2D>(), new ContactFilter2D(), result) != 0)
        {
            var resultTag = result[0].tag;

            if (resultTag == _trashholdTag
                && _figuresInTower.Contains(_currentFigure))
            {
                MoveToTrashholdAnim(_currentFigure);
            }
            else if (resultTag == _towerZoneTag
                && _figuresInTower.IsEmpty())
            {
                PlaceFigure();
            }
            else if (resultTag == _figureTag
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
        _figuresInTower.Add(_currentFigure);

        var placedFigure = _currentFigure;

        placedFigure.OnMouseDragAsObservable()
                .Subscribe(_ => OnDragStartFromTower(placedFigure))
                .AddTo(this);

        _currentFigure = null;
    }

    private void TurnBackFigure()
    {
        _currentFigure = null;
    }

    private void DestroyFigure()
    {
        if (_currentFigure is null)
            return;

        if (_figuresInTower.Contains(_currentFigure))
            _figuresInTower.Remove(_currentFigure);

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
        figure.transform.DOMove(_trashhold.transform.position, _animTime);
        figure.transform.DOShakeRotation(_animTime).OnComplete(DestroyFigure);
    }

    private void MoveBackAnim(Figure figure)
    {
        if (_figuresInTower.Contains(figure))
        {
            figure.transform.DOMove(_startPosition, _animTime).OnComplete(TurnBackFigure);
        }
        else
        {
            figure.transform.DOScale(Vector3.zero, _animTime);
            figure.transform.DOMove(_startPosition, _animTime).OnComplete(DestroyFigure);
        }       
    }

    private void MoveToTowerAnim(Figure figure, Figure topFigure)
    {
        var endPosition = new Vector3(topFigure.transform.position.x + Random.Range(-_xOffset, _xOffset), topFigure.transform.position.y + _yOffset);

        var sequence = DOTween.Sequence();

        sequence.Append(figure.transform.DOMoveY(topFigure.transform.position.y + 5, _animTime));
        sequence.Append(figure.transform.DOMove(endPosition, _animTime));
        sequence.Play();
    }
}

