using UnityEngine;
using System;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FuzzPhyte.Ray
{
    public class FP_ClickInputListener : MonoBehaviour
    {
#if ENABLE_INPUT_SYSTEM
        [SerializeField] protected InputAction clickAction;
#endif
        public Action<Vector2> OnClickPerformed;

        protected virtual void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            if (clickAction==null)
                return;
            clickAction.Enable();
            clickAction.performed+= OnClick;
#endif
        }

        protected virtual void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            if (clickAction==null)
                return;
            clickAction.performed-= OnClick;
            clickAction.Disable();
#endif
        }
#if ENABLE_INPUT_SYSTEM
        protected virtual void OnClick(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
            {
                return;
            }
            Vector2 screenPos = Vector2.zero;

            // Pointer / mouse / touch position (if available)
            if (Pointer.current != null)
            {
                screenPos = Pointer.current.position.ReadValue();
                // Screen bounds check
                if (screenPos.x < 0 || screenPos.y < 0 ||
                    screenPos.x > Screen.width || screenPos.y > Screen.height)
                {
                    return;
                }
            }
            OnClickPerformed?.Invoke(screenPos);
            Debug.Log($"Click performed at screen position: {screenPos}");
        }
#endif
    }
}
