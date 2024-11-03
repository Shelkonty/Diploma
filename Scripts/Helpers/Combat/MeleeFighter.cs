using System.Collections;
using System.Collections.Generic;
using Climbing;
using UnityEngine;

public enum AttackState { Idle, WindUp, Impact, Cooldown }

namespace Combat
{
    [RequireComponent(typeof(AnimationCharacterController))]
    public class MeleeFighter : MonoBehaviour
    {
        [field:SerializeField] public float Health { get; private set; } = 25f;
        [SerializeField] private GameObject sword;
        [SerializeField] public List<AttackData> attacks = new List<AttackData>();
        private SphereCollider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;
        private BoxCollider swordCollider;
        [HideInInspector] public AnimationCharacterController characterAnimation;
        public AnimatorStateInfo animState;
        public bool InAction { get; private set; }
        private ThirdPersonController _controller;
        public AttackState attackState { get; private set; }
        private AttackData attackData;
        private bool doCombo;
        private int comboCount;

        private void Awake()
        {
            characterAnimation = GetComponent<AnimationCharacterController>();
            _controller = GetComponent<ThirdPersonController>();
        }

        private void Start()
        {
            if (sword != null)
            {
                swordCollider = sword.GetComponent<BoxCollider>();
                leftHandCollider = characterAnimation.animator.GetBoneTransform(HumanBodyBones.LeftHand)
                    .GetComponent<SphereCollider>();
                rightHandCollider = characterAnimation.animator.GetBoneTransform(HumanBodyBones.RightHand)
                    .GetComponent<SphereCollider>();
                leftFootCollider = characterAnimation.animator.GetBoneTransform(HumanBodyBones.LeftFoot)
                    .GetComponent<SphereCollider>();
                rightFootCollider = characterAnimation.animator.GetBoneTransform(HumanBodyBones.RightFoot)
                    .GetComponent<SphereCollider>();
                
                DisableAllColliders();
            }
        }

        public void TryToAttack()
        {
            if (!InAction && attacks != null && attacks.Count > 0)
            {
                TakeDamage(20f);
                if (Health > 0)
                {
                    StartCoroutine(PlayerHitReaction());
                }
                StartCoroutine(Attack());
            }
            else if (attackState == AttackState.Impact || attackState == AttackState.Cooldown)
            {
                doCombo = true;
            }
        }

        void PlayerDeathAnimation(MeleeFighter attacker)
        {
            
        }

        void TakeDamage(float damage)
        {
            Health = Mathf.Clamp(Health - damage, 0, Health);
        }

        IEnumerator Attack()
        {

            if (comboCount >= attacks.Count)
            {
                comboCount = 0;
            }

            InAction = true;
            attackState = AttackState.WindUp;

            Debug.Log($"Performing attack: {attacks[comboCount].AniName}");

            characterAnimation.animator.CrossFade(attacks[comboCount].AniName, 0.2f);
            yield return null;

            var animState = _controller.characterAnimation.animator.GetNextAnimatorStateInfo(1);

            float timer = 0f;
            while (timer <= animState.length)
            {
                timer += Time.deltaTime;
                float normalizedTime = timer / animState.length;

                if (attackState == AttackState.WindUp)
                {
                    if (normalizedTime >= attacks[comboCount].ImpactStartTime)
                    {
                        attackState = AttackState.Impact;
                        EnableHitBox(attacks[comboCount]);
                    }
                }
                else if (attackState == AttackState.Impact)
                {
                    if (normalizedTime >= attacks[comboCount].ImpactEndTime)
                    {
                        attackState = AttackState.Cooldown;
                        DisableAllColliders();
                    }
                }
                else if (attackState == AttackState.Cooldown)
                {
                    if (doCombo)
                    {
                        doCombo = false;
                        comboCount = (comboCount + 1) % attacks.Count;
                        Debug.Log($"Combo attack: {attacks[comboCount].AniName}");
                        StartCoroutine(Attack());
                        yield break;
                    }
                }

                yield return null;
            }

            attackState = AttackState.Idle;
            comboCount = 0;
            InAction = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hitbox"))
            {
                Debug.Log("character was hit by me");
                StartCoroutine(PlayerHitReaction());
            }
        }

        IEnumerator PlayerHitReaction()
        {
            if (characterAnimation == null || _controller == null)
            {
                Debug.LogError("Required components are not initialized.");
                yield break;
            }

            InAction = true;
            
            characterAnimation.animator.CrossFade("SwordImpact3", 0.2f);
            yield return null;

            var animState = _controller.characterAnimation.animator.GetNextAnimatorStateInfo(1);
            yield return new WaitForSeconds(animState.length);

            InAction = false;
        }

        void EnableHitBox(AttackData attackData)
        {
            switch (attackData.HitboxToUse)
            {
                case AttackHitBox.LeftHand:
                    leftHandCollider.enabled = true;
                    break;
                case AttackHitBox.RightHand:
                    rightHandCollider.enabled = true;
                    break;
                case AttackHitBox.LeftFoot:
                    leftFootCollider.enabled = true;
                    break;
                case AttackHitBox.RightFoot:
                    rightFootCollider.enabled = true;
                    break;
                case AttackHitBox.Sword:
                    swordCollider.enabled = true;
                    break;
                default:
                    break;
            }
        }
        
        void DisableAllColliders()
        {
            if (swordCollider != null)
            {
                swordCollider.enabled = false;
            }

            if (leftHandCollider != null)
            {
                leftHandCollider.enabled = false;    
            }

            if (rightHandCollider != null)
            {
                rightHandCollider.enabled = false;    
            }

            if (leftFootCollider != null)
            {
                leftFootCollider.enabled = false;    
            }

            if (rightFootCollider != null)
            {
                rightFootCollider.enabled = false;
            }
            
        }
    }
}
