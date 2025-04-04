namespace FuzzPhyte.Ray
{
    using FuzzPhyte.Utility;
    using System;
    using UnityEngine;
    //Main root Scriptable Object that all others derive off of - shouldn't create this one directly
    [Serializable]
    public class SO_FPRaycaster : FP_Data
    {
        [Tooltip("The layer we care about to return/hit on")]
        public LayerMask LayerToInteract;
        [Tooltip("Raycast length")]
        public float RaycastLength;
        [Tooltip("Raycast Penetration through multiple objects")]
        public bool RayPenetration;
        [Tooltip("0 or 1 for 2D")]
        public Vector3 AxisToConvert;

        public virtual Vector2 Vector3Vector2(Vector3 input) 
        {
            float a = 0;
            float b = 0;
            if (AxisToConvert.x != 0)
            {
                //x
                a = AxisToConvert.x * input.x;
                if (AxisToConvert.y != 0)
                {
                    //standard
                    b = AxisToConvert.y * input.y;
                    return new Vector2(a, b);
                }
                else
                {
                    b = AxisToConvert.z * input.z;
                    return new Vector2(a, b);
                }
            }
            else
            {
                //must be y and z
                a = AxisToConvert.y * input.y;
                b = AxisToConvert.z * input.z;
                return new Vector2(a, b);
            } 
        }
        public virtual Vector3 Vector2Vector3(Vector2 input)
        {
            float a = 0;
            float b = 0;
            if (AxisToConvert.x != 0)
            {
                //x
                a = AxisToConvert.x * input.x;
                if (AxisToConvert.y != 0)
                {
                    //standard
                    b = AxisToConvert.y * input.y;
                    return new Vector3(a, b, 0);
                }
                else
                {
                    b = AxisToConvert.z * input.y;
                    return new Vector3(a, 0, b);
                }
            }
            else
            {
                //must be y and z
                a = AxisToConvert.y * input.x;
                b = AxisToConvert.z * input.y;
                return new Vector3(0,a ,b);
            }
        }
    }
}
