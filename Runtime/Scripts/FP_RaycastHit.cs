using UnityEngine;

namespace FuzzPhyte.Ray
{
    /// <summary>
    /// This is a class that represents and can hold both a 3D and 2D hit return from UnityEngine
    /// </summary>
    public class FP_RaycastHit
    {
        public RaycastHit2D TwoDHit;
        public RaycastHit ThreeDHit;
        public RaycastDimension TheDim;
        public RaycastType RayType;
        public FP_RaycastHit(RaycastHit full3D, RaycastType type)
        {
            TheDim = RaycastDimension.RaycastThreeD;
            ThreeDHit = full3D;
            RayType = type;
        }
        public FP_RaycastHit(RaycastHit2D full2D, RaycastType type)
        {
            TheDim = RaycastDimension.RaycastTwoD;
            TwoDHit = full2D;
            RayType = type;
        }
    }

}
