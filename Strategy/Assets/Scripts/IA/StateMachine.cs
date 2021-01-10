using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public GM gmScript;
    public bool turnIA;
    // Start is called before the first frame update
    void Start()
    {
        turnIA = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gmScript.playerTurn == 1 && ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.B)))){
            turnIA = true;
        }
    }
}
