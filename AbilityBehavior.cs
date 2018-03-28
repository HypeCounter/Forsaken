using System.Collections;

using UnityEngine;

namespace RPG.Personagem
{

    public abstract class AbilityBehavior : MonoBehaviour
    {
        protected AbilityConfig config;
        const float PARTICLE_DELAY = 10f;
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";
        const string ATTACK_TRIGGER = "Attack";

        public abstract void Use(GameObject target = null);

        public void SetConfig(AbilityConfig configToSet)
        {
           config = configToSet;
        }

        protected void PlayParticleEffect()
        {
            var particlePrefab = config.GetParticlePrefab();
            var particleObject = Instantiate(particlePrefab, 
                transform.position, 
                particlePrefab.transform.rotation);
                particleObject.transform.parent = transform;
              particleObject.GetComponent<ParticleSystem>().Play();
            StartCoroutine(DestroyParticleWhenFinished(particleObject));
          

        }
        IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab)
        {
            while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(PARTICLE_DELAY);
            }
            Destroy(particlePrefab);
            yield return new WaitForEndOfFrame(); 
        }

        protected void PlaySpecialAbilityAudio()
        {
            var abilitySound = config.GetAudioClip();
            var audioSource = GetComponent<AudioSource>();
 
            audioSource.PlayOneShot(abilitySound);
        }
        protected void PlayAnimationClip()
        {
            var animatorOverrideController = GetComponent<Character>().GetOverrideController();
            var animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = config.GetAnimationClip();
            animator.SetTrigger(ATTACK_TRIGGER);

            
        }
    }

}
