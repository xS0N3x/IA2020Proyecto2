using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosestEnemy : MonoBehaviour
{
    public float minDist = Mathf.Infinity;
    public Unit closestEnemy; //Tropas rojas
    public Unit[] enemies;
    //public bool hasMoved;
    public int tileSpeed;
    public List<Tile> walkableTiles;
    public Tile closestTile;
    public float moveSpeed;
    public List<Unit> enemiesInRange = new List<Unit>();
    public int attackRadius;

    public bool isKing;
    public Text displayedText;
    public GameObject deathEffect;
    public int playerNumber;

    // Attack Stats
    public int health;
    public int attackDamage;
    public int defenseDamage;
    public int armor;

    public DamageIcon damageIcon;

    // Start is called before the first frame update
    void Start()
    {
        enemies = FindObjectsOfType<Unit>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FindNearestEnemy(Unit[] lista) {
        foreach (Unit enemy in lista)
        {
            if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= minDist)
            {
                minDist = Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y);
                closestEnemy = enemy;
            }
        }
    }

    public void GetWalkableTiles()
    { // Looks for the tiles the unit can walk on

        Tile[] tiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles)
        {
            if (Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.y - tile.transform.position.y) <= tileSpeed)
            { // how far he can move
                if (tile.isClear() == true)
                { // is the tile clear from any obstacles
                    walkableTiles.Add(tile); //Acuerdate de vaciarla
                }

            }
        }

    }

    public void FindClosestTile() {

        minDist = Mathf.Infinity;

        foreach (Tile tile in walkableTiles) {
            if (Mathf.Abs(closestEnemy.transform.position.x - tile.transform.position.x) + Mathf.Abs(closestEnemy.transform.position.y - tile.transform.position.y) < minDist) {
                minDist = Mathf.Abs(closestEnemy.transform.position.x - tile.transform.position.x) + Mathf.Abs(closestEnemy.transform.position.y - tile.transform.position.y);
                closestTile = tile;
            }
        }

    }

    public void GetEnemies()
    {

        enemiesInRange.Clear();

        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= attackRadius) // check is the enemy is near enough to attack
            {

             enemiesInRange.Add(enemy);
             enemy.weaponIcon.SetActive(true);

            }
        }
    }

    public Unit FindLowestEnemy(List<Unit> lista)
    {
        Unit lowerEnemy = null;
        float minHealth = Mathf.Infinity;

        foreach (Unit enemy in lista)
        {
            if (enemy.health <= minHealth)
            {
                minHealth = enemy.health;
                lowerEnemy = enemy;
            }
        }

        return lowerEnemy;
    }
}
