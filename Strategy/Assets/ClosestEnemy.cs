using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestEnemy : MonoBehaviour
{
    public float minDist = Mathf.Infinity;
    public Unit closestEnemy; //Tropas rojas
    public Unit[] enemiesInRange;
    //public bool hasMoved;
    public int tileSpeed;
    public List<Tile> walkableTiles;
    public Tile closestTile;
    public float moveSpeed;


    // Start is called before the first frame update
    void Start()
    {
        enemiesInRange = FindObjectsOfType<Unit>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FindNearestEnemy() {
        foreach (Unit enemy in enemiesInRange)
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
        /*if (hasMoved == true)
        {
            return;
        }*/

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
}
