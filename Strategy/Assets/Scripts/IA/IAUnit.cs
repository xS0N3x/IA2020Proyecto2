using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAUnit : MonoBehaviour
{
    //public List<IAUnit> troops = new List<IAUnit>();
    IAUnit[] troops;
    Unit[] enemies;
    


    public Tile tileScript;
    public StateMachine smScript;





    public bool isSelected;
    public bool hasMoved;

    public int tileSpeed;
    public float moveSpeed;

    private GM gm;

    public int attackRadius;
    public bool hasAttacked;
    public List<Unit> enemiesInRange = new List<Unit>();

    public int playerNumber;

    public GameObject weaponIcon;

    // Attack Stats
    public int health;
    public int attackDamage;
    public int defenseDamage;
    public int armor;

    public DamageIcon damageIcon;

    public int cost;

    public GameObject deathEffect;

    private Animator camAnim;

    public bool isKing;

    private AudioSource source;

    public Text displayedText;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        camAnim = Camera.main.GetComponent<Animator>();
        gm = FindObjectOfType<GM>();
        UpdateHealthDisplay();

    }

    private void UpdateHealthDisplay()
    {
        if (isKing)
        {
            displayedText.text = health.ToString();
        }
    }

    /* void GetWalkableTiles()
     { // Looks for the tiles the unit can walk on
         if (hasMoved == true)
         {
             return;
         }

         Tile[] tiles = FindObjectsOfType<Tile>();
         foreach (Tile tile in tiles)
         {
             if (Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.y - tile.transform.position.y) <= tileSpeed)
             { // how far he can move
                 if (tile.isClear() == true)
                 { // is the tile clear from any obstacles
                     tile.Highlight();
                 }

             }
         }
     }*/
    private void Update()
    {
        if (smScript.turnIA == true)
        {
            smScript.turnIA = false;
            GetEnemies();
            
        }
    }

    void GetEnemies()
    {
        troops = GameObject.FindGameObjectWithTag("gameMaster").GetComponent<GM>().troops;
        enemies = GameObject.FindGameObjectWithTag("gameMaster").GetComponent<GM>().enemies;
        ManageEnemies();
    }

    void ManageEnemies()
    {
        
        //foreach (IAUnit troop in troops)
        for (int i = 0; i<troops.Length; i++)
        {
            /*float minDist = 10000000;
            Unit closestEnemy;
            foreach (Unit enemy in enemies)
            {
                if (Mathf.Abs(troop.transform.position.x - enemy.transform.position.x) + Mathf.Abs(troop.transform.position.y - enemy.transform.position.y) <= minDist)
                {
                    minDist = Mathf.Abs(troop.transform.position.x - enemy.transform.position.x) + Mathf.Abs(troop.transform.position.y - enemy.transform.position.y);
                    closestEnemy = enemy;
                }
            }*/

            //gm.selectedIAUnit = troop;
            List<Tile> walkableTiles = new List<Tile>();
            walkableTiles = GetWalkableTiles(troops[i]);
            Move(walkableTiles[Random.Range(0, 4)].transform);
            
            /*float aux = troop.transform.position.x;
            StartCoroutine(StartMovement(aux - 1));*/
            
        }
    }

    List<Tile> GetWalkableTiles(IAUnit troop)
    { // Looks for the tiles the unit can walk on
        if (hasMoved == true)
        {
            return null;
        }

        Tile[] tiles = FindObjectsOfType<Tile>();
        List<Tile> aux = new List<Tile>();
        foreach (Tile tile in tiles)
        {
            if (Mathf.Abs(troop.transform.position.x - tile.transform.position.x) + Mathf.Abs(troop.transform.position.y - tile.transform.position.y) <= tileSpeed)
            { // how far he can move
                if (tile.isClear() == true)
                { // is the tile clear from any obstacles
                    tile.Highlight();
                    aux.Add(tile);
                }

            }
        }

        return aux;
    }

    public void Move(Transform movePos)
    {
        gm.ResetTiles();
        StartCoroutine(StartMovement(movePos));
    }

    IEnumerator StartMovement(Transform movePos)
    { // Moves the character to his new position.


        while (transform.position.x != movePos.position.x)
        { // first aligns him with the new tile's x pos
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(movePos.position.x, transform.position.y), moveSpeed * Time.deltaTime);
            yield return null;
        }
        while (transform.position.y != movePos.position.y) // then y
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, movePos.position.y), moveSpeed * Time.deltaTime);
            yield return null;
        }

        hasMoved = true;
        ResetWeaponIcon();
        /*GetEnemies();
        gm.MoveInfoPanel(this);*/
    }


    public void ResetWeaponIcon()
    {
        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            enemy.weaponIcon.SetActive(false);
        }
    }
}

















    /*
    private void OnMouseDown() // select character or deselect if already selected
    {

        ResetWeaponIcon();

        if (isSelected == true)
        {

            isSelected = false;
            gm.selectedUnit = null;
            gm.ResetTiles();

        }
        else
        {
            if (playerNumber == gm.playerTurn)
            { // select unit only if it's his turn / dejar player 1 solo
                if (gm.selectedUnit != null)
                { // deselect the unit that is currently selected, so there's only one isSelected unit at a time
                    gm.selectedUnit.isSelected = false;
                }
                gm.ResetTiles();

                gm.selectedUnit = this;

                isSelected = true;
                if (source != null)
                {
                    source.Play();
                }

                GetWalkableTiles();
                GetEnemies();
            }

        }



        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        if (col != null)
        {
            Unit unit = col.GetComponent<Unit>(); // double check that what we clicked on is a unit
            if (unit != null && gm.selectedUnit != null)
            {
                if (gm.selectedUnit.enemiesInRange.Contains(unit) && !gm.selectedUnit.hasAttacked)
                { // does the currently selected unit have in his list the enemy we just clicked on
                    gm.selectedUnit.Attack(unit);

                }
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            gm.UpdateInfoPanel(this);
        }
    }



    void GetWalkableTiles()
    { // Looks for the tiles the unit can walk on
        if (hasMoved == true)
        {
            return;
        }

        Tile[] tiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles)
        {
            if (Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.y - tile.transform.position.y) <= tileSpeed)
            { // how far he can move
                if (tile.isClear() == true)
                { // is the tile clear from any obstacles
                    tile.Highlight();
                }

            }
        }
    }

    void GetEnemies()
    {

        enemiesInRange.Clear();

        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= attackRadius) // check is the enemy is near enough to attack
            {
                if (enemy.playerNumber != gm.playerTurn && !hasAttacked)
                { // make sure you don't attack your allies
                    enemiesInRange.Add(enemy);
                    enemy.weaponIcon.SetActive(true);
                }

            }
        }
    }

    public void Move(Transform movePos)
    {
        gm.ResetTiles();
        StartCoroutine(StartMovement(movePos));
    }

    void Attack(Unit enemy)
    {
        hasAttacked = true;

        int enemyDamege = attackDamage - enemy.armor;
        int unitDamage = enemy.defenseDamage - armor;

        if (enemyDamege >= 1)
        {
            enemy.health -= enemyDamege;
            enemy.UpdateHealthDisplay();
            DamageIcon d = Instantiate(damageIcon, enemy.transform.position, Quaternion.identity);
            d.Setup(enemyDamege);
        }

        if (transform.tag == "Archer" && enemy.tag != "Archer")
        {
            if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= 1) // check is the enemy is near enough to attack
            {
                if (unitDamage >= 1)
                {
                    health -= unitDamage;
                    UpdateHealthDisplay();
                    DamageIcon d = Instantiate(damageIcon, transform.position, Quaternion.identity);
                    d.Setup(unitDamage);
                }
            }
        }
        else
        {
            if (unitDamage >= 1)
            {
                health -= unitDamage;
                UpdateHealthDisplay();
                DamageIcon d = Instantiate(damageIcon, transform.position, Quaternion.identity);
                d.Setup(unitDamage);
            }
        }

        if (enemy.health <= 0)
        {

            if (deathEffect != null)
            {
                Instantiate(deathEffect, enemy.transform.position, Quaternion.identity);
                camAnim.SetTrigger("shake");
            }

            if (enemy.isKing)
            {
                gm.ShowVictoryPanel(enemy.playerNumber);
            }

            GetWalkableTiles(); // check for new walkable tiles (if enemy has died we can now walk on his tile)
            gm.RemoveInfoPanel(enemy);
            Destroy(enemy.gameObject);
        }

        if (health <= 0)
        {

            if (deathEffect != null)
            {
                Instantiate(deathEffect, enemy.transform.position, Quaternion.identity);
                camAnim.SetTrigger("shake");
            }

            if (isKing)
            {
                gm.ShowVictoryPanel(playerNumber);
            }

            gm.ResetTiles(); // reset tiles when we die
            gm.RemoveInfoPanel(this);
            Destroy(gameObject);
        }

        gm.UpdateInfoStats();


    }

    public void ResetWeaponIcon()
    {
        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            enemy.weaponIcon.SetActive(false);
        }
    }

    IEnumerator StartMovement(Transform movePos)
    { // Moves the character to his new position.


        while (transform.position.x != movePos.position.x)
        { // first aligns him with the new tile's x pos
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(movePos.position.x, transform.position.y), moveSpeed * Time.deltaTime);
            yield return null;
        }
        while (transform.position.y != movePos.position.y) // then y
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, movePos.position.y), moveSpeed * Time.deltaTime);
            yield return null;
        }

        hasMoved = true;
        ResetWeaponIcon();
        GetEnemies();
        gm.MoveInfoPanel(this);
    }




}

    */
