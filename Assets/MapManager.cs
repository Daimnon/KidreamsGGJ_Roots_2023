using System.Linq;
using Extensions;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    [SerializeField] private Transform _randomPlacesParent;

    private Transform[] _randomPlaces;

    private Transform[] RandomPlaces => _randomPlaces ??= _randomPlacesParent.Cast<Transform>().ToArray();
    
    public Transform GetRandomPlaceTransform() => RandomPlaces.GetRandom();

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
