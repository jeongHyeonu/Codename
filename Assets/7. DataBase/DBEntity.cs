[System.Serializable]
public class WeaponDBEntity
{
    public string weaponID;
    public float damage;
    public float reloadSpeed;
    public int magazine;
    public float useTime;
    public float bulletSpeed;
}

[System.Serializable]
public class EnemyDBEntity
{
    public float health;
    public float speed;
    public float damage;
    public float useTime;
}

