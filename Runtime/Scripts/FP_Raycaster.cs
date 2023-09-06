using System;
using UnityEngine;
using Unity.Mathematics;

namespace FuzzPhyte.Ray
{
    #region Enumerations for all FuzzPhyte.Ray classes
    public enum RaycastDimension
    {
        RaycastTwoD = 0,
        RaycastThreeD = 1,
    }
    public enum RaycastType
    {
        Line = 0,
        Box = 1,
        Cube = 2,
        Circle = 3,
        Sphere = 4,
        Spline = 5,
    }
    #endregion
    
    public class FP_Raycaster: IFPRaycaster
    {
        //delegate setup with instance of delegate
        private delegate FP_RaycastHit RayDataReturn(ref bool hit);
        private RayDataReturn RaydataHit;
        //Interface instance that is passed to us from our Constructor
        private IFPRaySetup _raySetup;
        //cached variable to keep track of different outcomes of instantly going out/into others by tracking first hit and storing that
        private bool _rayFirstHit;
        //cached variable to keep track of if we are active or not
        private bool _rayActive;
        public bool RayActive
        {
            get { return _rayActive; }
        }
        private FP_RayArgumentHit _currentHitItem;
        public FP_RayArgumentHit ReturnCurrentHitItem
        {
            get
            {
                return _currentHitItem;
            }
        }
        public FP_Raycaster(IFPRaySetup rayInformation)
        {
            _raySetup = rayInformation;
        }
        
        event EventHandler<FP_RayArgumentHit> PreRayFire;
        event EventHandler<FP_RayArgumentHit> PreRayEnter;
        event EventHandler<FP_RayArgumentHit> PreRayExit;

        /// <summary>
        /// Different event handlers tied to the overall raycast states
        /// We have an activation and a deactivation event handler: pretty straight forward
        /// Enter/Exit are tied to the initial hit/exit of the raycast/collider information
        /// FireHit is the continous process of hitting
        /// </summary>
        public event EventHandler<FP_RayArgumentHit> OnFPRayFireHit { add { PreRayFire += value; }remove { PreRayFire -= value; } }
        public event EventHandler<FP_RayArgumentHit> OnFPRayEnterHit { add { PreRayEnter += value; } remove { PreRayEnter -= value; } }
        public event EventHandler<FP_RayArgumentHit> OnFPRayExit { add { PreRayExit += value; } remove { PreRayExit -= value; } }
        public event EventHandler<FP_RayArgument> OnFPRayActivate;
        public event EventHandler<FP_RayArgument> OnFPRayDeactivate;

