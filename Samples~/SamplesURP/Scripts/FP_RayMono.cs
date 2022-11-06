using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;

namespace FuzzPhyte.Ray.Examples
{
    public class FP_RayMono : MonoBehaviour, IFPRaySetup
    {
        [Header("Raycaster!")]
        public string RaycastInformation;
        [Space]
        public Color RayEnterColor;
        public Color RayStayColor;
        public Color RayExitColor;
        public SO_FPRaycaster RayData;
        public Transform RaycastOrigin;
        public Transform RaycastEndDir;
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
        private bool _rayEnter;
        private bool _rayExit;
        private bool _rayStay;
        #region Callback Functions for Raycast Delegates
        public void OnRayEnter(object sender, FP_RayArgumentHit arg)
        {
            Debug.LogWarning($"RAY Enter: {arg.HitObject.name}");
            _rayEnter = true;
            _rayHit = arg;
        }
        public void OnRayStay(object sender, FP_RayArgumentHit arg)
        {
            Debug.LogWarning($"RAY Stay: {arg.HitObject.name}");
            _rayStay = true;
            _rayHit = arg;
        }
        public void OnRayExit(object sender, FP_RayArgumentHit arg)
        {
            Debug.LogWarning($"RAY Exit: {arg.HitObject.name}");
            _rayExit = true;
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
        public void Update()
        {
            if (_rayEnter)
            {
                Debug.DrawLine(_rayHit.WorldOrigin, _rayHit.WorldEndPoint, RayEnterColor, 5f);
                //Debug.DrawRay(_rayHit.WorldOrigin, _rayHit.Direction, RayEnterColor, 5f);
                _rayEnter = false;
            }
            if (_rayExit)
            {
                Debug.DrawLine(_rayHit.WorldOrigin, _rayHit.WorldEndPoint, RayExitColor, 5f);
                //Debug.DrawRay(_rayHit.WorldOrigin, _rayHit.Direction, RayExitColor, 5f);
                _rayExit = false;
            }
            if (_rayStay)
            {
                switch (RayData)
                {
                    case SO_FPRaycasterThreeD:
                    case SO_FPRaycasterTwoD:
                        Debug.DrawLine(_rayHit.WorldOrigin, _rayHit.WorldEndPoint, RayStayColor, 1f);
                        break;
                    case SO_FPCubecaster:
                        SO_FPCubecaster cube = RayData as SO_FPCubecaster;
                        DrawBox(_rayHit.WorldEndPoint, Quaternion.Euler(cube.BoxAngle), cube.BoxExtents, RayStayColor,1f);
                        break;
                    case SO_FPBoxcaster:
                        SO_FPBoxcaster box = RayData as SO_FPBoxcaster;
                        DrawBox(_rayHit.WorldEndPoint, Quaternion.Euler(box.BoxAngleRotation*box.BoxAngle), box.BoxExtents, RayStayColor, 1f);
                        break;
                    case SO_FPSpherecaster:
                        SO_FPSpherecaster sphere = RayData as SO_FPSpherecaster;
                        Vector4 rayFour = new Vector4(
                            _rayHit.WorldEndPoint.x,
                            _rayHit.WorldEndPoint.y,
                            _rayHit.WorldEndPoint.z,
                            0);
                        DrawSphere(rayFour, sphere.SphereRadius, RayStayColor, 1f);
                        break;
                    case SO_FPCirclecaster:
                        SO_FPCirclecaster circle = RayData as SO_FPCirclecaster;
                        Vector4 rayFourCircle = new Vector4(
                            _rayHit.WorldEndPoint.x,
                            _rayHit.WorldEndPoint.y,
                            _rayHit.WorldEndPoint.z,
                            0);
                        DrawCircle(rayFourCircle, new Vector3(1,0,0), circle.CircleRadius, RayStayColor, 1f);
                        break;
                }
                _rayStay = false;
            }
        }

       
        #region Debug Draw Modifications
        private static readonly Vector4[] s_UnitSquare =
        {
            new Vector4(-0.5f, 0.5f, 0, 1),
            new Vector4(0.5f, 0.5f, 0, 1),
            new Vector4(0.5f, -0.5f, 0, 1),
            new Vector4(-0.5f, -0.5f, 0, 1),
        };
        private static readonly Vector4[] s_UnitSphere = MakeUnitSphere(16);
        private static Vector4[] MakeUnitSphere(int len)
        {
            Debug.Assert(len > 2);
            var v = new Vector4[len * 3];
            for (int i = 0; i < len; i++)
            {
                var f = i / (float)len;
                float c = Mathf.Cos(f * (float)(Math.PI * 2.0));
                float s = Mathf.Sin(f * (float)(Math.PI * 2.0));
                v[0 * len + i] = new Vector4(c, s, 0, 1);
                v[1 * len + i] = new Vector4(0, c, s, 1);
                v[2 * len + i] = new Vector4(s, 0, c, 1);
            }
            return v;
        }
        //Unity Forums: https://forum.unity.com/threads/debug-drawbox-function-is-direly-needed.1038499/
        public void DrawBox(Vector3 pos, Quaternion rot, Vector3 scale, Color c, float time)
        {
            // create matrix
            Matrix4x4 m = new Matrix4x4();
            m.SetTRS(pos, rot, scale);

            var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
            var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
            var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
            var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));

