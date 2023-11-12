using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Scripting;
using FuzzPhyte.Utility;
namespace FuzzPhyte.Ray
{
    /// <summary>
    /// This debug class is to help visualize the work of the RayCaster you're using.
    /// You should remove this class before deploying and/or building as it does double the overhead
    /// </summary>
    [RequireComponent(typeof(FP_UtilityDraw))]
    public class FP_RayDebug : MonoBehaviour
    {
        [Header("RayCaster Reference")]
        [Tooltip("This has to be a Mono derived class that uses the IFPRaySetup interface")]
        public Component RayData;
        [Space]
        [Header("Debug Color Settings")]
        public Color RayEnterColor;
        public Color RayStayColor;
        public Color RayExitColor;
        [Tooltip("Reference to a Draw Utility within FuzzPhyte.Utility")]
        public FP_UtilityDraw DrawUtil;
        protected IFPRaySetup _rayData;
        protected void Awake()
        {
            if (RayData == null)
            {
                Debug.LogError($"You need to reference a Mono class that is using the IFPRaySetup Interface");
            }
            else
            {
                _rayData = RayData as IFPRaySetup;
            }
            DrawUtil = this.GetComponent<FP_UtilityDraw>();
            Debug.LogWarning($"Name of RaySetup: {RayData.GetType().Name}");
        }
        
        public void OnEnable()
        {
            StartCoroutine(DelayOnEnable());
        }
        protected IEnumerator DelayOnEnable()
        {
            ///need to wait for the other setup functionality to have occurred...
            ///quick fix until I come up with my own time related functions for Unity
            yield return new WaitForEndOfFrame();

            _rayData.Raycaster.OnFPRayFireHit += OnRayStay;
            _rayData.Raycaster.OnFPRayEnterHit += OnRayEnter;
            _rayData.Raycaster.OnFPRayExit += OnRayExit;
        }
        public void OnDisable()
        {
            _rayData.Raycaster.OnFPRayFireHit -= OnRayStay;
            _rayData.Raycaster.OnFPRayEnterHit -= OnRayEnter;
            _rayData.Raycaster.OnFPRayExit -= OnRayExit;
        }
        
        public void OnRayEnter(object sender, FP_RayArgumentHit arg) {
            Debug.DrawLine(arg.WorldOrigin, arg.WorldEndPoint, RayEnterColor, 5f);
        }
        public void OnRayStay(object sender, FP_RayArgumentHit arg)
        {
            switch (_rayData.FPRayInformation)
            {
                case SO_FPRaycasterThreeD:
                case SO_FPRaycasterTwoD:
                    Debug.DrawLine(arg.WorldOrigin, arg.WorldEndPoint, RayStayColor, 1f);
                    break;
                case SO_FPCubecaster:
                    SO_FPCubecaster cube = _rayData.FPRayInformation as SO_FPCubecaster;
                    DrawUtil.DrawBox(arg.WorldEndPoint, Quaternion.Euler(cube.BoxAngle), cube.BoxExtents, RayStayColor, 1f);
                    break;
                case SO_FPBoxcaster:
                    SO_FPBoxcaster box = _rayData.FPRayInformation as SO_FPBoxcaster;
                    DrawUtil.DrawBox(arg.WorldEndPoint, Quaternion.Euler(box.BoxAngleRotation * box.BoxAngle), box.BoxExtents, RayStayColor, 1f);
                    break;
                case SO_FPSpherecaster:
                    SO_FPSpherecaster sphere = _rayData.FPRayInformation as SO_FPSpherecaster;
                    Vector4 rayFour = new Vector4(
                        arg.WorldEndPoint.x,
                        arg.WorldEndPoint.y,
                        arg.WorldEndPoint.z,
                        0);
                    DrawUtil.DrawSphere(rayFour, sphere.SphereRadius, RayStayColor, 1f);
                    break;
                case SO_FPCirclecaster:
                    SO_FPCirclecaster circle = _rayData.FPRayInformation as SO_FPCirclecaster;
                    Vector4 rayFourCircle = new Vector4(
                        arg.WorldEndPoint.x,
                        arg.WorldEndPoint.y,
                        arg.WorldEndPoint.z,
                        0);
                    DrawUtil.DrawCircle(rayFourCircle, new Vector3(1, 0, 0), circle.CircleRadius, RayStayColor, 1f);
                    break;
            }
        }
        public void OnRayExit(object sender, FP_RayArgumentHit arg)
        {
            Debug.DrawLine(arg.WorldOrigin, arg.WorldEndPoint, RayExitColor, 5f);
        }
        
    }
}
