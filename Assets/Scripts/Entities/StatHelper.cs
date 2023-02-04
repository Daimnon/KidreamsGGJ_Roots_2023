public static class StatHelper
{
    public static int GetHp(PlayerData myData, int damageTaken, EntityData additionalData)
        => myData.Hp - damageTaken + (additionalData ? additionalData.Hp : 0);

    public static int GetSpeed(PlayerData myData, EntityData data)
        => myData.Speed + (data ? data.Speed : 0);

    public static int GetDamage(PlayerData myData, EntityData data)
        => myData.Damage + (data ? data.Damage : 0);

    public static int GeSpeed(PlayerData myData, EntityData data)
        => myData.Speed + (data ? data.Speed : 0);

    public static int GetVision(PlayerData myData, EntityData data)
        => myData.Vision + (data ? data.Vision : 0);
}