using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using System.Linq;
using ModestTree;
using UnityEngine.UI;
using Zenject;
using System;

public class GameController : MonoBehaviour
{
    private const string _towerZoneTag = "TowerZone";
    private const string _trashholdTag = "Trashhold";
    private const string _figureTag = "Figure";

    [SerializeField]
    private ScrollRect _scrollRect;
    [SerializeField]
    private GameObject _scrollViewContent;
    [SerializeField]
    private Canvas _gameField;
    [SerializeField]
    private GameObject _trashhold;
    [SerializeField]
    private GameObject _towerZone;

    private List<Figure> _spawnedFigures = new();
    private Figure _currentFigure;
    private Vector3 _startPosition;
    private List<Figure> _figuresInTower = new();
    private IGameConfig _gameConfig;
    private MessageController _messageController;

    public Action<Figure, Vector3, TweenCallback> OnMoveToTrashhold;
    public Action<Figure, bool, Vector3, TweenCallback> OnMoveBack;
    public Action<Figure, Figure, TweenCallback> OnMoveToTower;

    [Inject]
    public void Initialize(IGameConfig gameConfig, MessageController messageController)
    {
        _gameConfig = gameConfig;
        _messageController = messageController;

        GenerateFigures();
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

    private void GenerateFigures()
    {
        var figures = _gameConfig.GetConfig();

        foreach (var item in figures)
        {
            foreach (var color in item.colors)
            {
                var cube = Instantiate(item.prefab, _scrollViewContent.transform);
                cube.SetColor(color);

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
                OnMoveToTrashhold?.Invoke(_currentFigure, _trashhold.transform.position, DestroyFigure);

                _messageController.ShowMessage("MoveToTrashhold");
            }
            else if (resultTag == _towerZoneTag
                && _figuresInTower.IsEmpty())
            {
                PlaceFigure();

                _messageController.ShowMessage("FigurePlaced");
            }
            else if (resultTag == _figureTag
                && _figuresInTower.Contains(result[0].GetComponent<Figure>()))
            {
                var lastFigure = _figuresInTower.Last();

                OnMoveToTower?.Invoke(_currentFigure, lastFigure, PlaceFigure);

                _messageController.ShowMessage("FigurePlacedToTower");
            }
            else
            {
                var isInTower = _figuresInTower.Contains(_currentFigure);

                OnMoveBack?.Invoke(_currentFigure, isInTower, _startPosition, isInTower ? TurnBackFigure : DestroyFigure);

                _messageController.ShowMessage("MoveBack");
            }
        }
        else
        {
            var isInTower = _figuresInTower.Contains(_currentFigure);

            OnMoveBack?.Invoke(_currentFigure, isInTower, _startPosition, isInTower ? TurnBackFigure : DestroyFigure);

            _messageController.ShowMessage("MoveBack");
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

    private bool CheckDisplayBounds()
    {
        Vector3 cameraToObject = _currentFigure.transform.position - Camera.main.transform.position;

        float distance = -Vector3.Project(cameraToObject, Camera.main.transform.forward).z;

        Vector3 leftBot = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightTop = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distance));

        if (_currentFigure.transform.position.x < leftBot.x
            || _currentFigure.transform.position.x > rightTop.x
            || _currentFigure.transform.position.y < leftBot.y
            || _currentFigure.transform.position.y > rightTop.y)
        {
            return true;
        }

        return false;
    }

    private void OnDestroy()
    {
        foreach (var item in _spawnedFigures)
        {
            Destroy(item);
        }

        _spawnedFigures = null;
    }
}

