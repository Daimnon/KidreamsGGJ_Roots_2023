using System.Linq;
using Extensions;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance
    {
        get => _instance ??= FindObjectOfType<MapManager>();
        private set => _instance = value;
    }
    
    [SerializeField] private Transform _randomPlacesParent;
    [SerializeField] private Transform _villagerSpawnParent;

    private Transform[] _randomPlaces;
    private Transform[] _villagerSpawns;

    private Transform[] RandomPlaces => _randomPlaces ??= _randomPlacesParent.Cast<Transform>().ToArray();
    private Transform[] VillagerSpawnPoints => _villagerSpawns ??= _villagerSpawnParent.Cast<Transform>().ToArray();
    
    public Transform GetRandomPlaceTransform() => RandomPlaces.GetRandom();
    public Vector3 GetVillagerSpawnPosition() => VillagerSpawnPoints.GetRandom().position;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
