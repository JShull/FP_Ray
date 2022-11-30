using System;
using UnityEngine;
using Unity.Mathematics;
namespace FuzzPhyte.Ray
{
    
    public interface IFPRaySetup 
    {
        [SerializeField]
        SO_FPRaycaster FPRayInformation { get; set; }
        [SerializeField]
        Transform RayOrigin { get; }
        [SerializeField]
        float3 RayDirection { get; set; }
        [SerializeField]
        FP_Raycaster Raycaster { get; set; }
    }
}

