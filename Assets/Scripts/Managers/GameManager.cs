using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private delegate void GameState();
    private GameState _gameState;

    private void Awake()
    {
        _instance = this;
        _gameState = PlayerLoop;
    }
    private void Update()
    {
        _gameState.Invoke();
    }

    private void PlayerLoop()
    {

    }
    private void VampireLordLoop()
    {

    }
}
