using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "Combat System/Create a new attack")]
    public class AttackData : ScriptableObject
    {
        [field: SerializeField] public string AniName { get; private set; }
        [field: SerializeField] public AttackHitBox HitboxToUse { get; private set; }
        [field: SerializeField] public float ImpactStartTime { get; private set; }
        [field: SerializeField] public float ImpactEndTime { get; private set; }
    }
    public enum AttackHitBox {LeftHand, RightHand, LeftFoot, RightFoot, Sword}
}

