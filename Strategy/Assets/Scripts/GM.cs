using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Threading.Tasks;

public class GM : MonoBehaviour
{
    public Unit selectedUnit;
    public ClosestEnemy selectedEnemy;
    public CharacterCreation scriptCreacion;
    public int playerTurn = 1;

    public Transform selectedUnitSquare;

    private int enemyVillages;
    private int allyVillages;

    private Animator camAnim;
    public Image playerIcon; 
    public Sprite playerOneIcon;
    public Sprite playerTwoIcon;

    public GameObject unitInfoPanel;
    public Vector2 unitInfoPanelShift;
    Unit currentInfoUnit;
    public Text heathInfo;
    public Text attackDamageInfo;
    public Text armorInfo;
    public Text defenseDamageInfo;

    public int player1Gold;
    public int player2Gold;

    public Text player1GoldText;
    public Text player2GoldText;

    public Unit createdUnit;
    public Village createdVillage;

    public ClosestEnemy createdEnemy;
    public Village selectedVillage;

    public GameObject blueVictory;
    public GameObject darkVictory;

	private AudioSource source;

    public ClosestEnemy[] aux;
    public List<GameObject> enemies;
    public ClosestEnemy script;
    public bool turnofinalizado = false;
    public bool turnoiniciado = false;
    public bool movimientosRealizados = false;

    public GameObject villageIA;
    public GameObject EnemyIAArcher;
    public GameObject EnemyIABat;
    public GameObject EnemyIAKnight;


    public ClosestEnemy lowerEnemy;

    Village[] listaVillages;
    ClosestEnemy[] listaAliados;
    Unit[] lista;

    public GameObject unidadCreada;

    private void Start()
    {
		source = GetComponent<AudioSource>();
        camAnim = Camera.main.GetComponent<Animator>();
        GetGoldIncome(1);
        UpdateHealthDisplay(script);
        scriptCreacion = GetComponent<CharacterCreation>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("b")) {
            EndTurn();
        }

        if (selectedUnit != null) // moves the white square to the selected unit!
        {
            selectedUnitSquare.gameObject.SetActive(true);
            selectedUnitSquare.position = selectedUnit.transform.position;
        }
        else
        {
            selectedUnitSquare.gameObject.SetActive(false);
        }


