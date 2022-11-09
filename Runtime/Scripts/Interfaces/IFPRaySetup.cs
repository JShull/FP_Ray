using System;
using UnityEngine;
using Unity.Mathematics;
namespace FuzzPhyte.Ray
{
    public interface IFPRaySetup 
    {
        SO_FPRaycaster FPRayInformation { get; set; }
        Transform RayOrigin { get; }
        float3 RayDirection { get; set; }
        FP_Raycaster Raycaster { get; set; }
    }
}

