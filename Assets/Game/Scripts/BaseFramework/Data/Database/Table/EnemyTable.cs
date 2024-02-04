using System;
using System.Data;

[System.Serializable]
public class EnemyTable : DataTable<string, EnemyEntity>
{
    public override void GetDatabase()
    {
        DB_Enemy.ForEachEntity(e => Get(e));
    }

    private void Get(DB_Enemy e)
    {
        var enemyE = new EnemyEntity(e);
        Dictionary.Add(enemyE.Id, enemyE);
    }
}