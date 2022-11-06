using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzPhyte.Ray
{
    [CreateAssetMenu(fileName = "CircleRaycaster", menuName = "ScriptableObjects/FuzzPhyte/Ray/Circlecaster", order = 5)]
    public class SO_FPCirclecaster : SO_FPRaycaster
    {
        [Space]
        [Header("Sphere Variables")]
        [Tooltip("The radius of the sphere")]
        public float CircleRadius;
        [Tooltip("Which Axis for Rendering")]
        public Vector3 CircleAxis;
    }
}
