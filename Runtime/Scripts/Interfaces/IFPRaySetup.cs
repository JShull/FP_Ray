using System;
using UnityEngine;
using Unity.Mathematics;
namespace FuzzPhyte.Ray
{
    
    public interface IFPRaySetup 
    {
        public SO_FPRaycaster FPRayInformation { get; set; }
        public Transform RayOrigin { get; }
        public float3 RayDirection { get; set; }
        public FP_Raycaster Raycaster { get; set; }
    }
}

