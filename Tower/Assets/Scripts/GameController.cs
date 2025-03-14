using UniRx;
using UnityEngine;

public class GameController
{
    public GameController(GameConfig gameConfig, UIController uiController)
    {
        uiController.UpdateUI(gameConfig);
    }
}