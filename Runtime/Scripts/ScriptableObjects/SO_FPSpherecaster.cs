namespace FuzzPhyte.Ray
{
    using UnityEngine;
    [CreateAssetMenu(fileName = "SphereRaycaster", menuName = "FuzzPhyte/Ray/Spherecaster", order = 4)]
    public class SO_FPSpherecaster : SO_FPRaycaster
    {
        [Space]
        [Header("Sphere Variables")]
        [Tooltip("The radius of the sphere")]
        public float SphereRadius;
    }
}
