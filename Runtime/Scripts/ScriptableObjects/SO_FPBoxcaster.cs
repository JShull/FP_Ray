using System.Collections;
using System.Collections.Generic;
using FuzzPhyte.Ray;
using UnityEngine;
namespace FuzzPhyte.Ray
{
    [CreateAssetMenu(fileName = "BoxRaycaster", menuName = "ScriptableObjects/FuzzPhyte/Ray/Boxcaster", order = 2)]
    public class SO_FPBoxcaster : SO_FPRaycaster
    {
        [Space]
        [Header("Box Variables")]
        [Tooltip("The half extents of the box")]
        public Vector3 BoxExtents;
        [Tooltip("Angle of the box")]
        public float BoxAngle;
        [Tooltip("Vector for Box Angle Rotation")]
        public Vector3 BoxAngleRotation;
    }
}