        public virtual void ActivateRaycaster()
        {
            RaydataHit += RaycastThreeD;
            RaydataHit += RaycastTwoD;
            RaydataHit += RaycastCube;
            RaydataHit += RaycastBox;
            RaydataHit += RaycastSphere;
            RaydataHit += RaycastCircle;
            FP_RayArgument arg = new FP_RayArgument()
            {
                //HitObject = hit.transform,
                //RayType = _raySetup.FPRayInformation.RayType,
                WorldEndPoint = _raySetup.RayDirection*_raySetup.FPRayInformation.RaycastLength,
                WorldOrigin = _raySetup.RayOrigin.position
            };
            OnFPRayActivate?.Invoke(this, arg);
            _rayActive = true;
        }
        public virtual void DeactivateRaycaster()
        {
            RaydataHit -= RaycastThreeD;
            RaydataHit -= RaycastTwoD;
            RaydataHit -= RaycastCube;
            RaydataHit -= RaycastBox;
            RaydataHit -= RaycastSphere;
            RaydataHit -= RaycastCircle;
            
            if (_raySetup.RayOrigin != null)
            {
                FP_RayArgument arg = new FP_RayArgument()
                {
                    //HitObject = hit.transform,
                    //RayType = _raySetup.FPRayInformation.RayType,
                    WorldEndPoint = _raySetup.RayDirection * _raySetup.FPRayInformation.RaycastLength,
                    WorldOrigin = _raySetup.RayOrigin.position
                };
                OnFPRayDeactivate?.Invoke(this, arg);
            }
            else
            {
                FP_RayArgument arg = new FP_RayArgument()
                {
                    //HitObject = hit.transform,
                    //RayType = _raySetup.FPRayInformation.RayType,
                    WorldEndPoint = _raySetup.RayDirection * _raySetup.FPRayInformation.RaycastLength,
                    //WorldOrigin = _raySetup.RayOrigin.position
                };
                OnFPRayDeactivate?.Invoke(this, arg);
            }
            _rayActive = false;
        }
        /// <summary>
        /// Pausing the raycaster but not clearing it
        /// </summary>
        public virtual void PauseRaycaster()
        {
            ///if you had a current hit we retain that information... just FYI as we are only setting the flag false here
            _rayActive = false;
        }
        /// <summary>
        /// Only use this if you want to hard reset everything without an exit event, default is to clear and not play
        /// </summary>
        /// <param name="activateAfterReset">if you want to kick the raycaster back into a play/true state</param>
        public virtual void ResetRaycaster(bool activateAfterReset=false)
        {
            _rayActive = false;
            _currentHitItem = null;
            _rayFirstHit = false;
            _rayActive = activateAfterReset;
        }
        /// <summary>
        /// Reactivate the raycast - and we still track the last hit just FYI
        /// </summary>
        public virtual void PlayRaycaster()
        {
            if (_currentHitItem != null)
            {
                //exit it
                PreRayExit?.Invoke(this, _currentHitItem);
                _currentHitItem = null;
                _rayFirstHit = false;
            }
            _rayActive = true;
        }   
        /// <summary>
        /// Generic raycast with information we already have on file
        /// </summary>
        public virtual void FireRaycast()
        {
            if (!_rayActive)
            {
                return;
            }
            bool hitSuccess = false;
            
            switch (_raySetup.FPRayInformation)
            {
                case SO_FPRaycasterThreeD:
                    //ThreeD
                    hitSuccess = CastRay(PreRayEnter, PreRayFire, RaycastThreeD);
                    break;
                case SO_FPRaycasterTwoD:
                    //TwoD
                    hitSuccess = CastRay(PreRayEnter, PreRayFire, RaycastTwoD);
                    break;
                case SO_FPCubecaster:
                    //ThreeD Box
                    hitSuccess = CastRay(PreRayEnter, PreRayFire, RaycastCube);
                    break;
                case SO_FPBoxcaster:
                    hitSuccess = CastRay(PreRayEnter, PreRayFire, RaycastBox);
                    break;
                case SO_FPSpherecaster:
                    hitSuccess = CastRay(PreRayEnter, PreRayFire, RaycastSphere);
                    break;
                case SO_FPCirclecaster:
                    hitSuccess = CastRay(PreRayEnter, PreRayFire, RaycastCircle);
                    break;
            }
        }
        /// <summary>
        /// Super Delegate Function
        /// </summary>
        /// <param name="eventH">Event Handler we need for the type</param>
        /// <param name="raycastData">Delegate for the raycast information</param>
        /// <param name="hitSuccess"> bool for successful hit</param>
        /// <returns></returns>
        private bool CastRay(EventHandler<FP_RayArgumentHit> CastOnEnterEvent, EventHandler<FP_RayArgumentHit>CastHitEvent, RayDataReturn raycastData)
        {
            bool hitSuccess = false;
            //RaycastType rayType = RaycastType.Line;
            
            FP_RaycastHit hit = raycastData(ref hitSuccess);
            if (hitSuccess)
            {
                FP_RayArgumentHit arg = new FP_RayArgumentHit();
                Transform hitItem=null;
                switch (hit.TheDim)
                {
                    case RaycastDimension.RaycastThreeD:
                        arg = ReturnArgument(hit.ThreeDHit,hit.RayType);
                        hitItem = hit.ThreeDHit.transform;
                        break;
                    case RaycastDimension.RaycastTwoD:
                        arg = ReturnArgument(hit.TwoDHit, hit.RayType);
                        hitItem = hit.TwoDHit.transform;
                        break;
                }
                if (_currentHitItem != null)
                {
                    //we hitting the same object?
                    if (_currentHitItem.HitObject != hitItem && _rayFirstHit)
                    {
                        //we have somehow magically hit something else without leaving the other item
                        //something has crossed in front of us
                        //need to leave the current hit and fire off the enter hit
                        PreRayExit?.Invoke(this, _currentHitItem);
                        CastOnEnterEvent?.Invoke(this, arg);
                        _currentHitItem = arg;
                        //special case break out now
                        return hitSuccess;
                    }
                }
                //update current Hit Item
                _currentHitItem = arg;

                if (!_rayFirstHit)
                {
                    //event arg for first hit
                    CastOnEnterEvent?.Invoke(this, arg);
                    _rayFirstHit = true;
                }
                else
                {
                    //event arg for hit
                    CastHitEvent?.Invoke(this, arg);
                }
            }
            else
            {
                //we are no longer hitting something
                if (_rayFirstHit &&_currentHitItem!=null)
                {
                    //we were hitting something we need to exit?
                    //FP_RayArgumentHit arg = ReturnArgument()
                    //we now left
                    PreRayExit?.Invoke(this, _currentHitItem);
                    _currentHitItem = null;
                    _rayFirstHit = false;
                }
            }
            return hitSuccess;
        }
        /// <summary>
        /// Return a new FP_RayArgumentHit
        /// </summary>
        /// <param name="hitT"></param>
        /// <param name="hitPt"></param>
        /// <returns></returns>
        private FP_RayArgumentHit ReturnArgument(Transform hitT, Vector3 hitPt,RaycastType rayType)
        {
            return new FP_RayArgumentHit()
            {
                HitObject = hitT,
                CastingItem = _raySetup,
                RayType = rayType,
                WorldEndPoint = hitPt,
                WorldOrigin = _raySetup.RayOrigin.position
            };
        }
        private FP_RayArgumentHit ReturnArgument(RaycastHit hitInformation, RaycastType rayType)
        {
            return new FP_RayArgumentHit()
            {
                HitObject = hitInformation.transform,
                CastingItem = _raySetup,
                RayType = rayType,
                WorldEndPoint = hitInformation.point,
                WorldOrigin = _raySetup.RayOrigin.position
            };
        }
        private FP_RayArgumentHit ReturnArgument(RaycastHit2D hitInformation, RaycastType rayType)
        {
            return new FP_RayArgumentHit()
            {
                HitObject = hitInformation.transform,
                CastingItem = _raySetup,
                RayType = rayType,
                WorldEndPoint = _raySetup.FPRayInformation.Vector2Vector3(hitInformation.point),
                WorldOrigin = Vector3.Scale(_raySetup.RayOrigin.position, _raySetup.FPRayInformation.AxisToConvert)
            };
        }
        #region Functions for RaycastHit Return Type
        /// <summary>
        /// Unity 3D Vector Raycast
        /// </summary>
        /// <param name="hitSuccess"></param>
        /// <returns>Returns a FP_RaycastHit with a Line Type</returns>
        private FP_RaycastHit RaycastThreeD( ref bool hitSuccess)
        {
            RaycastHit hit;
            hitSuccess = Physics.Raycast(_raySetup.RayOrigin.position, _raySetup.RayDirection, out hit, _raySetup.FPRayInformation.RaycastLength, _raySetup.FPRayInformation.LayerToInteract);
            return new FP_RaycastHit(hit, RaycastType.Line);
        }
        /// <summary>
        /// Unity 2D Vector Raycast
        /// </summary>
        /// <param name="hitSuccess"></param>
        /// <returns> Returns a FP_RaycastHit with a Line Type</returns>
        private FP_RaycastHit RaycastTwoD(ref bool hitSuccess)
        {
            SO_FPRaycasterTwoD twoD = _raySetup.FPRayInformation as SO_FPRaycasterTwoD;
            RaycastHit2D hit = Physics2D.Raycast(
                twoD.Vector3Vector2(_raySetup.RayOrigin.position),
                twoD.Vector3Vector2(_raySetup.RayDirection),
                twoD.RaycastLength,
                twoD.LayerToInteract);
            hitSuccess = hit.collider != null;
            return new FP_RaycastHit(hit, RaycastType.Line);
        }
        /// <summary>
        /// Full 2D Box Cast
        /// </summary>
        /// <param name="hitSuccess"></param>
        /// <returns>Returns a FP_RaycastHit with a Box Type</returns>
        private FP_RaycastHit RaycastBox(ref bool hitSuccess)
        { 
            SO_FPBoxcaster theboxData = (SO_FPBoxcaster)_raySetup.FPRayInformation;
            RaycastHit2D hit = Physics2D.BoxCast(
                theboxData.Vector3Vector2(_raySetup.RayOrigin.position),
                theboxData.Vector3Vector2(theboxData.BoxExtents),
                theboxData.BoxAngle,
                theboxData.Vector3Vector2(_raySetup.RayDirection),
                theboxData.RaycastLength,
                theboxData.LayerToInteract);
            hitSuccess = hit.collider != null;
            return new FP_RaycastHit(hit, RaycastType.Box);
        }
        /// <summary>
        /// Full 3D Cube Cast
        /// </summary>
        /// <param name="hitSuccess"></param>
        /// <returns>Returns a FP_RaycastHit with a Cube Type</returns>
        private FP_RaycastHit RaycastCube(ref bool hitSuccess)
        {
            RaycastHit hit;
            SO_FPCubecaster theBoxData = (SO_FPCubecaster)_raySetup.FPRayInformation;
            hitSuccess = Physics.BoxCast(
                _raySetup.RayOrigin.position,
                theBoxData.BoxExtents,
                _raySetup.RayDirection,
                out hit,
                Quaternion.Euler(theBoxData.BoxAngle),
                theBoxData.RaycastLength,
                theBoxData.LayerToInteract);
            return new FP_RaycastHit(hit, RaycastType.Cube);
        }
        /// <summary>
        /// Full 3D Sphere Cast
        /// </summary>
        /// <param name="hitSuccess">Returns true if we hit something</param>
        /// <returns>Returns a FP_RaycastHit with a Sphere Type</returns>
        private FP_RaycastHit RaycastSphere(ref bool hitSuccess)
        {
            RaycastHit hit;
            SO_FPSpherecaster theSphereData = (SO_FPSpherecaster)_raySetup.FPRayInformation;
            hitSuccess = Physics.SphereCast(
                _raySetup.RayOrigin.position,
                theSphereData.SphereRadius,
                _raySetup.RayDirection,
                out hit,
                theSphereData.RaycastLength,
                theSphereData.LayerToInteract);
            return new FP_RaycastHit(hit, RaycastType.Sphere);
        }
        /// <summary>
        /// 2D Unity CircleCast
        /// </summary>
        /// <param name="hitSuccess"> returns true if we hit something</param>
        /// <returns>Returns a FP_RaycastHit with a Circle Type</returns>
        private FP_RaycastHit RaycastCircle(ref bool hitSuccess)
        {
            SO_FPCirclecaster theCircleData = (SO_FPCirclecaster)_raySetup.FPRayInformation;

            RaycastHit2D hit = Physics2D.CircleCast(
                theCircleData.Vector3Vector2(_raySetup.RayOrigin.position),
                theCircleData.CircleRadius,
                theCircleData.Vector3Vector2(_raySetup.RayDirection),
                theCircleData.RaycastLength,
                theCircleData.LayerToInteract
                );
            hitSuccess = hit.collider != null;
            return new FP_RaycastHit(hit, RaycastType.Circle);
        }
        #endregion
 
    }
}

