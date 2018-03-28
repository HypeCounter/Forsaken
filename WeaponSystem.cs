using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Personagem
{
    public class WeaponSystem : MonoBehaviour
    {

        [SerializeField] float baseDamage = 10f;
        [SerializeField] WeaponConfig currentWeaponConfig;
        [SerializeField] ParticleSystem criticalHitParticle = null;
        [SerializeField] float criticalHitMult = 2f;
        [SerializeField] ParticleSystem hitParticle = null;
        [Range(.1f, 2.0f)] [SerializeField] float criticalHitChance = 0.1f;
        GameObject weaponObject;
        GameObject target;
        Animator animator;
        Character character;
        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";
        const string IDLE = "Idle";
        float lastHitTime = 0f;
        Character currentHealthState;
      


        void Start()
        {
            character = GetComponent<Character>();
            animator = GetComponent<Animator>();
            PutWeaponInHand(currentWeaponConfig);         
            SetAttackAnimation();
            

        }

       
        void Update()
        {
            bool targetIsDead;
            bool targetIsOutOfRange;
            if(target == null)
            {
                targetIsDead = false;
                targetIsOutOfRange = false;
            }
            else
            {
                var targetHealth = target.GetComponent<HealthSystem>().healthAsPercentage;
                targetIsDead = targetHealth <= Mathf.Epsilon;
                var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                targetIsOutOfRange = distanceToTarget > currentWeaponConfig.GetMaxAttackRange();
            }
            float characterHealth = GetComponent<HealthSystem>().healthAsPercentage;
            bool characterIsDead = (characterHealth <= Mathf.Epsilon);
            if (characterIsDead || targetIsOutOfRange || targetIsDead)
            {
                StopAllCoroutines();
            }

        }

        public void PutWeaponInHand(WeaponConfig weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            Destroy(weaponObject);
            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);            
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
            
        }
        private void SetAttackAnimation()
        {
            if (!character.GetOverrideController())
            {
                Debug.Break();
                Debug.LogAssertion("Please Provide " + gameObject + "with a animator override controller");
            }
            else
            {
                var animatorOverrideController = character.GetOverrideController();
                animator.runtimeAnimatorController = animatorOverrideController;
                animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
                animatorOverrideController[IDLE] = currentWeaponConfig.GetIdleWeaponClip();
            }
        }

        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberofDominantHands = dominantHands.Length;
            Assert.AreNotEqual(numberofDominantHands, 0, "Sem mão dominante no jogador");
            Assert.IsFalse(numberofDominantHands > 1, "Maos dominantes multiplas");
            return dominantHands[0].gameObject;

        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;

            StartCoroutine(AttackTargetRepeatedly());
   
        }


        IEnumerator AttackTargetRepeatedly()
        {
            //determinar se esta vivo
            bool attackerStillAlive = GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
            bool targetStillAlive = target.GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
            if (attackerStillAlive)
            {
                transform.LookAt(target.transform);
            }
            while (attackerStillAlive && targetStillAlive)
            {
                var animationClip = currentWeaponConfig.GetAttackAnimClip();
                float animationClipTIme = animationClip.length / character.GetAnimSpeedMultiplier();
                float timeToWait = animationClipTIme + currentWeaponConfig.GetTimeBetweenAnimationCycles();
               // float weaponHitPeriod = currentWeaponConfig.GetTimeBetweenAnimationCycles();
               // float timeToWait = weaponHitPeriod * character.GetAnimSpeedMultiplier();
                bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;
                if(isTimeToHitAgain)
                {
                    AttackTargetOnce();
                    lastHitTime = Time.time;

                }
                yield return new WaitForSeconds(timeToWait);
            }                     
          
        }
        void AttackTargetOnce()
        {            
            
                transform.LookAt(target.transform);
                animator.SetTrigger(ATTACK_TRIGGER);
                float damageDelay = currentWeaponConfig.GetDamageDelay();
                SetAttackAnimation();
                StartCoroutine(DamageAfterDelay(damageDelay));         
            
                      
        }

        IEnumerator DamageAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());



        }
        public void StopAttacking()
        {
            animator.StopPlayback();
            StopAllCoroutines();
        }

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        
        private float CalculateDamage()
        {
            bool isCriticalHit = UnityEngine.Random.Range(0f, 2f) <= criticalHitChance;
            float damageBeforeCritical = baseDamage + currentWeaponConfig.GetWeaponDamage();

            if (isCriticalHit)
            {

                criticalHitParticle.Play();
                return damageBeforeCritical * criticalHitMult;

            }
            else
            {
                hitParticle.Play();
                return damageBeforeCritical;

            }
        }



    }
}
