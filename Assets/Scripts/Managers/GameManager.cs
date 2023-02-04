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

    // win/lose condition: blood empty = perma-death, blood full = vampireLord resurrection, value = 0-100.
    [SerializeField] private int _bloodAmount = 25;
    public int BloodAmount => _bloodAmount;

    [SerializeField] private GameObject _playerPrefab, _currentVampireLord;
    public GameObject PlayerPrefab => _playerPrefab;
    public GameObject CurrentVampireLord => _currentVampireLord;

    [SerializeField] private PlayerController _playerController;
    public PlayerController PlayerController { get => _playerController; set => value = _playerController; }
    
    [SerializeField] private VampireLordController _vampireLordController;
    public VampireLordController VampireLordController => _vampireLordController;

    [SerializeField] private Transform _playerSpawn, _vampireLordSpawn;
    public Transform PlayerSpawn => _playerSpawn;
    public Transform VampireLordSpawn => _vampireLordSpawn;

    [SerializeField] private List<Villager> _engraved;
    public List<Villager> Engraved { get => _engraved; set => value = _engraved; }

    [SerializeField] private Villager _chosenEngraved;
    public Villager ChosenEngraved { get => _chosenEngraved; set => value = _chosenEngraved; }

    [SerializeField] private List<Entity> _allEntities;
    public List<Entity> AllEntities { get => _allEntities; set => value = _allEntities; }

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
        Debug.Log("Underworld anim done - Resurrecting player!");

        _vampireLordController.gameObject.SetActive(true);
        //ResurrectPlayer();
    }

    public void TransitionToOverworld()
    {
        _vampireLordController.gameObject.SetActive(false);

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
        newPlayerController.AbsorbedEntity = _chosenEngraved.Data;
        ChangeState(GameStates.PlayerLoop);
    }
    public void Test()
    {

    }
}
