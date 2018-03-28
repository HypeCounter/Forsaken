using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Personagem
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(HealthSystem))]

    public class NpcAI : MonoBehaviour
    {
        [SerializeField] WayPointContainer patrolPath;
        [SerializeField] float waypointTolerence = 2f;
        [SerializeField] float wayPointDwell = 15f;
        [SerializeField] float talkingRadius = 3f;

        int nextWayPointIndex;
        float currentWeaponRange;
        float distanceToPlayer;
        PlayerMovement player = null;
        enum State { idle, patrolling, talking }
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

            if (distanceToPlayer > talkingRadius && state != State.patrolling)
            {
                StopAllCoroutines();                
                StartCoroutine(Patrol());
            }
            if (distanceToPlayer <= talkingRadius && state != State.talking)
            {
                StopAllCoroutines();              
                StartCoroutine(TalkingToPlayer());
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

        IEnumerator TalkingToPlayer()
        {
            state = State.talking;
            while (true)
            {
                character.transform.LookAt(player.transform.position);
                yield return new WaitForEndOfFrame();
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
        
    }
}
