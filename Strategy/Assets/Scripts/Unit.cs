﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public bool isSelected;
    public bool hasMoved;

    public int tileSpeed;
    public float moveSpeed;

    private GM gm;

    public int attackRadius;
    public bool hasAttacked;
    public List<ClosestEnemy> enemiesInRange = new List<ClosestEnemy>();

    public int playerNumber;
    public bool isDead = false;

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

    public void UpdateHealthDisplay ()
    {
        if (isKing)
        {
            displayedText.text = health.ToString();
        }
    }

    private void OnMouseDown() // select character or deselect if already selected
    {
        
        ResetWeaponIcon();

        if (isSelected == true)
        {
            
            isSelected = false;
            gm.selectedUnit = null;
            gm.ResetTiles();

        }
        else {
            if (playerNumber == gm.playerTurn) { // select unit only if it's his turn / dejar player 1 solo
                if (gm.selectedUnit != null)
                { // deselect the unit that is currently selected, so there's only one isSelected unit at a time
                    gm.selectedUnit.isSelected = false;
                }
                gm.ResetTiles();

                gm.selectedUnit = this;

                isSelected = true;
				if(source != null){
					source.Play();
				}
				
                GetWalkableTiles();
                GetEnemies();
            }

        }


        /*if (enemiesInRange.Count > 0 && enemiesInRange.Contains(gm.selectedEnemy)) {
            
            gm.selectedUnit.Attack(gm.selectedEnemy);
        }*/

        /*Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        if (col != null)
        {
            Unit unit = col.GetComponent<Unit>(); // double check that what we clicked on is a unit
            ClosestEnemy unit2 = col.GetComponent<ClosestEnemy>(); // double check that what we clicked on is a enemy
            if ((unit != null || unit2 != null) && gm.selectedUnit != null )
            {
                if (enemiesInRange.Contains(unit2) && !gm.selectedUnit.hasAttacked)
                { // does the currently selected unit have in his list the enemy we just clicked on
                    gm.selectedUnit.Attack(unit2);

                }
            }
        }*/
        
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            gm.UpdateInfoPanel(this);
        }
    }



    void GetWalkableTiles() { // Looks for the tiles the unit can walk on
        if (hasMoved == true) {
            return;
        }

        Tile[] tiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles) {
            if (Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.y - tile.transform.position.y) <= tileSpeed)
            { // how far he can move
                if (tile.isClear() == true)
                { // is the tile clear from any obstacles
                    tile.Highlight();
                }

            }          
        }
    }

    void GetEnemies() {
    
        enemiesInRange.Clear();

        ClosestEnemy[] enemies = FindObjectsOfType<ClosestEnemy>();
        foreach (ClosestEnemy enemy in enemies)
        {
            if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= attackRadius) // check is the enemy is near enough to attack
            {
                if (enemy.playerNumber != gm.playerTurn && !hasAttacked) { // make sure you don't attack your allies
                    enemiesInRange.Add(enemy);
                    enemy.weaponIcon.SetActive(true);
                }

            }
        }

        Village[] villages = FindObjectsOfType<Village>();
        foreach (Village village in villages)
        {
            if (Mathf.Abs(transform.position.x - village.transform.position.x) + Mathf.Abs(transform.position.y - village.transform.position.y) <= attackRadius) // check is the enemy is near enough to attack
            {
                if (village.playerNumber != gm.playerTurn && !hasAttacked)
                { // make sure you don't attack your allies

                    village.weaponIcon.SetActive(true);
                }

            }
        }



    }

    public void Move(Transform movePos)
    {
        gm.ResetTiles();
        StartCoroutine(StartMovement(movePos));
    }

    public void Attack(ClosestEnemy enemy) {
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
        } else {
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
         
            if (deathEffect != null){
				Instantiate(deathEffect, transform.position, Quaternion.identity); /*HEMOS CAMBIADO ESTA VAINA A VER*/
                camAnim.SetTrigger("shake");
			}

            if (enemy.isKing)
            {
                gm.ShowVictoryPanel(enemy.playerNumber);
            }

            GetWalkableTiles(); // check for new walkable tiles (if enemy has died we can now walk on his tile)
            gm.RemoveInfoPanel(enemy);
            enemy.isDead = true;
            enemy.gameObject.SetActive(false);
            //Destroy(enemy.gameObject);
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
            isDead = true;
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }

        gm.UpdateInfoStats();
  

    }

    public void AttackVillage(Village villag)
    {
        hasAttacked = true;

        int enemyDamege = attackDamage;
        

        if (enemyDamege >= 1)
        {
            villag.health -= enemyDamege;
            DamageIcon d = Instantiate(damageIcon, villag.transform.position, Quaternion.identity);
            d.Setup(enemyDamege);
        }

        if (villag.health <= 0)
        {

            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity); /*HEMOS CAMBIADO ESTA VAINA A VER*/
                camAnim.SetTrigger("shake");
            }

            GetWalkableTiles(); // check for new walkable tiles (if enemy has died we can now walk on his tile)
            villag.isDead = true;
            villag.gameObject.SetActive(false);
            //Destroy(enemy.gameObject);
        }

    }

    public void ResetWeaponIcon() {
        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            enemy.weaponIcon.SetActive(false);
        }
    }

    IEnumerator StartMovement(Transform movePos) { // Moves the character to his new position.


        while (transform.position.x != movePos.position.x) { // first aligns him with the new tile's x pos
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
        gm.ResetWeaponIcon();
        GetEnemies();
        gm.MoveInfoPanel(this);
    }




}
