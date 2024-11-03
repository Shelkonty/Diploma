using Climbing;
using UnityEngine;

public class ComponentInitializer : MonoBehaviour
{
    private void Awake()
    {
        // Ensure that required components are assigned before they are used
        AnimationCharacterController animationCharacterController = GetComponent<AnimationCharacterController>();
        if (animationCharacterController != null)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component is missing on this GameObject.");
                return;
            }
            animationCharacterController.animator = animator;

            ThirdPersonController thirdPersonController = GetComponent<ThirdPersonController>();
            if (thirdPersonController == null)
            {
                Debug.LogError("ThirdPersonController component is missing on this GameObject.");
                return;
            }
            animationCharacterController.controller = thirdPersonController;
        }
        else
        {
            Debug.LogError("AnimationCharacterController component is missing on this GameObject.");
        }
    }
}