using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NutoriousSystem : MonoBehaviour
{
    [SerializeField]
    private  int nutoriousPoints = 0;
    [SerializeField]
    private int[] spawnMilestones;
    private int curSpawnMilestone = 0;
    [SerializeField]
    private Entity entityToSpawn;
    [SerializeField]
    private MapManager mapManager;
    private void Start()
    {
        CheckSpawnMilestone();
    }
    private void OnEnable()
    {
        Entity.OnEntityDeath += OnEntityDeath;
    }

    private void OnDisable()
    {
        Entity.OnEntityDeath -= OnEntityDeath;
    }
    private void OnEntityDeath(Entity obj)
    {
        if (nutoriousPoints % 3 == 0)
        {
            var tempObj = Instantiate(entityToSpawn, mapManager.GetRandomVillagerSpawnPosition(), Quaternion.identity);
        }
        nutoriousPoints++;


    }
    private void OnEntityDeath()
    {
        if (nutoriousPoints % 3 == 0)
        {
        }
        nutoriousPoints++;

    }
    private void CheckSpawnMilestone()
    {
        var shouldSpawn = spawnMilestones.Where(milestone => milestone == curSpawnMilestone).Any();
        if(shouldSpawn)
        {
            var tempObj = Instantiate(entityToSpawn, mapManager.GetRandomVillagerSpawnPosition(), Quaternion.identity);
        }
    }
}