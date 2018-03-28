using System;
using UnityEngine;

using UnityEngine.AI;
using RPG.CameraUI; //TODO considerar reescrever


namespace RPG.Personagem
{
    [SelectionBase]

    public class Character : MonoBehaviour
    {
        [Header("Capsule Collider")]
        [SerializeField] Vector3 colliderCenter = new Vector3(0, 0.8f, 0);
        [SerializeField] float colliderRadius = 0.3f;
        [SerializeField] float colliderHeight = 1.6f;

        [Header("Animator")]
        [SerializeField] RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animatorOverideController;
        [SerializeField] Avatar characterAvatar;

        [Header("Movement")]
        [SerializeField] float moveSpeedMultiplier = .7f;
        [SerializeField] float animatioSpeedMutiplier = 1.5f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float moveThreshold = 1f;
        [SerializeField] [Range(.1f, 1f)] float animatorForwardCap = 1f;

        [Header("Audio")]
        [SerializeField] float audioSpetial = 1f;
        [SerializeField] float audioVolume = 1f;

        [Header("NavMeshAgent")]
        [SerializeField] float navMeshSteeringSpeed = 1f;
        [SerializeField] float navMeshStoppingDistance = 1.5f;

        float turnAmount;
        float forwardAmount;
        NavMeshAgent navMeshAgent;
        Animator myAnimator;
        Rigidbody myRigidBody;
        bool isAlive = true;


        bool isInDirectMode = false;

        void Awake()
        {
            AddRequiredComponents();
        }

        private void AddRequiredComponents()
        {
            var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = colliderCenter;
            capsuleCollider.height = colliderHeight;
            capsuleCollider.radius = colliderRadius;

            myRigidBody = gameObject.AddComponent<Rigidbody>();
            myRigidBody.constraints = RigidbodyConstraints.FreezeRotation;

            myAnimator = gameObject.AddComponent<Animator>();
            myAnimator.runtimeAnimatorController = animatorController;
            myAnimator.avatar = characterAvatar;            
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = audioSpetial;
            audioSource.volume = audioVolume;

            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.stoppingDistance = navMeshStoppingDistance;
            navMeshAgent.autoBraking = false;
            navMeshAgent.updatePosition = true;
            navMeshAgent.updateRotation = false;           
            navMeshAgent.speed = navMeshSteeringSpeed;  
        }
     
        void Update()
        {
            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && isAlive)
            {
                Move(navMeshAgent.desiredVelocity);
            }
            else
            {
                Move(Vector3.zero);
            }
        }

        public float GetAnimSpeedMultiplier()
        {
            return myAnimator.speed;
        }

        public void SetDestination(Vector3 worldPos)
        {
            navMeshAgent.destination = worldPos;
        }

        void Move(Vector3 movement)
        {
            SetForwardAndTurn(movement);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        void SetForwardAndTurn(Vector3 movement)
        {
            if (movement.magnitude > moveThreshold)
            {
                movement.Normalize();
            }

            var localMove = transform.InverseTransformDirection(movement);
            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            forwardAmount = localMove.z;
        }

        public AnimatorOverrideController GetOverrideController()
        {
            return animatorOverideController;
        }

        void UpdateAnimator()
        {
            // update the animator parameters
            myAnimator.SetFloat("Forward", forwardAmount * animatorForwardCap, 0.1f, Time.deltaTime);
            myAnimator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            myAnimator.speed = animatioSpeedMutiplier;
        }
        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }
    
        void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (Time.deltaTime > 0)
            {
                Vector3 velocity = (myAnimator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                velocity.y = myRigidBody.velocity.y;
                myRigidBody.velocity = velocity;
            }
        }
        public void Kill()
        {
            isAlive = false;
        }
      
    }
}

