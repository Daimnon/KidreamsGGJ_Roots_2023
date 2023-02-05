using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TextCore.Text;

public enum GameStates { PlayerLoop, VampireLordLoop }

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private delegate void GameState();
    private GameState _gameState;

    public event Action OnResurrectPlayer;

    // win/lose condition: blood empty = perma-death, blood full = vampireLord resurrection, value = 0-100.
    [SerializeField] private int _bloodAmount = 25;
    public int BloodAmount => _bloodAmount;

    [SerializeField] private int _deathCount = 0;
    public int DeathCount { get => _deathCount; set => _deathCount = value; }

    [SerializeField] private GameObject _playerPrefab, _currentVampireLord;
    public GameObject PlayerPrefab => _playerPrefab;
    public GameObject CurrentVampireLord => _currentVampireLord;

    [SerializeField] private PlayerController _playerController;
    public PlayerController PlayerController { get => _playerController; set => _playerController = value; }
    
    [SerializeField] private VampireLordController _vampireLordController;
    public VampireLordController VampireLordController => _vampireLordController;

    [SerializeField] private Transform _playerSpawn, _vampireLordSpawn;
    public Transform PlayerSpawn => _playerSpawn;
    public Transform VampireLordSpawn => _vampireLordSpawn;

    [SerializeField] private List<Villager> _engraved;
    public List<Villager> Engraved { get => _engraved; set => _engraved = value ; }

    [SerializeField] private Villager _chosenEngraved;
    public Villager ChosenEngraved { get => _chosenEngraved; set => _chosenEngraved = value; }

    [SerializeField] private List<Entity> _allEntities;
    public List<Entity> AllEntities { get => _allEntities; set => _allEntities = value; }

    [SerializeField] private UnderworldOverlay _underworldOverlay;
    public UnderworldOverlay UnderworldOverlay => _underworldOverlay;

    [SerializeField] private bool _debugPlayerLoop = false;

    private void Awake()
    {
        _instance = this;
        _engraved = new();
        _allEntities = new();
        _vampireLordController = _currentVampireLord.GetComponent<VampireLordController>();
        _underworldOverlay.SetRegularMode();
        OnResurrectPlayer += TransitionToOverworld;
        _gameState = PlayerLoop;
    }
    private void Update()
    {
        _gameState.Invoke();
    }
    private void OnDisable()
    {
        OnResurrectPlayer -= TransitionToOverworld;
    }

    private void PlayerLoop()
    {
        if (_debugPlayerLoop)
            Debug.Log($"GameState is PlayerLoop");

        if (!CameraManager.Instance.IsFollowingPlayer())
            CameraManager.Instance.ChangeState(CameraStates.FollowPlayer);
    }
    private void VampireLordLoop()
    {
        if (_debugPlayerLoop)
            Debug.Log($"GameState is VampireLordLoop");

        if (CameraManager.Instance.IsFollowingPlayer())
            CameraManager.Instance.ChangeState(CameraStates.FollowVampireLord);
    }

    [Button("Test TransitionToUnderworld")]
    public async void TransitionToUnderworld()
    {
        foreach (Entity entity in AllEntities)
        {
            if (entity)
                Destroy(entity.gameObject);
        }

        AllEntities.Clear();

        await _underworldOverlay.StartUnderworldAnim();

        _vampireLordController.gameObject.SetActive(true);
        ChangeState(GameStates.VampireLordLoop);
    }

    public void TransitionToOverworld()
    {
        _vampireLordController.gameObject.SetActive(false);

        _underworldOverlay.SetRegularMode();
        Debug.Log("Underworld anim done - Resurrecting player!");

        ResurrectPlayer();
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

    public void OnPlayerDie()
    {
        Debug.Log($"player died");
        TransitionToUnderworld();
        Destroy(PlayerController.gameObject);
    }
    public void InvokeResurrectPlayer()
    {
        OnResurrectPlayer?.Invoke();
    }
    public void OnEntityDie(Entity entity)
    {
        if (entity)
        {
            Debug.Log($"{entity.Data.Name} died");
            Destroy(entity.gameObject);
        }
    }
    private void ResurrectPlayer()
    {
        GameObject newPlayer = Instantiate(_playerPrefab, _playerSpawn);
        PlayerController newPlayerController = newPlayer.GetComponent<PlayerController>();
        newPlayerController.AbsorbedEntity = _chosenEngraved.Data;
        ChangeState(GameStates.PlayerLoop);
    }
}
