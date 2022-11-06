using UnityEngine;

namespace FuzzPhyte.Ray.Examples
{
    public class CameraExample : MonoBehaviour
    {
        public float CameraSensitivity=0.1f;
        public float PlayerRotationSensitivity = 1f;
        public float LookLimit=45;
        public bool Inverse = true;
        public Transform PlayerRoot;
        public Transform Camera3DPivot;
        public Transform TransformTwoDPivot;
        public bool ThreeDCamera = true;
        private float rotX;
        private float rotZ;
        private void OnEnable()
        {
            if (PlayerRoot == null)
            {
                Debug.LogWarning($"Need a reference for the Transform Player Root, setting it to this transform: {this.name}..");
                PlayerRoot = this.transform;
            }
            if (!ThreeDCamera)
            {
                if (TransformTwoDPivot == null)
                {
                    Debug.LogError($"Need a reference for the 2D Camera Root");
                }
            }
            else
            {
                if(Camera3DPivot == null)
                {
                    Debug.LogWarning($"Need a reference for the Camera Root, setting it to the first camera we find in a child object: {this.name}..");
                    var firstChildCamera = GetComponentInChildren<Camera>();
                    if (firstChildCamera != null)
                    {
                        Camera3DPivot = firstChildCamera.transform;
                    }
                    else
                    {
                        Debug.LogError($"NO CAMERA FOUND");
                    }
                }
            }
        }
        void Update()
        {
            
#if ENABLE_LEGACY_INPUT_MANAGER
            float rotateHorizontal = Input.GetAxis("Mouse X");
            float rotateVertical = Input.GetAxis("Mouse Y");
            if (ThreeDCamera)
            {
                if (Inverse)
                {
                    rotX += -rotateVertical * CameraSensitivity;
                }
                else
                {
                    rotX += rotateVertical * CameraSensitivity;
                }
                rotX = Mathf.Clamp(rotX, -LookLimit, LookLimit);
                Camera3DPivot.transform.localRotation = Quaternion.Euler(rotX * CameraSensitivity, 0, 0);
                PlayerRoot.rotation *= Quaternion.Euler(0, rotateHorizontal * PlayerRotationSensitivity, 0);
            }
            else
            {
                if (Inverse)
                {
                    rotZ += -rotateVertical * CameraSensitivity;
                }
                else
                {
                    rotZ += rotateVertical * CameraSensitivity;
                }
                rotZ = Mathf.Clamp(rotZ, -LookLimit, LookLimit);
                TransformTwoDPivot.transform.localRotation = Quaternion.Euler(0, 0, rotZ * CameraSensitivity);
            }
                 
#endif
        }
    }
}

