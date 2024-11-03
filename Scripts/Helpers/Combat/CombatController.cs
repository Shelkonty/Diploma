using Climbing;
using UnityEngine;

namespace Combat
{
    public class CombatController : MonoBehaviour
    {
        [HideInInspector] public InputCharacterController characterInput;
        private MeleeFighter meleeFighter;

        private void Awake()
        {
            meleeFighter = GetComponent<MeleeFighter>();
            characterInput = GetComponent<InputCharacterController>();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Attack"))
            {
                meleeFighter.TryToAttack();
            }
        }
    }

}