        /*Cambios*/
        if (playerTurn == 2) {

            listaVillages = FindObjectsOfType<Village>();
            listaAliados = FindObjectsOfType<ClosestEnemy>();
            lista = FindObjectsOfType<Unit>();

            foreach (ClosestEnemy pavo in listaAliados) {
                if (pavo.gm == null) {
                    pavo.gm = gameObject.GetComponent<GM>();
                    unidadCreada = pavo.gameObject;
                }
            }

            foreach (Village villa in listaVillages)
            {
                if (villa.gm == null && villa.playerNumber == 2)
                {
                    villa.gm = gameObject.GetComponent<GM>();
                    unidadCreada = villa.gameObject;
                }
            }

            if (!turnoiniciado) {

                if (unidadCreada != null) {

                    if (unidadCreada.tag == "Bat")
                    {
                        unidadCreada.transform.SetParent(GameObject.Find("Bats").transform);
                    }
                    else if (unidadCreada.tag == "Knight")
                    {
                        unidadCreada.transform.SetParent(GameObject.Find("Knights").transform);
                    }
                    else if (unidadCreada.tag == "Archer")
                    {
                        unidadCreada.transform.SetParent(GameObject.Find("Archers").transform);
                    }

                    enemies.Add(unidadCreada);

                    unidadCreada = null;

                }

                StartCoroutine(ManageEnemies());
                turnoiniciado = true;
            }

            if (movimientosRealizados && !turnofinalizado) {
               StartCoroutine(PurchaseEnemies());
            }

            if (turnofinalizado) {
                EndTurn();
                turnofinalizado = false;
                turnoiniciado = false;
            }
        }
        else{
            listaVillages = FindObjectsOfType<Village>();
            listaAliados = FindObjectsOfType<ClosestEnemy>();
            lista = FindObjectsOfType<Unit>();
        }

    }
    
    IEnumerator PurchaseEnemies() {

        int redBats = 0;
        int redKnights = 0;
        int redArchers = 0;
        int redVillages = 0;
        int blueBats = 0;
        int blueKnights = 0;
        int blueArchers = 0;
        int blueVillages = 0;

        //scriptCreacion.cratableTilesIA;

        foreach (Unit unit in lista) {
            if (unit.tag == "Bat") {
                redBats += 1;
            } else if (unit.tag == "Archer") {
                redArchers += 1;
            } else if (unit.tag == "Knigt") {
                redKnights += 1;
            }
        }

        foreach (ClosestEnemy unit in listaAliados)
        {
            if (unit.tag == "Bat")
            {
                blueBats += 1;
            }
            else if (unit.tag == "Archer")
            {
                blueArchers += 1;
            }
            else if (unit.tag == "Knigt")
            {
                blueKnights += 1;
            }
        }

        foreach (Village village in listaVillages) {
            if (village.playerNumber == 1)
            {
                redVillages += 1;
            }
            else {
                blueVillages += 1;
            }
        }

        int azules = blueArchers + blueBats + blueKnights;
        int rojos = redArchers + redBats + redKnights;

        if (azules < rojos)
        {
            //Sustituyo lo que me falte
            if (blueBats < redBats && player2Gold >= 80)
            {
                //comprar Bat --> 80g
                scriptCreacion.SetCreatableTiles();
                player2Gold -= 80;
                float minX = Mathf.Infinity;
                float minY = Mathf.Infinity;
                Tile desiredTile = null;

                foreach (Tile tile in scriptCreacion.cratableTilesIA)
                {

                    if (tile.transform.position.x < minX)
                    {
                        minX = tile.transform.position.x;
                        minY = tile.transform.position.y;
                        desiredTile = tile;
                    }
                    else if (tile.transform.position.y < minY)
                    {
                        minX = tile.transform.position.x;
                        minY = tile.transform.position.y;
                        desiredTile = tile;
                    }

                }
                Instantiate(EnemyIABat, new Vector3(desiredTile.transform.position.x, desiredTile.transform.position.y, 0), Quaternion.identity);
                ResetTiles();
                createdVillage = null;

                yield return new WaitForSecondsRealtime(1);
            }
            else if ((blueArchers < redArchers && player2Gold >= 90) || (blueArchers < redKnights && player2Gold >= 70 && azules < 3))
            {
                //comprar archer --> 70g
                scriptCreacion.SetCreatableTiles();
                player2Gold -= 70;
                float minX = Mathf.Infinity;
                float minY = Mathf.Infinity;
                Tile desiredTile = null;

                foreach (Tile tile in scriptCreacion.cratableTilesIA)
                {

                    if (tile.transform.position.x < minX)
                    {
                        minX = tile.transform.position.x;
                        minY = tile.transform.position.y;
                        desiredTile = tile;
                    }
                    else if (tile.transform.position.y < minY)
                    {
                        minX = tile.transform.position.x;
                        minY = tile.transform.position.y;
                        desiredTile = tile;
                    }

                }
                Instantiate(EnemyIAArcher, new Vector3(desiredTile.transform.position.x, desiredTile.transform.position.y, 0), Quaternion.identity);
                ResetTiles();
                createdVillage = null;

                yield return new WaitForSecondsRealtime(1);
            }
            else if ((blueKnights < redKnights && player2Gold >= 80) || (blueKnights < redKnights && player2Gold >= 40 && azules < 3))
            {
                // comprar knight --> 40g
                scriptCreacion.SetCreatableTiles();
                player2Gold -= 40;
                float minX = Mathf.Infinity;
                float minY = Mathf.Infinity;
                Tile desiredTile = null;

                foreach (Tile tile in scriptCreacion.cratableTilesIA)
                {

                    if (tile.transform.position.x < minX)
                    {
                        minX = tile.transform.position.x;
                        minY = tile.transform.position.y;
                        desiredTile = tile;
                    }
                    else if (tile.transform.position.y < minY)
                    {
                        minX = tile.transform.position.x;
                        minY = tile.transform.position.y;
                        desiredTile = tile;
                    }

                }
                Instantiate(EnemyIAKnight, new Vector3(desiredTile.transform.position.x, desiredTile.transform.position.y, 0), Quaternion.identity);
                ResetTiles();
                createdVillage = null;

                yield return new WaitForSecondsRealtime(1);
            }

        }
        else if (blueVillages - redVillages < 2 && player2Gold >= 100)
        {
            //Comprar villa --> villages +1 --> 100g
            scriptCreacion.SetCreatableTiles();
            player2Gold -= 100;
            float maxX = -Mathf.Infinity;
            float maxY = -Mathf.Infinity;
            Tile desiredTile = null;

            foreach (Tile tile in scriptCreacion.cratableTilesIA) {

                if (tile.transform.position.x > maxX) {
                    maxX = tile.transform.position.x;
                    maxY = tile.transform.position.y;
                    desiredTile = tile;
                } else if (tile.transform.position.y > maxY) {
                    maxX = tile.transform.position.x;
                    maxY = tile.transform.position.y;
                    desiredTile = tile;
                }

            }
            createdVillage = villageIA.GetComponent<Village>();
            Instantiate(createdVillage, new Vector3(desiredTile.transform.position.x, desiredTile.transform.position.y, 0), Quaternion.identity);
            ResetTiles();
            createdVillage = null;

            yield return new WaitForSecondsRealtime(1);
        }
        else {
            //Compro esclavos chetaos
            float dice = Random.Range(0,1);

            if (player2Gold < 100) {
                if (dice <= 0.7)
                {
                    //ahorrar
                }
                else if (dice <= 0.8 && player2Gold >= 80)
                {
                    //comprar bat
                    scriptCreacion.SetCreatableTiles();
                    player2Gold -= 80;
                    float minX = Mathf.Infinity;
                    float minY = Mathf.Infinity;
                    Tile desiredTile = null;

                    foreach (Tile tile in scriptCreacion.cratableTilesIA)
                    {

                        if (tile.transform.position.x < minX)
                        {
                            minX = tile.transform.position.x;
                            minY = tile.transform.position.y;
                            desiredTile = tile;
                        }
                        else if (tile.transform.position.y < minY)
                        {
                            minX = tile.transform.position.x;
                            minY = tile.transform.position.y;
                            desiredTile = tile;
                        }

                    }
                    Instantiate(EnemyIABat, new Vector3(desiredTile.transform.position.x, desiredTile.transform.position.y, 0), Quaternion.identity);
                    ResetTiles();
                    createdVillage = null;

                    yield return new WaitForSecondsRealtime(1);
                }
                else if (dice <= 0.9 && player2Gold >= 70)
                {
                    //comprar archer
                    scriptCreacion.SetCreatableTiles();
                    player2Gold -= 70;
                    float minX = Mathf.Infinity;
                    float minY = Mathf.Infinity;
                    Tile desiredTile = null;

                    foreach (Tile tile in scriptCreacion.cratableTilesIA)
                    {

                        if (tile.transform.position.x < minX)
                        {
                            minX = tile.transform.position.x;
                            minY = tile.transform.position.y;
                            desiredTile = tile;
                        }
                        else if (tile.transform.position.y < minY)
                        {
                            minX = tile.transform.position.x;
                            minY = tile.transform.position.y;
                            desiredTile = tile;
                        }

                    }
                    Instantiate(EnemyIAArcher, new Vector3(desiredTile.transform.position.x, desiredTile.transform.position.y, 0), Quaternion.identity);
                    ResetTiles();
                    createdVillage = null;

                    yield return new WaitForSecondsRealtime(1);
                }
                else if (dice <= 1 && player2Gold >= 40)
                {
                    //comprar knight
                    scriptCreacion.SetCreatableTiles();
                    player2Gold -= 40;
                    float minX = Mathf.Infinity;
                    float minY = Mathf.Infinity;
                    Tile desiredTile = null;

                    foreach (Tile tile in scriptCreacion.cratableTilesIA)
                    {

                        if (tile.transform.position.x < minX)
                        {
                            minX = tile.transform.position.x;
                            minY = tile.transform.position.y;
                            desiredTile = tile;
                        }
                        else if (tile.transform.position.y < minY)
                        {
                            minX = tile.transform.position.x;
                            minY = tile.transform.position.y;
                            desiredTile = tile;
                        }

                    }
                    Instantiate(EnemyIAKnight, new Vector3(desiredTile.transform.position.x, desiredTile.transform.position.y, 0), Quaternion.identity);
                    ResetTiles();
                    createdVillage = null;

                    yield return new WaitForSecondsRealtime(1);
                }
            }
            else {
                if (dice <= 0.3)
                {

                }
                else if (dice <= 0.55)
                {
                    //comprar bat
                }
                else if (dice <= 0.85)
                {
                    //comprar archer
                }
                else if (dice <= 1)
                {
                    //comprar knight
                }
            }
            
        }

        turnofinalizado = true;

    }

    
    IEnumerator ManageEnemies() {

        /*if (unidadCreada.tag == "Bat") {
            unidadCreada.transform.SetParent(GameObject.Find("Bats").transform);
        } else if (unidadCreada.tag == "Knight") {
            unidadCreada.transform.SetParent(GameObject.Find("Knights").transform);
        } else if (unidadCreada.tag == "Archer"){
            unidadCreada.transform.SetParent(GameObject.Find("Archer").transform);
        }
        
        enemies.Add(unidadCreada);

        unidadCreada = null;*/

        foreach (GameObject enemy in enemies)
        {
            script = enemy.GetComponent<ClosestEnemy>();

            string tag = script.tag;

            listaVillages = FindObjectsOfType<Village>();
            listaAliados = FindObjectsOfType<ClosestEnemy>();
            lista = FindObjectsOfType<Unit>();

            switch (tag) {
                case "King":
                    if (script.hasMoved == false)
                    {
                        script.GetEnemies(); //Compruebo si hay enemigos al alcance actualmente
                        if (script.enemiesInRange.Count > 0)
                        {
                            Unit lowerEnemy = script.FindLowestEnemy(script.enemiesInRange);
                            StartCoroutine(Attack(lowerEnemy, script));
                            yield return new WaitForSecondsRealtime(1);
                        }
                        else
                        {
                            script.FindNearestAlly(listaAliados);

                            Vector3 puntoMedio;

                            allyVillages = 0;
                            foreach (Village village in listaVillages)
                            {
                                if (village.playerNumber == 2)
                                {
                                    allyVillages += 1;
                                }
                            }

                            if (script.closestAlly == null)
                            {
                                script.FindNearestEnemy(lista);
                                script.GetWalkableTiles();
                                script.IrseACuenca();
                                StartCoroutine(StartMovement(enemy, script));
                                yield return new WaitForSecondsRealtime(1);

                                script.GetEnemies(); //Compruebo si hay enemigos al alcance actualmente
                                if (script.enemiesInRange.Count > 0)
                                {

                                    Unit lowerEnemy = script.FindLowestEnemy(script.enemiesInRange);
                                    StartCoroutine(Attack(lowerEnemy, script));
                                    yield return new WaitForSecondsRealtime(1);
                                }
                            }
                            else {

                                if (allyVillages > 0)
                                {
                                    script.FindNearestAllyVillage(listaVillages);
                                    puntoMedio = new Vector3(Mathf.Floor((script.closestAlly.transform.position.x + script.closestAllyVillage.transform.position.x) / 2),
                                                     Mathf.Floor((script.closestAlly.transform.position.y + script.closestAllyVillage.transform.position.y) / 2),
                                                     script.transform.position.z);
                                }
                                else
                                {

                                    puntoMedio = script.closestAlly.transform.position;
                                }
                                script.GetWalkableTiles();
                                script.FindClosestTile(puntoMedio);
                                StartCoroutine(StartMovement(enemy, script));
                                yield return new WaitForSecondsRealtime(1);

                                script.GetEnemies(); //Compruebo si hay enemigos al alcance actualmente
                                if (script.enemiesInRange.Count > 0)
                                {

                                    Unit lowerEnemy = script.FindLowestEnemy(script.enemiesInRange);
                                    StartCoroutine(Attack(lowerEnemy, script));
                                    yield return new WaitForSecondsRealtime(1);
                                }
                            }

                            
                        }
                        script.hasMoved = true;
                    }
                    

                    break;
                case "Knight":
                    if (script.hasMoved == false)
                    {
                        script.GetEnemies();
                        if (script.enemiesInRange.Count > 0)
                        {
                            Unit lowerEnemy = script.FindLowestEnemy(script.enemiesInRange);
                            StartCoroutine(Attack(lowerEnemy, script));

                        }
                        else
                        {
                            script.FindNearestEnemy(lista);
                            script.GetWalkableTiles();
                            script.FindClosestTile();
                            StartCoroutine(StartMovement(enemy, script));
                            yield return new WaitForSecondsRealtime(1);

                            script.GetEnemies();
                            if (script.enemiesInRange.Count > 0)
                            {
                                Unit lowerEnemy = script.FindLowestEnemy(script.enemiesInRange);
                                StartCoroutine(Attack(lowerEnemy, script));

                            }
                        }
                        script.hasMoved = true;
                    }


                    break;
                case "Archer":

                    if (script.hasMoved == false)
                    {
                        script.GetEnemies(); //Compruebo si hay enemigos al alcance actualmente
                        if (script.enemiesInRange.Count > 0)
                        {
                            Unit lowerEnemy = script.FindLowestEnemy(script.enemiesInRange);
                            StartCoroutine(Attack(lowerEnemy, script));

                        }
                        else
                        {
                            script.FindNearestAllyWithTag(listaAliados, "Bat");
                            if (script.closestAlly != null) {
                                script.GetWalkableTiles();
                                script.AllyTiles(script.closestAlly);
                                script.FarestTile(script.walkableTiles);
                                StartCoroutine(StartMovement(enemy, script));
                                yield return new WaitForSecondsRealtime(1);

                                script.GetEnemies();
                                if (script.enemiesInRange.Count > 0)
                                {
                                    Unit lowerEnemy = script.FindLowestEnemy(script.enemiesInRange);
                                    StartCoroutine(Attack(lowerEnemy, script));

                                }
                            }else
                            {
                                script.FindNearestAllyWithTag(listaAliados, "Knight");
                                if (script.closestAlly != null)
                                {
                                    script.GetWalkableTiles();
                                    script.AllyTiles(script.closestAlly);
                                    script.FarestTile(script.walkableTiles);
                                    StartCoroutine(StartMovement(enemy, script));
                                    yield return new WaitForSecondsRealtime(1);

                                    script.GetEnemies();
                                    if (script.enemiesInRange.Count > 0)
                                    {
                                        Unit lowerEnemy = script.FindLowestEnemy(script.enemiesInRange);
                                        StartCoroutine(Attack(lowerEnemy, script));

                                    }
                                }
                                else
                                {
                                    script.FindNearestAllyWithTag(listaAliados, "King");
                                    script.GetWalkableTiles();
                                    script.AllyTiles(script.closestAlly);
                                    script.FarestTile(script.walkableTiles);
                                    StartCoroutine(StartMovement(enemy, script));
                                    yield return new WaitForSecondsRealtime(1);

                                    script.GetEnemies();
                                    if (script.enemiesInRange.Count > 0)
                                    {
                                        Unit lowerEnemy = script.FindLowestEnemy(script.enemiesInRange);
                                        StartCoroutine(Attack(lowerEnemy, script));

                                    }
                                }
                            }
                           
                        }
                        script.hasMoved = true;
                    }
                    


                    break;
                case "Bat":
                    if (script.hasMoved == false)
                    {
                        script.GetEnemies(); //Compruebo si hay enemigos al alcance actualmente

                        enemyVillages = 0;
                        foreach (Village village in listaVillages)
                        {
                            if (village.playerNumber == 1)
                            {
                                enemyVillages += 1;
                            }
                        }

                        if (enemyVillages > 0)
                        {
                            script.FindNearestVillage(listaVillages);
                        }

                        if (script.enemiesInRange.Count > 0)
                        {
                            Unit lowerEnemy = script.FindLowestEnemy(script.enemiesInRange);
                            StartCoroutine(Attack(lowerEnemy, script));
                            yield return new WaitForSecondsRealtime(1);

                        }
                        else if (script.villagesInRange.Count > 0)
                        {

                            StartCoroutine(AttackVillage(script.closestVillage, script));
                            yield return new WaitForSecondsRealtime(1);
                        }
                        else
                        {

                            enemyVillages = 0;
                            foreach (Village village in listaVillages)
                            {
                                if (village.playerNumber == 1)
                                {
                                    enemyVillages += 1;
                                }
                            }

                            script.GetWalkableTiles();
                            if (enemyVillages > 0)
                            {
                                script.FindClosestTile(script.closestVillage.transform.position);
                            }
                            else
                            {
                                script.FindNearestEnemy(lista);
                                script.FindClosestTile();
                            }
                            StartCoroutine(StartMovement(enemy, script));
                            yield return new WaitForSecondsRealtime(1);

                            script.GetEnemies(); //Compruebo si hay enemigos al alcance actualmente
                            if (enemyVillages > 0)
                            {
                                script.GetVillages();
                            }
                            if (script.enemiesInRange.Count > 0)
                            {
                                Unit lowerEnemy = script.FindLowestEnemy(script.enemiesInRange);

                                if (lowerEnemy != null)
                                {
                                    StartCoroutine(Attack(lowerEnemy, script));
                                    yield return new WaitForSecondsRealtime(1);
                                }

                            }
                            else if (script.villagesInRange.Count > 0)
                            {

                                script.FindLowestVillage(script.villagesInRange);
                                StartCoroutine(AttackVillage(script.closestVillage, script));
                                yield return new WaitForSecondsRealtime(1);
                                script.villagesInRange.Clear();
                            }


                        }
                        script.hasMoved = true;
                    }
                    

                    break;
                default:
                    break;
            }



        }

        movimientosRealizados = true;
       
    }

    public void UpdateHealthDisplay(ClosestEnemy script)
    {
        if (script.isKing)
        {
            script.displayedText.text = script.health.ToString();
        }
    }

    IEnumerator Attack(Unit enemy, ClosestEnemy script)
    {

        int enemyDamege = script.attackDamage - enemy.armor;
        int unitDamage = enemy.defenseDamage - script.armor;

        if (enemyDamege >= 1)
        {
            enemy.health -= enemyDamege;
            enemy.UpdateHealthDisplay();
            DamageIcon d = Instantiate(script.damageIcon, enemy.transform.position, Quaternion.identity);
            d.Setup(enemyDamege);
        }

        if (script.transform.tag == "Archer" && enemy.tag != "Archer")
        {
            if (Mathf.Abs(script.transform.position.x - enemy.transform.position.x) + Mathf.Abs(script.transform.position.y - enemy.transform.position.y) <= 1) // check is the enemy is near enough to attack
            {
                if (unitDamage >= 1)
                {
                    script.health -= unitDamage;
                    UpdateHealthDisplay(script);
                    DamageIcon d = Instantiate(script.damageIcon, script.transform.position, Quaternion.identity);
                    d.Setup(unitDamage);
                }
            }
        }
        else
        {
            if (unitDamage >= 1)
            {
                script.health -= unitDamage;
                UpdateHealthDisplay(script);
                DamageIcon d = Instantiate(script.damageIcon, script.transform.position, Quaternion.identity);
                d.Setup(unitDamage);
            }
        }

        if (enemy.health <= 0)
        {

            if (script.deathEffect != null)
            {
                Instantiate(script.deathEffect, enemy.transform.position, Quaternion.identity);
                camAnim.SetTrigger("shake");
            }

            if (enemy.isKing)
            {
                ShowVictoryPanel(enemy.playerNumber);
            }

            script.GetWalkableTiles(); // check for new walkable tiles (if enemy has died we can now walk on his tile)
            RemoveInfoPanel(enemy);
            enemy.isDead = true;
            enemy.gameObject.SetActive(false);
            // Destroy(enemy.gameObject);
            script.enemies = FindObjectsOfType<Unit>();
        }

        if (script.health <= 0)
        {

            if (script.deathEffect != null)
            {
                Instantiate(script.deathEffect, script.transform.position, Quaternion.identity);
                camAnim.SetTrigger("shake");
            }

            if (script.isKing)
            {
                ShowVictoryPanel(script.playerNumber);
            }

            ResetTiles(); // reset tiles when we die
            RemoveInfoPanel(script);
            script.isDead = true;
            script.gameObject.SetActive(false);
            //Destroy(script.gameObject);
        }

        UpdateInfoStats();

        yield return null;
    }

    IEnumerator AttackVillage(Village enemy, ClosestEnemy script)
    {

        int enemyDamege = script.attackDamage;

        if (enemyDamege >= 1)
        {
            enemy.health -= enemyDamege;
            DamageIcon d = Instantiate(script.damageIcon, enemy.transform.position, Quaternion.identity);
            d.Setup(enemyDamege);
        }

        if (enemy.health <= 0)
        {

            if (script.deathEffect != null)
            {
                Instantiate(script.deathEffect, enemy.transform.position, Quaternion.identity);
                camAnim.SetTrigger("shake");
            }

            script.GetWalkableTiles(); // check for new walkable tiles (if enemy has died we can now walk on his tile)
            enemy.isDead = true;
            enemy.gameObject.SetActive(false);
            //Destroy(enemy.gameObject);
        }

        UpdateInfoStats();

        yield return null;
    }

    IEnumerator StartMovement(GameObject enemy, ClosestEnemy script) {
        while (enemy.transform.position.x != script.closestTile.transform.position.x)
        { // first aligns him with the new tile's x pos
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, new Vector2(script.closestTile.transform.position.x, enemy.transform.position.y), script.moveSpeed * Time.deltaTime);
            yield return null;
        }
        while (enemy.transform.position.y != script.closestTile.transform.position.y) // then y
        {
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, new Vector2(enemy.transform.position.x, script.closestTile.transform.position.y), script.moveSpeed * Time.deltaTime);
            yield return null;
        }

        script.walkableTiles.Clear();
        //selectedUnit.ResetWeaponIcon();
        ResetWeaponIcon();
    }


    public void ResetWeaponIcon()
    {
        ClosestEnemy[] enemies = FindObjectsOfType<ClosestEnemy>();
        foreach (ClosestEnemy enemy in enemies)
        {
            enemy.weaponIcon.SetActive(false);
        }

        Village[] villages = FindObjectsOfType<Village>();
        foreach (Village village in villages)
        {
            village.weaponIcon.SetActive(false);
        }
    }

    // Sets panel active/inactive and moves it to the correct place
    public void UpdateInfoPanel(Unit unit) {

        if (unit.Equals(currentInfoUnit) == false)
        {
            unitInfoPanel.transform.position = (Vector2)unit.transform.position + unitInfoPanelShift;
            unitInfoPanel.SetActive(true);

            currentInfoUnit = unit;

            UpdateInfoStats();

        } else {
            unitInfoPanel.SetActive(false);
            currentInfoUnit = null;
        }

    }

    // Updates the stats of the infoPanel
    public void UpdateInfoStats() {
        if (currentInfoUnit != null)
        {
            attackDamageInfo.text = currentInfoUnit.attackDamage.ToString();
            defenseDamageInfo.text = currentInfoUnit.defenseDamage.ToString();
            armorInfo.text = currentInfoUnit.armor.ToString();
            heathInfo.text = currentInfoUnit.health.ToString();
        }
    }

    // Moves the udpate panel (if the panel is actived on a unit and that unit moves)
    public void MoveInfoPanel(Unit unit) {
        if (unit.Equals(currentInfoUnit))
        {
            unitInfoPanel.transform.position = (Vector2)unit.transform.position + unitInfoPanelShift;
        }
    }

    // Deactivate info panel (when a unit dies)
    public void RemoveInfoPanel(Unit unit) {
        if (unit.Equals(currentInfoUnit))
        {
            unitInfoPanel.SetActive(false);
			currentInfoUnit = null;
        }
    }

    public void RemoveInfoPanel(ClosestEnemy iaunit)
    {
        if (iaunit.Equals(currentInfoUnit))
        {
            unitInfoPanel.SetActive(false);
            currentInfoUnit = null;
        }
    }



    public void ResetTiles() {
        Tile[] tiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles)
        {
            tile.Reset();
        }
    }

    void EndTurn() {
		source.Play();
        camAnim.SetTrigger("shake");

        // deselects the selected unit when the turn ends
        if (selectedUnit != null) {
            selectedUnit.ResetWeaponIcon();
            selectedUnit.isSelected = false;
            selectedUnit = null;
        }

        ResetTiles();

        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units) {
            unit.hasAttacked = false;
            unit.hasMoved = false;
            unit.ResetWeaponIcon();
            if (unit.isDead) {
                Destroy(unit.gameObject);
            }
        }

        ClosestEnemy[] x = FindObjectsOfType<ClosestEnemy>();
        foreach (ClosestEnemy unit in x)
        {
            unit.hasMoved = false;
            ResetWeaponIcon();
            if (unit.isDead) {
                Destroy(unit.gameObject);
            }
        }

        Village[] y = FindObjectsOfType<Village>();
        foreach (Village unit in y) {
            if (unit.isDead) {
                Destroy(unit.gameObject);
            }
        }

        if (playerTurn == 1) { //esto se dejará
            playerIcon.sprite = playerTwoIcon;
            playerTurn = 2;
            /*Cambios*/

            enemies.Clear();
            aux = GameObject.Find("IAUnits").GetComponentsInChildren<ClosestEnemy>(); 
            for (int i = 0; i < aux.Length; i++) {
                enemies.Add(aux[i].gameObject);
            }

            /*Cambios*/
        }
        else if (playerTurn == 2) {
            playerIcon.sprite = playerOneIcon;
            playerTurn = 1;
        }

        GetGoldIncome(playerTurn);
        GetComponent<CharacterCreation>().CloseCharacterCreationMenus();
        createdUnit = null;
    }

    void GetGoldIncome(int playerTurn) {
        foreach (Village village in FindObjectsOfType<Village>())
        {
            if (village.playerNumber == playerTurn)
            {
                if (playerTurn == 1)
                {
                    player1Gold += village.goldPerTurn;
                }
                else
                {
                    player2Gold += village.goldPerTurn;
                }
            }
        }
        if (playerTurn == 1)
        {
            player1Gold += 20;
        }
        else
        {
            player2Gold += 20;
        }

        UpdateGoldText();
    }

    public void UpdateGoldText()
    {
        player1GoldText.text = player1Gold.ToString();
        player2GoldText.text = player2Gold.ToString();
    }

    // Victory UI

    public void ShowVictoryPanel(int playerNumber) {

        if (playerNumber == 1)
        {
            blueVictory.SetActive(true);
        } else if (playerNumber == 2) {
            darkVictory.SetActive(true);
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
