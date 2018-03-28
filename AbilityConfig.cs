using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
namespace RPG.Personagem {
   
    public abstract class AbilityConfig : ScriptableObject {

        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] AnimationClip abilityAnimation;
        [SerializeField] GameObject particlePrefab = null;
        [SerializeField] AudioClip audioClip = null;

        protected AbilityBehavior behavior;
        
                        

        public abstract AbilityBehavior GetBehaviourComponent(GameObject objectToAttachTo);
        public void AttachAbilityTo(GameObject objectToAttachTo)
        {
            AbilityBehavior behaviorComponent = GetBehaviourComponent(objectToAttachTo);
            behaviorComponent.SetConfig(this);
            behavior = behaviorComponent;
        }
        public void Use(GameObject target)
        {

            behavior.Use(target);
        }

        public float GetEnergyCost()
        {
            return energyCost;
        }
        public AnimationClip GetAnimationClip()
        {
            return abilityAnimation;
        }
        public GameObject GetParticlePrefab()
        {
            return particlePrefab;
        }
        public AudioClip GetAudioClip()
        {
            return audioClip;
        }
    }

}
