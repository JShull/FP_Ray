using Unity.Mathematics;
using UnityEngine;

namespace FuzzPhyte.Ray.Examples
{
    /// <summary>
    /// Example of how to utilize the IFPRaySetup Interface
    /// There's a lot of boiler plate code for how this works and for the most part this example
    /// can be reused and/or modified to your needs
    /// </summary>
    public class FP_RayMono : MonoBehaviour, IFPRaySetup
    {
        #region Setup Variables
        [Header("Raycaster!")]
        public string RaycastInformation;
        public SO_FPRaycaster RayData;
        public Transform RaycastOrigin;
        public Transform RaycastEndDir;
        #endregion
        #region Interface Requirements
        public SO_FPRaycaster FPRayInformation
        {
            get { return RayData; }
            set { RayData = value; }
        }
        public Transform RayOrigin {
            get { return RaycastOrigin; }
            set { RaycastOrigin = value; }
        }
        //quick fix for cases in which we end up destroying endpoints
        public float3 RayDirection
        {
            get { 
                if(RaycastEndDir==null||RaycastOrigin==null)
                {
                    return Vector3.zero;
                }
                return Vector3.Normalize(RaycastEndDir.position - RaycastOrigin.position); }
            set { RayDirection = value; }
        }
        private FP_Raycaster _raycaster;

        private FP_RayArgumentHit _rayHit;

        public FP_Raycaster Raycaster { get { return _raycaster; } set { _raycaster = value; } }
        
        public void SetupRaycaster()
        {
            _raycaster = new FP_Raycaster(this);
        }
        #endregion
        private void Awake()
        {
            SetupRaycaster();
        }
        public void OnEnable()
        {
            _raycaster.OnFPRayFireHit += OnRayStay;
            _raycaster.OnFPRayEnterHit += OnRayEnter;
            _raycaster.OnFPRayExit += OnRayExit;
            _raycaster.ActivateRaycaster();
        }
        public void OnDisable()
        {
            _raycaster.OnFPRayFireHit -= OnRayStay;
            _raycaster.OnFPRayEnterHit -= OnRayEnter;
            _raycaster.OnFPRayExit -= OnRayExit;
            _raycaster.DeactivateRaycaster();
        }
        #region Callback Functions for Raycast Delegates
        public void OnRayEnter(object sender, FP_RayArgumentHit arg)
        {
            if (arg.HitObject != null)
            {
                Debug.LogWarning($"RAY Enter: {arg.HitObject.name}");
            }
            
            _rayHit = arg;
        }
        public void OnRayStay(object sender, FP_RayArgumentHit arg)
        {
            if (arg.HitObject != null)
            {
                Debug.LogWarning($"RAY Stay: {arg.HitObject.name}");
            }
            
            _rayHit = arg;
        }
        public void OnRayExit(object sender, FP_RayArgumentHit arg)
        {
            if (arg.HitObject != null)
            {
                Debug.LogWarning($"RAY Exit: {arg.HitObject.name}");
            }
            
            _rayHit = arg;
        }
        #endregion
        /// <summary>
        /// Using FixedUpdate to send the Physics Raycast
        /// </summary>
        public void FixedUpdate()
        {
            _raycaster.FireRaycast();
        }

        
    }

}
