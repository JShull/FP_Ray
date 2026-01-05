namespace FuzzPhyte.Ray.Editor
{
    using FuzzPhyte.Utility;
    using System;
    using System.Reflection;
    using Unity.Mathematics;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public static class FP_RaycasterGizmoDrawer
    {
        // Visual defaults
        private const float k_DefaultInfiniteVisualLength = 500f;
        private const float k_ArrowSize = 0.08f;
        private const float k_distSizeScale = 0.025f;
        private const float k_MinDirSqr = 0.000001f;

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawRaySetupGizmos(MonoBehaviour target, GizmoType gizmoType)
        {
            if (target is not IFPRaySetup setup)
                return;
            if(!setup.DrawEditorGizmos)
                return;
            var info = setup.FPRayInformation;
            if (info == null)
                return;

            if (setup.RayOrigin == null)
                return;

            Vector3 origin = setup.RayOrigin.position;

            Vector3 dir = (Vector3)setup.RayDirection;
            if (dir.sqrMagnitude < 0.000001f)
                dir = target.transform.forward;

            if (dir.sqrMagnitude < 0.000001f)
                return;

            dir.Normalize();

            float length = info.RaycastLength <= 0f ? 500f : info.RaycastLength;
            float superScale = info.RaycastLength <= 0f ? 0.1f : 1;
            Vector3 end = origin + dir * length;
            Vector3 uiEnd = origin + dir * (length *0.5f);
            Color color = info switch
            {
                SO_FPSpherecaster => Color.green,
                SO_FPBoxcaster => Color.magenta,
                _ => Color.cyan
            };

            Handles.color = color;
            switch (info)
            {
                case SO_FPSpherecaster sphere:
                    DrawSphereSweep(origin, end, dir, sphere.SphereRadius);
                    break;
                case SO_FPBoxcaster box:
                    DrawBoxSweep(origin, end, ResolveBoxRotation(box), box.BoxExtents);
                    break;
                default:
                    Handles.DrawLine(origin, end);
                    break;
            }
            DrawArrow(uiEnd, dir, length * 0.5f * superScale, color);
            bool isSelected = (gizmoType & GizmoType.Selected) != 0;
            if (isSelected)
            {
                DrawLabel(origin, info, length);
            }
            
            DrawCastVolumeSweep(origin, end, dir, info);
            Handles.SphereHandleCap(0, origin, Quaternion.identity, 0.03f, EventType.Repaint);
            Handles.DrawSolidDisc(end, dir, k_distSizeScale * length*superScale);
            //Handles.SphereHandleCap(1, end, Quaternion.identity, 0.03f, EventType.Repaint);
            //Handles.ConeHandleCap(0, end, Quaternion.LookRotation(dir), 0.1f, EventType.Repaint);
        }

        private static void DrawArrow(Vector3 end, Vector3 dir, float length, Color c)
        {
            float size = Mathf.Max(0.1f, length * k_ArrowSize);
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
            Color prev = Handles.color;
            Handles.color = c;
            Handles.ConeHandleCap(0, end, rot, size, EventType.Repaint);
            Handles.color = prev;
        }

        private static void DrawLabel(Vector3 origin, SO_FPRaycaster info, float length)
        {
            // Keep label readable even when non-selected
            GUIStyle previous = new GUIStyle(EditorStyles.label);
            //Color prev = Handles.color;
            //Handles.color = Color.black;
            //font style
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.black;
            string typeName = info.GetType().Name;
            Handles.Label(origin + Vector3.up * 0.05f, $"{typeName} Len: {length:0.##}", labelStyle);
        }

        private static void DrawCastVolumeSweep(Vector3 start, Vector3 end, Vector3 dir, SO_FPRaycaster info)
        {
            // Sphere / Circle radius checks
            if (TryGetFloat(info, "SphereRadius", out float sphereRadius))
            {
                DrawSphereSweep(start, end, dir, sphereRadius);
                return;
            }

            if (TryGetFloat(info, "CircleRadius", out float circleRadius))
            {
                DrawCircleSweep(start, end, dir, circleRadius, Resolve2DNormal(info));
                return;
            }

            // Box/Cube extents checks
            if (TryGetVector3(info, "BoxExtents", out Vector3 boxExtents))
            {
                Quaternion rot = ResolveBoxRotation(info);
                DrawBoxSweep(start, end, rot, boxExtents);
                return;
            }

            // If no known fields found, it's a plain line caster (already drawn)
        }

        private static void DrawSphereSweep(Vector3 start, Vector3 end, Vector3 dir, float radius)
        {
            // Start + end spheres
            Handles.DrawSolidDisc(start, dir, radius);
            Handles.DrawSolidDisc(end, dir, radius);
            //Handles.DrawWireSphere(start, radius);
            //Handles.DrawWireSphere(end, radius);

            // Connect with 4 lines around the “capsule”
            BuildOrthonormalBasis(dir, out Vector3 u, out Vector3 v);

            Handles.DrawLine(start + u * radius, end + u * radius);
            Handles.DrawLine(start - u * radius, end - u * radius);
            Handles.DrawLine(start + v * radius, end + v * radius);
            Handles.DrawLine(start - v * radius, end - v * radius);
        }

        private static void DrawCircleSweep(Vector3 start, Vector3 end, Vector3 dir, float radius, Vector3 normal)
        {
            // Start + end discs
            Handles.DrawWireDisc(start, normal, radius);
            Handles.DrawWireDisc(end, normal, radius);

            // Connect with 2-4 lines in-plane
            BuildOrthonormalBasis(normal, out Vector3 u, out Vector3 v);

            Handles.DrawLine(start + u * radius, end + u * radius);
            Handles.DrawLine(start - u * radius, end - u * radius);
            Handles.DrawLine(start + v * radius, end + v * radius);
            Handles.DrawLine(start - v * radius, end - v * radius);
        }

        private static void DrawBoxSweep(Vector3 start, Vector3 end, Quaternion rot, Vector3 halfExtents)
        {
            // Draw oriented wire boxes at start and end
            DrawWireBox(start, rot, halfExtents);
            DrawWireBox(end, rot, halfExtents);

            // Connect corresponding corners to show sweep volume
            Vector3[] a = GetBoxCornersWorld(start, rot, halfExtents);
            Vector3[] b = GetBoxCornersWorld(end, rot, halfExtents);

            for (int i = 0; i < 8; i++)
                Handles.DrawLine(a[i], b[i]);
        }

        private static void DrawWireBox(Vector3 center, Quaternion rot, Vector3 halfExtents)
        {
            Matrix4x4 prev = Handles.matrix;
            Handles.matrix = Matrix4x4.TRS(center, rot, Vector3.one);
            Handles.DrawWireCube(Vector3.zero, halfExtents * 2f);
            Handles.matrix = prev;
        }

        private static Vector3[] GetBoxCornersWorld(Vector3 center, Quaternion rot, Vector3 halfExtents)
        {
            // 8 corners in local space
            Vector3[] local =
            {
                new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z),
                new Vector3( halfExtents.x, -halfExtents.y, -halfExtents.z),
                new Vector3( halfExtents.x,  halfExtents.y, -halfExtents.z),
                new Vector3(-halfExtents.x,  halfExtents.y, -halfExtents.z),

                new Vector3(-halfExtents.x, -halfExtents.y,  halfExtents.z),
                new Vector3( halfExtents.x, -halfExtents.y,  halfExtents.z),
                new Vector3( halfExtents.x,  halfExtents.y,  halfExtents.z),
                new Vector3(-halfExtents.x,  halfExtents.y,  halfExtents.z),
            };

            Vector3[] world = new Vector3[8];
            for (int i = 0; i < 8; i++)
                world[i] = center + (rot * local[i]);

            return world;
        }

        private static void BuildOrthonormalBasis(Vector3 n, out Vector3 u, out Vector3 v)
        {
            n.Normalize();
            u = Vector3.Cross(n, Vector3.up);
            if (u.sqrMagnitude < 0.0001f)
                u = Vector3.Cross(n, Vector3.right);

            u.Normalize();
            v = Vector3.Cross(n, u).normalized;
        }

        private static Vector3 Resolve2DNormal(SO_FPRaycaster info)
        {
            // AxisToConvert is documented as “0 or 1 for 2D”
            // Common setups:
            // (1,1,0) => XY plane => normal Z
            // (1,0,1) => XZ plane => normal Y
            // (0,1,1) => YZ plane => normal X
            Vector3 axis = info.AxisToConvert;

            if (Mathf.Approximately(axis.z, 0f) && (!Mathf.Approximately(axis.x, 0f) || !Mathf.Approximately(axis.y, 0f)))
                return Vector3.forward;

            if (Mathf.Approximately(axis.y, 0f) && (!Mathf.Approximately(axis.x, 0f) || !Mathf.Approximately(axis.z, 0f)))
                return Vector3.up;

            if (Mathf.Approximately(axis.x, 0f) && (!Mathf.Approximately(axis.y, 0f) || !Mathf.Approximately(axis.z, 0f)))
                return Vector3.right;

            // Fallback
            return Vector3.forward;
        }

        private static Quaternion ResolveBoxRotation(SO_FPRaycaster info)
        {
            // Your runtime debug uses either:
            // - Quaternion.Euler(cube.BoxAngle) where BoxAngle is Vector3 (cubecaster)
            // - Quaternion.Euler(box.BoxAngleRotation * box.BoxAngle) where BoxAngle is float (boxcaster)
            if (TryGetVector3(info, "BoxAngle", out Vector3 eulerAngles))
                return Quaternion.Euler(eulerAngles);

            if (TryGetFloat(info, "BoxAngle", out float angleScalar) && TryGetVector3(info, "BoxAngleRotation", out Vector3 axis))
                return Quaternion.Euler(axis * angleScalar);

            return Quaternion.identity;
        }

        private static bool TryGetFloat(object obj, string fieldName, out float value)
        {
            value = default;

            var f = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (f == null || f.FieldType != typeof(float))
                return false;

            value = (float)f.GetValue(obj);
            return true;
        }

        private static bool TryGetVector3(object obj, string fieldName, out Vector3 value)
        {
            value = default;

            var f = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (f == null || f.FieldType != typeof(Vector3))
                return false;

            value = (Vector3)f.GetValue(obj);
            return true;
        }
    }
}
