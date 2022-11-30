using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Scripting;

namespace FuzzPhyte.Ray
{
    /// <summary>
    /// This debug class is to help visualize the work of the RayCaster you're using.
    /// You should remove this class before deploying and/or building as it does double the overhead
    /// </summary>
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
        
        private IFPRaySetup _rayData;
        private void Awake()
        {
            if (RayData == null)
            {
                Debug.LogError($"You need to reference a Mono class that is using the IFPRaySetup Interface");
            }
            else
            {
                _rayData = RayData as IFPRaySetup;
            }
            
            Debug.LogWarning($"Name of RaySetup: {RayData.GetType().Name}");
        }
        
        public void OnEnable()
        {
            StartCoroutine(DelayOnEnable());
        }
        private IEnumerator DelayOnEnable()
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
                    DrawBox(arg.WorldEndPoint, Quaternion.Euler(cube.BoxAngle), cube.BoxExtents, RayStayColor, 1f);
                    break;
                case SO_FPBoxcaster:
                    SO_FPBoxcaster box = _rayData.FPRayInformation as SO_FPBoxcaster;
                    DrawBox(arg.WorldEndPoint, Quaternion.Euler(box.BoxAngleRotation * box.BoxAngle), box.BoxExtents, RayStayColor, 1f);
                    break;
                case SO_FPSpherecaster:
                    SO_FPSpherecaster sphere = _rayData.FPRayInformation as SO_FPSpherecaster;
                    Vector4 rayFour = new Vector4(
                        arg.WorldEndPoint.x,
                        arg.WorldEndPoint.y,
                        arg.WorldEndPoint.z,
                        0);
                    DrawSphere(rayFour, sphere.SphereRadius, RayStayColor, 1f);
                    break;
                case SO_FPCirclecaster:
                    SO_FPCirclecaster circle = _rayData.FPRayInformation as SO_FPCirclecaster;
                    Vector4 rayFourCircle = new Vector4(
                        arg.WorldEndPoint.x,
                        arg.WorldEndPoint.y,
                        arg.WorldEndPoint.z,
                        0);
                    DrawCircle(rayFourCircle, new Vector3(1, 0, 0), circle.CircleRadius, RayStayColor, 1f);
                    break;
            }
        }
        public void OnRayExit(object sender, FP_RayArgumentHit arg)
        {
            Debug.DrawLine(arg.WorldOrigin, arg.WorldEndPoint, RayExitColor, 5f);
        }
        #region Debug Unity Drawing
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
        /// <summary>
        /// Stolen from the Unity Forums: https://forum.unity.com/threads/debug-drawbox-function-is-direly-needed.1038499/
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="scale"></param>
        /// <param name="c"></param>
        /// <param name="time"></param>
        public void DrawBox(Vector3 pos, Quaternion rot, Vector3 scale, Color c, float time)
        {
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

            Debug.DrawLine(point1, point2, c, time);
            Debug.DrawLine(point2, point3, c, time);
            Debug.DrawLine(point3, point4, c, time);
            Debug.DrawLine(point4, point1, c, time);

            Debug.DrawLine(point5, point6, c, time);
            Debug.DrawLine(point6, point7, c, time);
            Debug.DrawLine(point7, point8, c, time);
            Debug.DrawLine(point8, point5, c, time);

            Debug.DrawLine(point1, point5, c, time);
            Debug.DrawLine(point2, point6, c, time);
            Debug.DrawLine(point3, point7, c, time);
            Debug.DrawLine(point4, point8, c, time);
        }
        /// <summary>
        /// Stole from the Unity GitHub https://github.com/Unity-Technologies/Graphics/pull/2287/files#diff-cc2ed84f51a3297faff7fd239fe421ca1ca75b9643a22f7808d3a274ff3252e9R195
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        /// <param name="time"></param>
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
        /// <summary>
        /// Also stolen from the Unity Forums thread from above
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="drawSide"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        /// <param name="time"></param>
        public void DrawCircle(Vector4 pos, Vector3 drawSide, float radius, Color color, float time)
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
