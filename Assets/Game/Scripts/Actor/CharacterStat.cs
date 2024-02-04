[System.Serializable]
public class CharacterStat
{
    public int healthPoint = 1;
    public float moveSpeed=10;
    public float jumpPower=5;
    public float attackDamage = 10;
    public int coin = 10;

    public CharacterStat(int healthPoint, float moveSpeed, float jumpPower, float attackDamage,int coin)
    {
        this.healthPoint = healthPoint;
        this.moveSpeed = moveSpeed;
        this.jumpPower = jumpPower;
        this.attackDamage = attackDamage;
        this.coin = coin;
    }
}