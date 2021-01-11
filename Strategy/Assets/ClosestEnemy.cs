using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosestEnemy : MonoBehaviour
{
    public float minDist = Mathf.Infinity;
    public float maxDist = -1;
    public Unit closestEnemy; //Tropas rojas
    public ClosestEnemy closestAlly;
    public Village closestVillage;
    public Village closestAllyVillage;
    public Unit[] enemies;
    //public bool hasMoved;
    public int tileSpeed;
    public List<Tile> walkableTiles;
    public List<Tile> allyTiles;
    public Tile closestTile;
    public float moveSpeed;
    public List<Unit> enemiesInRange = new List<Unit>();
    public List<Village> villagesInRange = new List<Village>();
    public ClosestEnemy[] allies;
    public int attackRadius;
    public GM gm;
    public bool hasMoved = false;
    public bool isDead = false;

    public bool isKing;
    public Text displayedText;
    public GameObject deathEffect;
    public int playerNumber;
    public int cost;

    // Attack Stats
    public int health;
    public int attackDamage;
    public int defenseDamage;
    public int armor;

    public DamageIcon damageIcon;
    public GameObject weaponIcon;

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
            if (!enemy.isDead) {

                if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= minDist)
                {
                    minDist = Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y);
                    closestEnemy = enemy;
                }

            }
            
        }

        minDist = Mathf.Infinity;
    }

    public void FindNearestAlly(ClosestEnemy[] lista)
    {

        closestAlly = null;

        foreach (ClosestEnemy ally in lista)
        {
            if (ally.tag != gameObject.tag && !ally.isDead) {

                if (Mathf.Abs(transform.position.x - ally.transform.position.x) + Mathf.Abs(transform.position.y - ally.transform.position.y) <= minDist)
                {
                    minDist = Mathf.Abs(transform.position.x - ally.transform.position.x) + Mathf.Abs(transform.position.y - ally.transform.position.y);
                    closestAlly = ally;
                }

            }
            
        }

        minDist = Mathf.Infinity;
    }

    public void FindNearestAllyWithTag(ClosestEnemy[] lista, string tag)
    {
        closestAlly = null;

        foreach (ClosestEnemy ally in lista)
        {
            if (ally.tag != gameObject.tag && ally.tag == tag && !isDead)
            {

                if (Mathf.Abs(transform.position.x - ally.transform.position.x) + Mathf.Abs(transform.position.y - ally.transform.position.y) <= minDist)
                {
                    minDist = Mathf.Abs(transform.position.x - ally.transform.position.x) + Mathf.Abs(transform.position.y - ally.transform.position.y);
                    closestAlly = ally;
                }

            }

        }

        minDist = Mathf.Infinity;
    }

    public void FindNearestVillage(Village[] lista)
    {

        foreach (Village village in lista)
        {
            if (village.playerNumber == 1 && !village.isDead) {
                if (Mathf.Abs(transform.position.x - village.transform.position.x) + Mathf.Abs(transform.position.y - village.transform.position.y) <= minDist)
                {
                    minDist = Mathf.Abs(transform.position.x - village.transform.position.x) + Mathf.Abs(transform.position.y - village.transform.position.y);
                    closestVillage = village;
                }
            }
            
        }

        minDist = Mathf.Infinity;
    }

    public void FindNearestAllyVillage(Village[] lista)
    {

        foreach (Village village in lista)
        {
            if (village.playerNumber == 2 && !village.isDead)
            {
                if (Mathf.Abs(transform.position.x - village.transform.position.x) + Mathf.Abs(transform.position.y - village.transform.position.y) <= minDist)
                {
                    minDist = Mathf.Abs(transform.position.x - village.transform.position.x) + Mathf.Abs(transform.position.y - village.transform.position.y);
                    closestAllyVillage = village;
                }
            }

        }

        minDist = Mathf.Infinity;
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

    public void AllyTiles(ClosestEnemy ally)
    { // Looks for the tiles the unit can walk on

        Tile[] tiles = FindObjectsOfType<Tile>();
        allyTiles.Clear();
        foreach (Tile tile in tiles)
        {
            if (!ally.isDead) {

                if (Mathf.Abs(ally.transform.position.x - tile.transform.position.x) + Mathf.Abs(ally.transform.position.y - tile.transform.position.y) <= tileSpeed)
                { // how far he can move
                    if (tile.isClear() == true)
                    { // is the tile clear from any obstacles
                        allyTiles.Add(tile); //Acuerdate de vaciarla
                    }
                }

            }
            
        }
    }

    public void IrseACuenca() {
        maxDist = -1;
        FindNearestEnemy(enemies);
        foreach (Tile tile in walkableTiles)
        {
            if (!closestEnemy.isDead)
            {
                if (Mathf.Abs(closestEnemy.transform.position.x - tile.transform.position.x) + Mathf.Abs(closestEnemy.transform.position.y - tile.transform.position.y) > maxDist)
                {
                    maxDist = Mathf.Abs(closestEnemy.transform.position.x - tile.transform.position.x) + Mathf.Abs(closestEnemy.transform.position.y - tile.transform.position.y);
                    closestTile = tile;
                }
            }

        }
    }

    public void FarestTile(List<Tile> walkTiles)
    {
        maxDist = -1;
        FindNearestEnemy(enemies);
        List<Tile> commonTiles = new List<Tile>();
        foreach (Tile a in walkTiles)
        {
            if (allyTiles.Contains(a))
            {
                commonTiles.Add(a);
            }
        }
        if (commonTiles.Count == 0)
        {
            FindClosestTile(closestAlly.transform.position);
        }
        else
        {
            foreach (Tile tile in commonTiles)
            {
                if (!closestEnemy.isDead) {
                    if (Mathf.Abs(closestEnemy.transform.position.x - tile.transform.position.x) + Mathf.Abs(closestEnemy.transform.position.y - tile.transform.position.y) > maxDist)
                    {
                        maxDist = Mathf.Abs(closestEnemy.transform.position.x - tile.transform.position.x) + Mathf.Abs(closestEnemy.transform.position.y - tile.transform.position.y);
                        closestTile = tile;
                    }
                }
                
            }
        }
    }

    public void MiddleTile(List<Tile> guolkTails)
    {
        minDist = Mathf.Infinity;
        FindNearestEnemy(enemies);
        List<Tile> commonTiles = new List<Tile>();
        foreach (Tile a in guolkTails)
        {
            if (allyTiles.Contains(a))
            {
                commonTiles.Add(a);
            }
        }
        if (commonTiles.Count == 0)
        {
            FindClosestTile(closestAlly.transform.position);
        }
        else
        {
            foreach (Tile tile in commonTiles)
            {
                if (!closestEnemy.isDead) {

                    if (Mathf.Abs(closestEnemy.transform.position.x - tile.transform.position.x) + Mathf.Abs(closestEnemy.transform.position.y - tile.transform.position.y) < minDist)
                    {
                        minDist = Mathf.Abs(closestEnemy.transform.position.x - tile.transform.position.x) + Mathf.Abs(closestEnemy.transform.position.y - tile.transform.position.y);
                        closestTile = tile;
                    }
                }
                
            }
        }
    }

    public void FindClosestTile() {

        minDist = Mathf.Infinity;

        foreach (Tile tile in walkableTiles) {
            if (!closestEnemy.isDead) {

                if (Mathf.Abs(closestEnemy.transform.position.x - tile.transform.position.x) + Mathf.Abs(closestEnemy.transform.position.y - tile.transform.position.y) < minDist)
                {
                    minDist = Mathf.Abs(closestEnemy.transform.position.x - tile.transform.position.x) + Mathf.Abs(closestEnemy.transform.position.y - tile.transform.position.y);
                    closestTile = tile;
                }
            }
            
        }

        minDist = Mathf.Infinity;

    }

    public void FindClosestTile(Vector3 posicion)
    {

        minDist = Mathf.Infinity;

        foreach (Tile tile in walkableTiles)
        {
            if (Mathf.Abs(posicion.x - tile.transform.position.x) + Mathf.Abs(posicion.y - tile.transform.position.y) < minDist)
            {
                minDist = Mathf.Abs(posicion.x - tile.transform.position.x) + Mathf.Abs(posicion.y - tile.transform.position.y);
                closestTile = tile;
            }
        }

        minDist = Mathf.Infinity;

    }

    public void GetEnemies()
    {

        enemiesInRange.Clear();

        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            if (!enemy.isDead) {
                if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= attackRadius) // check is the enemy is near enough to attack
                {

                    enemiesInRange.Add(enemy);
                    enemy.weaponIcon.SetActive(true);

                }
            }
            
        }
    }



    public void GetVillages()
    {

        villagesInRange.Clear();

        Village[]villages = FindObjectsOfType<Village>();
        foreach (Village village in villages)
        {
            if (!village.isDead) {
                if (Mathf.Abs(transform.position.x - village.transform.position.x) + Mathf.Abs(transform.position.y - village.transform.position.y) <= attackRadius) // check is the enemy is near enough to attack
                {

                    villagesInRange.Add(village);
                    village.weaponIcon.SetActive(true);

                }
            }
            
        }

    }

    public Unit FindLowestEnemy(List<Unit> lista)
    {
        Unit lowerEnemy = null;
        float minHealth = Mathf.Infinity;

        foreach (Unit enemy in lista)
        {
            if (!enemy.isDead) {
                if (enemy.health <= minHealth)
                {
                    minHealth = enemy.health;
                    lowerEnemy = enemy;
                }
            }
            
        }

        return lowerEnemy;
    }

    public Village FindLowestVillage(List<Village> lista)
    {
        Village lowerVillage = null;
        float minHealth = Mathf.Infinity;

        foreach (Village village in lista)
        {
            if (!village.isDead) {

                if (village.health <= minHealth)
                {
                    minHealth = village.health;
                    lowerVillage = village;
                }
            }
            
        }

        return lowerVillage;
    }

    public void UpdateHealthDisplay()
    {
        if (isKing)
        {
            displayedText.text = health.ToString();
        }
    }

    private void OnMouseDown()
    {
        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        if (col != null)
        {
            Debug.Log(col);
          
            ClosestEnemy unit = col.GetComponent<ClosestEnemy>(); // double check that what we clicked on is a enemy
            if (unit != null)
            {
                Debug.Log(unit);
                gm.selectedEnemy = unit;
                allies = FindObjectsOfType<ClosestEnemy>();
                List<ClosestEnemy> allyInRange = new List<ClosestEnemy>();

                foreach (ClosestEnemy ally in allies) {
                    if (Mathf.Abs(gm.selectedUnit.transform.position.x - ally.transform.position.x) + Mathf.Abs(gm.selectedUnit.transform.position.y - ally.transform.position.y) <= gm.selectedUnit.attackRadius) // check is the enemy is near enough to attack
                    {
                        allyInRange.Add(ally);
                        ally.weaponIcon.SetActive(true);

                    }
                }

                if (allyInRange.Contains(unit) && !gm.selectedUnit.hasAttacked) {
                    gm.selectedUnit.Attack(unit);
                    gm.ResetWeaponIcon();
                }

            }
        }
    }
}
