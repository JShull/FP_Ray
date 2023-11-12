using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic Jump for ground detection example
/// </summary>
namespace FuzzPhyte.Ray.Examples
{
    public class SimpleJump : MonoBehaviour
    {
        [Tooltip("This is just a quick way to get our raycast information")]
        public FP_RayMono RaycastReference;
        [Space]
        [Header("Jump Related Parameters")]
        [Tooltip("This is the force applied to the rigidbody when jumping")]
        public float jumpForce = 10.0f; // The force that will be applied when the capsule jumps
        [Tooltip("This is the input key we are registering for 'Jump'")]
        public KeyCode jumpKey = KeyCode.Space; // The key that will be used to make the capsule jump
        [Tooltip("Reference to the Rigidbody for our 'player'")]
        public Rigidbody2D RB; // A reference to the capsule's Rigidbody component
        [SerializeField]
        private bool _OnGround;
        /// <summary>
        /// Make sure to assign your listeners to the start function because our existing FP_RayMono example uses
        /// OnEnable to activate our FP_Raycaster script. 
        /// FYI: Unity order of operations: Awake-->OnEnable-->Reset-->Start
        /// https://docs.unity3d.com/Manual/ExecutionOrder.html
        /// </summary>
        private void Start()
        {
            if (RB == null)
            {
                if (GetComponent<Rigidbody2D>())
                {
                    RB = GetComponent<Rigidbody2D>();
                }
                else
                {
                    Debug.LogError($"Missing the Rigidbody reference");
                }
            }
            if (RaycastReference == null)
            {
                if (GetComponent<FP_RayMono>())
                {
                    RaycastReference = GetComponent<FP_RayMono>();
                }
                else
                {
                    Debug.LogError($"Missing the FP_RayMono reference");
                }
            }
            RaycastReference.Raycaster.OnFPRayFireHit += OnGroundRay;
            RaycastReference.Raycaster.OnFPRayExit += OnGroundRayExit;
        }
        public void OnDisable()
        {
            if (RaycastReference != null)
            {
                RaycastReference.Raycaster.OnFPRayFireHit -= OnGroundRay;
                RaycastReference.Raycaster.OnFPRayExit -= OnGroundRayExit;
            }
        }
        /// <summary>
        /// Our Event Function / listener for Raycast hit/stay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        public void OnGroundRay(object sender, FP_RayArgumentHit arg)
        {
            ///because we are already filtering via the scriptable object and for the 'ground' layer
            ///we can just jump right into setting our 'OnGround' boolean true
            _OnGround = true;
        }
        
        public void OnGroundRayExit(object sender, FP_RayArgumentHit arg)
        {
            ///because we are already filtering via the scriptable object and for the 'ground' layer
            ///we can just jump right into setting our 'OnGround' boolean false
            _OnGround = false;
        }
        void Update()
        {
            ///if we are on the ground and we happened to have pushed down on our jump key
            ///we jump
            if (_OnGround && Input.GetKeyDown(jumpKey))
            {
                // Add an upward force to the capsule's Rigidbody
                RB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            
        }
    }

}
