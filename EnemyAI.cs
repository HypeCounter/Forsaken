using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Personagem
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(HealthSystem))]
    
    public class EnemyAI : MonoBehaviour
    { 
        
        [SerializeField] float chaseRadius = 6f;
        [SerializeField] WayPointContainer patrolPath;
        [SerializeField] float waypointTolerence = 2f;
        [SerializeField] float wayPointDwell = 1.5f;
        bool isAttacking = false;
        int nextWayPointIndex;
        float currentWeaponRange;
        float distanceToPlayer;
        PlayerMovement player = null;
        enum State { idle, patrolling, attacking, chasing}
        State state = State.idle;
        Character character;

        void Start()
        {
            player = FindObjectOfType<PlayerMovement>();
            character = GetComponent<Character>();
        }

        void Update()
        {
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
                  

            if (distanceToPlayer>chaseRadius && state != State.patrolling)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(Patrol());
            }
            if (distanceToPlayer<= chaseRadius && state != State.chasing)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(ChasePlayer());
            }
            if (distanceToPlayer <= currentWeaponRange && state != State.attacking)
                {
                StopAllCoroutines();
                

                weaponSystem.AttackTarget(player.gameObject);
            }
        }  
        
        IEnumerator ChasePlayer()
        {
            state = State.chasing;
            while (distanceToPlayer >= currentWeaponRange)
            {
                character.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }
        IEnumerator Patrol()
        {
            state = State.patrolling;
            while (true)
            {
                Vector3 nextWayPointPos = patrolPath.transform.GetChild(nextWayPointIndex).position;
                character.SetDestination(nextWayPointPos);
                CycleWaypointWhenClose(nextWayPointPos);
                yield return new WaitForSeconds(wayPointDwell);
            }
        }
    

        private void CycleWaypointWhenClose(Vector3 nextWayPointPos)
        {
            if (Vector3.Distance(transform.position, nextWayPointPos) <= waypointTolerence)
            {
                nextWayPointIndex = (nextWayPointIndex + 1) % patrolPath.transform.childCount;
          
                new WaitForSeconds(1f);
            }
        }

        void OnDrawGizmos()
        {
            //		Gizmos.color = Color.black;
            //		Gizmos.DrawLine (transform.position, clickPoint);
            //		Gizmos.DrawSphere(currentDestination, 0;15f);
            //		Gizmos.DrawSphere(clickPoint, 0.1f);
            //
            		Gizmos.color = new Color(255f, 0f,0, .5f);
            		Gizmos.DrawWireSphere(transform.position, currentWeaponRange);
            //
            		Gizmos.color = new Color(0f, 0f, 255, .5f);
            		Gizmos.DrawWireSphere(transform.position, chaseRadius);
            //	
        }
    }
}