            var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
            var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
            var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
            var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));

            Debug.DrawLine(point1, point2, c,time);
            Debug.DrawLine(point2, point3, c,time);
            Debug.DrawLine(point3, point4, c,time);
            Debug.DrawLine(point4, point1, c,time);

            Debug.DrawLine(point5, point6, c,time);
            Debug.DrawLine(point6, point7, c,time);
            Debug.DrawLine(point7, point8, c,time);
            Debug.DrawLine(point8, point5, c,time);

            Debug.DrawLine(point1, point5, c,time);
            Debug.DrawLine(point2, point6, c,time);
            Debug.DrawLine(point3, point7, c,time);
            Debug.DrawLine(point4, point8, c,time);

            // optional axis display
            //Debug.DrawRay(m.GetPosition(), m.GetForward(), Color.magenta);
            //Debug.DrawRay(m.GetPosition(), m.GetUp(), Color.yellow);
            //Debug.DrawRay(m.GetPosition(), m.GetRight(), Color.red);
        }
        //Unity GitHub https://github.com/Unity-Technologies/Graphics/pull/2287/files#diff-cc2ed84f51a3297faff7fd239fe421ca1ca75b9643a22f7808d3a274ff3252e9R195
        public void DrawSphere(Vector4 pos, float radius, Color color, float time)
        {
            Vector4[] v = s_UnitSphere;
            int len = s_UnitSphere.Length / 3;
            for (int i = 0; i < len; i++)
            {
                var sX = pos + radius * v[0 * len + i];
                var eX = pos + radius * v[0 * len + (i + 1) % len];
                var sY = pos + radius * v[1 * len + i];
                var eY = pos + radius * v[1 * len + (i + 1) % len];
                var sZ = pos + radius * v[2 * len + i];
                var eZ = pos + radius * v[2 * len + (i + 1) % len];
                Debug.DrawLine(sX, eX, color, time);
                Debug.DrawLine(sY, eY, color, time);
                Debug.DrawLine(sZ, eZ, color, time);
            }
        }
        public void DrawCircle(Vector4 pos, Vector3 drawSide,float radius, Color color, float time)
        {
            Vector4[] v = s_UnitSphere;
            int len = s_UnitSphere.Length / 3;
            for (int i = 0; i < len; i++)
            {
                var sX = pos + radius * v[0 * len + i];
                var eX = pos + radius * v[0 * len + (i + 1) % len];
                var sY = pos + radius * v[1 * len + i];
                var eY = pos + radius * v[1 * len + (i + 1) % len];
                var sZ = pos + radius * v[2 * len + i];
                var eZ = pos + radius * v[2 * len + (i + 1) % len];
                if (drawSide.x != 0)
                {
                    Debug.DrawLine(sX, eX, color, time);
                }
                if (drawSide.y != 0)
                {
                    Debug.DrawLine(sY, eY, color, time);
                }
                if (drawSide.z != 0)
                {
                    Debug.DrawLine(sZ, eZ, color, time);
                }
            }
        }
        #endregion
    }

}
