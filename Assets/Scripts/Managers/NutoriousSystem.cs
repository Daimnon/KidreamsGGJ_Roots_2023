using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutoriousSystem : MonoBehaviour
{
    [SerializeField]
    private int nutoriousPoints = 0;
    [SerializeField]
    private Entity entityToSpawn;
    [SerializeField]
    private MapManager mapManager;


    private void AddPoints() => nutoriousPoints++;
    private void OnAddPoints()
    {
        if(nutoriousPoints == 3)
        {
            var tempObj = Instantiate(entityToSpawn.GetComponent<GameObject>(), mapManager.GetRandomVillagerSpawnPosition(), Quaternion.identity);
        }
    }

}
