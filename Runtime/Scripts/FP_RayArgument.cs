using System;
using Unity.Mathematics;
namespace FuzzPhyte.Ray
{
    /// <summary>
    /// Argument class to help pass information around between observers and messages
    /// </summary>
    public class FP_RayArgument : EventArgs
    {
        public IFPRaySetup CastingItem;
        public float3 WorldOrigin;
        public float3 WorldEndPoint;
        public float3 Direction;
        public RaycastType RayType;
    }
}
