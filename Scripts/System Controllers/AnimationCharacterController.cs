using UnityEngine;

namespace Climbing
{
    public class AnimationCharacterController : MonoBehaviour
    {
        [HideInInspector] public ThirdPersonController controller;
        [HideInInspector] public Animator animator;
        private Vector3 animVelocity;

        public SwitchCameras switchCameras;
        public AnimatorStateInfo animState;

        private MatchTargetWeightMask matchTargetWeightMask = new MatchTargetWeightMask(Vector3.one, 0);

        void Awake()
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component is missing on this GameObject.");
                return;
            }

            controller = GetComponent<ThirdPersonController>();
            if (controller == null)
            {
                Debug.LogError("ThirdPersonController component is missing on this GameObject.");
                return;
            }

            switchCameras = Camera.main.GetComponent<SwitchCameras>();
        }

        void Update()
        {
            if (animator == null) return;

            animator.SetFloat("Velocity", animVelocity.magnitude);

            animState = animator.GetCurrentAnimatorStateInfo(0);

            if (animState.IsTag("Root") || animState.IsTag("Drop"))
            {
                animator.applyRootMotion = true;
            }
            else
            {
                animator.applyRootMotion = false;
            }
        }

        public void SetAnimVelocity(Vector3 value) { animVelocity = value; animVelocity.y = 0; }
        public Vector3 GetAnimVelocity() { return animVelocity; }

        public bool RootMotion() { return animator.applyRootMotion; }

        public void Fall()
        {
            if (animator == null) return;

            animator.SetBool("Jump", false);
            animator.SetBool("onAir", true);
            animator.SetBool("Land", false);
            controller.characterMovement.DisableFeetIK();
        }

        public void Land()
        {
            if (animator == null) return;

            animator.SetBool("Jump", false);
            animator.SetBool("onAir", false);
            animator.SetBool("Land", true);
            controller.characterMovement.EnableFeetIK();
        }

        public void HangLedge(ClimbController.ClimbState state)
        {
            if (animator == null) return;

            if (state == ClimbController.ClimbState.BHanging)
                animator.CrossFade("Idle To Braced Hang", 0.2f);
            else if (state == ClimbController.ClimbState.FHanging)
                animator.CrossFade("Idle To Freehang", 0.2f);

            animator.SetBool("Land", false);
            animator.SetInteger("Climb State", (int)state);
            animator.SetBool("Hanging", true);
        }

        public void LedgeToLedge(ClimbController.ClimbState state, Vector3 direction, ref float startTime, ref float endTime)
        {
            if (animator == null) return;

            if (state == ClimbController.ClimbState.BHanging)
            {
                if (direction.x == -1 && direction.y == 0 ||
                    direction.x == -1 && direction.y == 1 ||
                    direction.x == -1 && direction.y == -1)
                {
                    animator.CrossFade("Braced Hang Hop Left", 0.2f);
                    startTime = 0.2f;
                    endTime = 0.49f;
                }
                else if (direction.x == 1 && direction.y == 0 ||
                        direction.x == 1 && direction.y == -1 ||
                        direction.x == 1 && direction.y == 1)
                {
                    animator.CrossFade("Braced Hang Hop Right", 0.2f);
                    startTime = 0.2f;
                    endTime = 0.49f;
                }
                else if (direction.x == 0 && direction.y == 1)
                {
                    animator.CrossFade("Braced Hang Hop Up", 0.2f);
                    startTime = 0.3f;
                    endTime = 0.48f;
                }
                else if (direction.x == 0 && direction.y == -1)
                {

                    animator.CrossFade("Braced Hang Hop Down", 0.2f);
                    startTime = 0.3f;
                    endTime = 0.7f;
                }
            }

            animator.SetInteger("Climb State", (int)state);
            animator.SetBool("Hanging", true);
        }

        public void BracedClimb()
        {
            if (animator == null) return;
            animator.CrossFade("Braced Hang To Crouch", 0.2f);
        }

        public void FreeClimb()
        {
            if (animator == null) return;
            animator.CrossFade("Freehang Climb", 0.2f);
        }

        public void DropToFree(int state)
        {
            if (animator == null) return;
            animator.CrossFade("Drop To Freehang", 0.1f);
            animator.SetInteger("Climb State", (int)state);
            animator.SetBool("Hanging", true);
            SetAnimVelocity(Vector3.forward);
        }

        public void DropToBraced(int state)
        {
            if (animator == null) return;
            animator.CrossFade("Drop To Bracedhang", 0.1f);
            animator.SetInteger("Climb State", (int)state);
            animator.SetBool("Hanging", true);
            SetAnimVelocity(Vector3.forward);
        }

        public void DropLedge(int state)
        {
            if (animator == null) return;
            animator.SetBool("Hanging", false);
            animator.SetInteger("Climb State", state);
        }

        public void HangMovement(float value, int climbstate)
        {
            if (animator == null) return;
            animator.SetFloat("Horizontal", Mathf.Lerp(animator.GetFloat("Horizontal"), value, Time.deltaTime * 15));
            animator.SetInteger("Climb State", climbstate);
        }

        public void JumpPrediction(bool state)
        {
            if (animator == null) return;
            controller.characterAnimation.animator.CrossFade("Predicted Jump", 0.1f);
            animator.SetBool("Crouch", state);
        }

        public void EnableIKSolver()
        {
            controller.characterMovement.EnableFeetIK();
        }

        public void EnableController()
        {
            controller.EnableController();
        }

        public void SetMatchTarget(AvatarTarget avatarTarget, Vector3 targetPos, Quaternion targetRot, Vector3 offset, float startnormalizedTime, float targetNormalizedTime)
        {
            if (animator == null || animator.isMatchingTarget) return;

            float normalizeTime = Mathf.Repeat(animState.normalizedTime, 1f);

            if (normalizeTime > targetNormalizedTime) return;

            animator.SetTarget(avatarTarget, targetNormalizedTime);
            animator.MatchTarget(targetPos + offset, targetRot, avatarTarget, matchTargetWeightMask, startnormalizedTime, targetNormalizedTime, true);
        }
    }
}
