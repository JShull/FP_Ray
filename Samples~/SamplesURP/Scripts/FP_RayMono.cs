using Unity.Mathematics;
using UnityEngine;

namespace FuzzPhyte.Ray.Examples
{
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
        public float3 RayDirection
        {
            get { return Vector3.Normalize(RaycastEndDir.position - RaycastOrigin.position); }
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
            Debug.LogWarning($"RAY Enter: {arg.HitObject.name}");
            _rayHit = arg;
        }
        public void OnRayStay(object sender, FP_RayArgumentHit arg)
        {
            Debug.LogWarning($"RAY Stay: {arg.HitObject.name}");
            _rayHit = arg;
        }
        public void OnRayExit(object sender, FP_RayArgumentHit arg)
        {
            Debug.LogWarning($"RAY Exit: {arg.HitObject.name}");
            _rayHit = arg;
        }
        #endregion
        
        public void Start()
        {
            
        }

        public void FixedUpdate()
        {
            _raycaster.FireRaycast();
        }

        
    }

}
