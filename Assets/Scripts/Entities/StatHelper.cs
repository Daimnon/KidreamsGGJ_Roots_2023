using System.Collections.Generic;
using System.Linq;

public static class StatHelper
{
    public static int GetHp(PlayerData myData, int damageTaken, IEnumerable<EntityData> additionalData)
        => myData.Hp - damageTaken + GetStatIfAbsorbed(additionalData, EntityData.Stat.Hp);

    public static int GetSpeed(PlayerData myData, IEnumerable<EntityData> additionalData)
        => myData.Speed + GetStatIfAbsorbed(additionalData, EntityData.Stat.Speed);

    public static int GetDamage(PlayerData myData, IEnumerable<EntityData> additionalData)
        => myData.Damage + GetStatIfAbsorbed(additionalData, EntityData.Stat.Damage);

    public static int GeSpeed(PlayerData myData, IEnumerable<EntityData> additionalData)
        => myData.Speed + GetStatIfAbsorbed(additionalData, EntityData.Stat.Speed);

    public static int GetVision(PlayerData myData, IEnumerable<EntityData> additionalData)
        => myData.Vision + GetStatIfAbsorbed(additionalData, EntityData.Stat.Vision);

    private static int GetStatIfAbsorbed(IEnumerable<EntityData> datas, EntityData.Stat stat)
    {
        return datas.Sum(data =>
        {
            if (!data) return 0;
            if (data.AbsorbedStat == stat) return data.GetStat(stat);
            return 0;
        });
    }
}