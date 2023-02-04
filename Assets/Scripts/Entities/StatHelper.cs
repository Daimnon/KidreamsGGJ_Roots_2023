public static class StatHelper
{
    public static int GetHp(PlayerData myData, int damageTaken, EntityData additionalData)
        => myData.Hp - damageTaken + GetStatIfAbsorbed(additionalData, EntityData.Stat.Hp);

    public static int GetSpeed(PlayerData myData, EntityData additionalData)
        => myData.Speed + GetStatIfAbsorbed(additionalData, EntityData.Stat.Speed);

    public static int GetDamage(PlayerData myData, EntityData additionalData)
        => myData.Damage + GetStatIfAbsorbed(additionalData, EntityData.Stat.Damage);

    public static int GeSpeed(PlayerData myData, EntityData additionalData)
        => myData.Speed + GetStatIfAbsorbed(additionalData, EntityData.Stat.Speed);

    public static int GetVision(PlayerData myData, EntityData additionalData)
        => myData.Vision + GetStatIfAbsorbed(additionalData, EntityData.Stat.Vision);

    private static int GetStatIfAbsorbed(EntityData data, EntityData.Stat stat)
    {
        if (!data) return 0;
        if (data.AbsorbedStat == stat) return data.GetStat(stat);
        return 0;
    }
}