using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Personagem {


    public class EnemySpawn :MonoBehaviour {


        [SerializeField] float EnemySpawnRadius = 10f;
        [SerializeField] GameObject enemySpawned;
        GameObject player = null;
        bool isSpawned = false;
        public Transform spawnPoint;

 

	// Use this for initialization
	void Start () {
            player = GameObject.FindGameObjectWithTag("Player");
            spawnPoint = this.transform;
        }
	
	// Update is called once per frame
	void Update () {
            float distanceToPlayertoSpawn = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayertoSpawn <= EnemySpawnRadius & isSpawned != true) {
                isSpawned = true;

                GenerateEnemy();
            }
        }
        public void GenerateEnemy()
        {
            Instantiate(enemySpawned,spawnPoint.transform);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black; 
            Gizmos.color = new Color(0f, 0f, 255, .5f);
            Gizmos.DrawWireSphere(transform.position, EnemySpawnRadius);
        }


    }
    
}


