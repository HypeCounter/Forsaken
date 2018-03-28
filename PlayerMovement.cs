
using UnityEngine;
using RPG.CameraUI; //TODO considerar reescrever
using System.Collections;

namespace RPG.Personagem
{
    public class PlayerMovement : MonoBehaviour
    {
      
        SpecialAbilities abilities;
        Character myCharacter;
        DialogueSystem talkTo;
    
        WeaponSystem weaponSystem;    
  
    
        void Start()
        {
            myCharacter = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            RegisterforMouseEvent();
            weaponSystem = GetComponent<WeaponSystem>();


           
  
        }

        void RegisterforMouseEvent()
        {
            var cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverNPC += OnMouseOverNPC;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        void Update()
        {
    
           ScanForAbilityKeyDown();
           
        }

        void ScanForAbilityKeyDown()
        {
            for (int keyIndex = 1; keyIndex <= abilities.GetNumbersOfAbilities(); keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                    abilities.AttemptSpecialAbility(keyIndex);
            }
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                weaponSystem.StopAttacking();
                myCharacter.SetDestination(destination);
            }
        }


        bool IsTargetInRange(GameObject target)

        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
        }

        void OnMouseOverNPC (NpcAI npc)
        {

            if (Input.GetMouseButton(0) && IsTargetInRange(npc.gameObject))
            {
             
                talkTo = npc.GetComponent<DialogueSystem>();
                talkTo.Talk(npc.gameObject);

            }
            if (Input.GetMouseButton(0)&& !IsTargetInRange(npc.gameObject))
            {
                
                StartCoroutine(MoveAndTalk(npc));
            }

        }
            

        void OnMouseOverEnemy(EnemyAI enemy)
        {
            {
                
             
                if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
                {                    
                    weaponSystem.AttackTarget(enemy.gameObject);
                }
                else if (Input.GetMouseButton(0) && !IsTargetInRange(enemy.gameObject))
                {
                    StartCoroutine(MoveAnAttack(enemy));
                    //atacar
                }
                else if (Input.GetMouseButtonDown(1) && IsTargetInRange(enemy.gameObject))
                {
                    abilities.AttemptSpecialAbility(0, enemy.gameObject);
                }
                else if (Input.GetMouseButtonDown(1) && !IsTargetInRange(enemy.gameObject))
                {
                    StartCoroutine(MoveAnSmite(enemy));
                }
            }
        }
        IEnumerator MoveToTarget(GameObject target)
        {
            myCharacter.SetDestination(target.transform.position);
            while (!IsTargetInRange(target))
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }

        IEnumerator MoveToTalk(NpcAI npc)
        {
            myCharacter.SetDestination(npc.transform.position);
            while (!IsTargetInRange(npc.gameObject))
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }

        IEnumerator MoveAndTalk(NpcAI npc)
        {
            yield return StartCoroutine(MoveToTalk(npc));

        }

        IEnumerator MoveAnAttack(EnemyAI enemy)
        {
            yield return StartCoroutine(MoveToTarget(enemy.gameObject));
            weaponSystem.AttackTarget(enemy.gameObject);

        }

        IEnumerator MoveAnSmite(EnemyAI enemy)
        {
            yield return StartCoroutine(MoveToTarget(enemy.gameObject));
            abilities.AttemptSpecialAbility(0, enemy.gameObject);

        }





    }
}

