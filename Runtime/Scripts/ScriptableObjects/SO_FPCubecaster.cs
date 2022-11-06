using System.Collections;
using System.Collections.Generic;
using FuzzPhyte.Ray;
using UnityEngine;

namespace FuzzPhyte.Ray
{
    [CreateAssetMenu(fileName = "CubeRaycaster", menuName = "ScriptableObjects/FuzzPhyte/Ray/Cubecaster", order = 3)]
    public class SO_FPCubecaster : SO_FPRaycaster
    {
        [Space]
        [Header("Cube Variables")]
        [Tooltip("The half extents of the box")]
        public Vector3 BoxExtents;
        [Tooltip("Angle of the box")]
        public Vector3 BoxAngle;
    }

}

