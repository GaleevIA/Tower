using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
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
    private IMessageController _messageController;
    private IAnimController _animController;
    private bool _dragIsOn;
    private bool _towerIsFull;

    [Inject]
    public void Initialize(IGameConfig gameConfig, IMessageController messageController, IAnimController animController)
    {
        _gameConfig = gameConfig;
        _messageController = messageController;
        _animController = animController;

        GenerateFigures();
        MakeSubscribes();
        LoadProgress();
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

        _dragIsOn = true;
        _currentFigure = figure;
        _startPosition = _currentFigure.transform.position;
    }

    private void OnDrag(Vector3 point)
    {
        if (!_dragIsOn 
            && _currentFigure is null)
            return;

        var a = Camera.main.ScreenToWorldPoint(point);

        _currentFigure.transform.position = new Vector3(a.x, a.y);
    }

    private void OnDragEnd()
    {
        _dragIsOn = false;

        _scrollRect.enabled = true;

        if (_currentFigure == null)
            return;

        var result = new List<Collider2D>();

        if (Physics2D.OverlapCollider(_currentFigure.GetComponent<BoxCollider2D>(), new ContactFilter2D(), result) != 0)
        {
            var resultTag = result[0].tag;

            if (CheckConditionsBeforeMoveToTrashhold(resultTag))
            {
                var indexOfFigure = _figuresInTower.IndexOf(_currentFigure);

                _animController.MoveToTrashholdAnim(_currentFigure, _trashhold.transform.position, null);
                _animController.MoveFiguresDown(_figuresInTower, indexOfFigure + 1, DestroyFigure);

                _messageController.ShowMessage("MoveToTrashhold");
            }
            else if (CheckConditionsBeforeMoveToTowerBase(resultTag))
            {
                PlaceFigure(_currentFigure);

                _messageController.ShowMessage("FigurePlaced");
            }
            else if (CheckConditionsBeforeMoveToTower(resultTag, result[0].GetComponent<Figure>()))
            {
                var lastFigure = _figuresInTower.Last();

                _animController.MoveToTowerAnim(_currentFigure, lastFigure, () => PlaceFigure(_currentFigure)) ;

                _messageController.ShowMessage("FigurePlacedToTower");
            }
            else
            {
                var isInTower = _figuresInTower.Contains(_currentFigure);

                _animController.MoveBackAnim(_currentFigure, isInTower, _startPosition, isInTower ? TurnBackFigure : DestroyFigure);

                _messageController.ShowMessage("MoveBack");
            }
        }
        else
        {
            var isInTower = _figuresInTower.Contains(_currentFigure);

            _animController.MoveBackAnim(_currentFigure, isInTower, _startPosition, isInTower ? TurnBackFigure : DestroyFigure);

            _messageController.ShowMessage("MoveBack");
        }    
    }

    private void PlaceFigure(Figure figure)
    {
        _figuresInTower.Add(figure);

        var placedFigure = figure;

        placedFigure.OnMouseDragAsObservable()
                .Subscribe(_ => OnDragStartFromTower(placedFigure))
                .AddTo(this);

        _currentFigure = null;

        _towerIsFull = CheckDisplayBounds();
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

        _towerIsFull = CheckDisplayBounds();
    }

    private bool CheckDisplayBounds()
    {
        var figure = _figuresInTower.Last();
        var figurePosition = figure.transform.position;
        var figureBounds = figure.BoxCollider.bounds;

        Vector3 cameraToObject = figurePosition - Camera.main.transform.position;

        float distance = -Vector3.Project(cameraToObject, Camera.main.transform.forward).z;

        Vector3 leftBot = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightTop = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distance));

        if (figureBounds.min.x < leftBot.x
            || figureBounds.max.x > rightTop.x
            || figureBounds.min.y < leftBot.y
            || figureBounds.max.y > rightTop.y)
            return true;

        return false;
    }

    private bool CheckConditionsBeforeMoveToTrashhold(string resultTag)
    {
        return resultTag == _trashholdTag
                && _figuresInTower.Contains(_currentFigure);
    }

    private bool CheckConditionsBeforeMoveToTowerBase(string resultTag)
    {
        return resultTag == _towerZoneTag
                && _figuresInTower.IsEmpty();
    }

    private bool CheckConditionsBeforeMoveToTower(string resultTag, Figure otherFigure)
    {
        return resultTag == _figureTag
                && _figuresInTower.Contains(otherFigure)
                && !_towerIsFull;
    }

    private void SaveProgress()
    {
        var saveArray = new string[_figuresInTower.Count];

        for (int i = 0; i < _figuresInTower.Count; i++)
        {
            var saveInfo = new FigureSaveInfo(_figuresInTower[i]);
            saveArray[i] = JsonUtility.ToJson(saveInfo);
        }

        var saveString = String.Join(";", saveArray);

        PlayerPrefs.SetString("FiguresInTower", saveString);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        var loadString = PlayerPrefs.GetString("FiguresInTower");

        if (loadString.IsEmpty())
            return;

        var figuresPrefabs = _gameConfig.GetConfig().Select(e => e.prefab).ToList();
        var loadArray = loadString.Split(";");

        for (int i = 0; i < loadArray.Length; i++)
        {
            var saveInfo = JsonUtility.FromJson<FigureSaveInfo>(loadArray[i]);

            var prefabToInstantiate = figuresPrefabs.FirstOrDefault(e => e.FigureType == saveInfo.figureType);

            if (prefabToInstantiate is null)
                prefabToInstantiate = figuresPrefabs.First();

            var figureGO = Instantiate(prefabToInstantiate, saveInfo.position, Quaternion.identity, _gameField.transform);
            figureGO.SetColor(saveInfo.color);

            PlaceFigure(figureGO);
        }
    }

    private void OnDestroy()
    {
        foreach (var item in _spawnedFigures)
            Destroy(item);

        _spawnedFigures = null;
    }

    private void OnApplicationQuit()
    {
        SaveProgress();
    }
}