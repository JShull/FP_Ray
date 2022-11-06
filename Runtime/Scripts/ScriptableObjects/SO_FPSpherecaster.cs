using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzPhyte.Ray
{
    [CreateAssetMenu(fileName = "SphereRaycaster", menuName = "ScriptableObjects/FuzzPhyte/Ray/Spherecaster", order = 4)]
    public class SO_FPSpherecaster : SO_FPRaycaster
    {
        [Space]
        [Header("Sphere Variables")]
        [Tooltip("The radius of the sphere")]
        public float SphereRadius;
    }
}
