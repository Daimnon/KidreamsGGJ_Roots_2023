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
    public Vector3 GetRandomVillagerSpawnPosition() => VillagerSpawnPoints.GetRandom().position;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Vector3 GetRandomRunawayPlace(Vector3 entityPos, Vector3 playerTransformPosition)
    {
        return _randomPlaces.Select(trans => trans.position).Where(pos =>
        {
            var entityToTargetVec = pos - entityPos;
            var playerChaseVec = entityPos - playerTransformPosition;

            return Vector3.Dot(playerChaseVec, entityToTargetVec) < 0f;
        }).ToArray().GetRandom();
    }
}
