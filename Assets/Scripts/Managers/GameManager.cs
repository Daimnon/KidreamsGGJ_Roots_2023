using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates { PlayerLoop, VampireLordLoop }

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private delegate void GameState();
    private GameState _gameState;

    [SerializeField] private PlayerData _newPlayerData, _nextPlayerData;
    [SerializeField] private GameObject _playerPrefab, _currentPlayer;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _playerSpawn, _vampireLordSpawn;
    [SerializeField] private bool _debugPlayerLoop;

    public PlayerData NewPlayerData => _newPlayerData;
    public PlayerData NextPlayerData { get => _nextPlayerData; set => value = _nextPlayerData; }

    public GameObject PlayerPrefab => _playerPrefab;
    public GameObject CurrentPlayer { get => _currentPlayer; set => value = _currentPlayer; }

    public PlayerController PlayerController { get => _playerController; set => value = _playerController; }

    public Transform PlayerSpawn { get => _playerSpawn; set => value = _playerSpawn; }
    public Transform VampireLordSpawn => _vampireLordSpawn;


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
        if (_debugPlayerLoop) Debug.Log($"GameState is PlayerLoop");
    }
    private void VampireLordLoop()
    {
        if (_debugPlayerLoop) Debug.Log($"GameState is VampireLordLoop");
        GameObject newPlayer = Instantiate(_playerPrefab, _playerSpawn);
        newPlayer.transform.SetParent(_playerSpawn.parent);
        PlayerController newPlayerController = newPlayer.GetComponent<PlayerController>();
        newPlayerController.Data = _nextPlayerData;
        ChangeState(GameStates.PlayerLoop);

    }

    public void ChangeState(GameStates newState)
    {
        switch (newState)
        {
            case GameStates.PlayerLoop:
                _gameState = PlayerLoop;
                break;
            case GameStates.VampireLordLoop:
                _gameState = VampireLordLoop;
                break;
        }
    }
}
