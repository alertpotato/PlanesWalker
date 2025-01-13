using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    public int[] PlayerSupply = new int[4]; // food, weapons, money, wisdom
    public int[] EnemySupply = new int[4]; // food, weapons, money, wisdom

    public void Reset()
    {
        for (int i = 0; i < 4; i++)
        {
            PlayerSupply[i] = 0;
            EnemySupply[i] = 0;
        }
    }
    public int[] AddSupply(int index,int value)
    {
        PlayerSupply[index] += value;
        return PlayerSupply;
    }
    public int[] AddEnemySupply(int index,int value)
    {
        EnemySupply[index] += value;
        return EnemySupply;
    }
}