using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{

    public int goldPerTurn;
    public int playerNumber;
    public int cost;

    public int health;
    public bool isDead = false;

    public DamageIcon damageIcon;
    public GameObject deathEffect;
    public GameObject weaponIcon;

    public GM gm;



    private void OnMouseDown()
    {
        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        if (col != null)
        {
            Debug.Log(col);
            Village unit = col.GetComponent<Village>(); // double check that what we clicked on is a enemy
            if (unit != null)
            {
                Debug.Log(unit);
                gm.selectedVillage = unit;
                Village[] listaVillages = FindObjectsOfType<Village>();
                List<Village> listaDeVillages = new List<Village>();

                foreach (Village vill in listaVillages) {
                    listaDeVillages.Add(vill);
                }

                if (listaDeVillages.Contains(unit) && !gm.selectedUnit.hasAttacked)
                {
                    gm.selectedUnit.AttackVillage(unit);
                    gm.ResetWeaponIcon();
                }
            }
        }
    }

}
