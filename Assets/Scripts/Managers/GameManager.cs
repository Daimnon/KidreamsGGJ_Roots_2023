using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum GameStates { PlayerLoop, VampireLordLoop }

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private delegate void GameState();
    private GameState _gameState;

    [SerializeField] private PlayerData _newPlayerData, _nextPlayerData;
    [SerializeField] private GameObject _playerPrefab, _currentPlayer, _vampireLord;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _playerSpawn, _vampireLordSpawn;
    [SerializeField] private List<Grave> _savedGraves;
    [SerializeField] private bool _debugPlayerLoop;

    public List<Entity> AllEntities { get; set; }
    public List<Grave> SavedGraves { get => _savedGraves; set => value = _savedGraves; }

    public PlayerData NewPlayerData => _newPlayerData;
    public PlayerData NextPlayerData { get => _nextPlayerData; set => value = _nextPlayerData; }

    public GameObject PlayerPrefab => _playerPrefab;
    public GameObject VampireLord => _vampireLord;
    public GameObject CurrentPlayer { get => _currentPlayer; set => value = _currentPlayer; }

    public PlayerController PlayerController { get => _playerController; set => value = _playerController; }

    public Transform PlayerSpawn { get => _playerSpawn; set => value = _playerSpawn; }
    public Transform VampireLordSpawn => _vampireLordSpawn;

    private UnderworldOverlay _underworldOverlay;


    private void Awake()
    {
        _instance = this;
        _gameState = PlayerLoop;
        _underworldOverlay = GetComponent<UnderworldOverlay>();
        _underworldOverlay.SetRegularMode();
        AllEntities = new();
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
    }

    [Button("Test TransitionToUnderworld")]
    public async void TransitionToUnderworld()
    {
        foreach (Entity entity in AllEntities)
        {
            Destroy(entity.gameObject);
        }

        AllEntities.Clear();

        await _underworldOverlay.StartUnderworldAnim();
        Debug.Log("Underworld anim done - Resurrecting player!");

        VampireLord.SetActive(true);
        //ResurrectPlayer();
    }

    public void TransitionToOverworld()
    {
        VampireLord.SetActive(false);

        _underworldOverlay.SetRegularMode();
        Debug.Log("Underworld anim done - Resurrecting player!");

        ResurrectPlayer();
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
                TransitionToUnderworld();
                break;
        }
    }

    private void ResurrectPlayer()
    {
        GameObject newPlayer = Instantiate(_playerPrefab, _playerSpawn);
        PlayerController newPlayerController = newPlayer.GetComponent<PlayerController>();
        newPlayerController.AbsorbedEntity = _nextPlayerData;
        AbsorbedEntity.
        ChangeState(GameStates.PlayerLoop);
    }
}
