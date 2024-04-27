using UnityEngine;
using UnityEngine.InputSystem;

namespace MouthTrainer.Input
{
    public class FirstPersonCameraControl : MonoBehaviour
    {
        public float translationSpeed = 1;
        public float rotationSensitivity = 75;
        public InputAction WASD_Controls;
        public InputAction mouseControls;

        private Vector2 eulerRotation;

        #region Unity
        // Можно было бы сделать через событие performed у мышки,
        // Но тогда управление и изображение дёргается сильно.
        // Потому было решено убрать всё просто в Update

        private void OnEnable()
        {
            WASD_Controls.Enable();
            mouseControls.Enable();
        }
        private void Update()
        {
            ControlMouse();
            ControlKeyboard();
        }

        private void OnDisable()
        {
            WASD_Controls.Disable();
            mouseControls.Disable();
        }

        #endregion

        private void ControlKeyboard() 
        {
            if (!Mouse.current.rightButton.isPressed)
                return;

            Vector2 movementInput = WASD_Controls.ReadValue<Vector2>();
            Vector3 movement = Vector3.zero;
            movement += transform.forward * translationSpeed * movementInput.y;
            movement += transform.right * translationSpeed * movementInput.x;

            transform.position += movement * Time.deltaTime;
        }
        private void ControlMouse() 
        {
            if (!Mouse.current.rightButton.isPressed)
                return;

            Vector2 rotationInput = mouseControls.ReadValue<Vector2>();
            Vector2 additiveRotation = rotationInput * rotationSensitivity * Time.deltaTime;
            eulerRotation += new Vector2(-additiveRotation.y, additiveRotation.x);
            transform.localEulerAngles = eulerRotation;
        }
    }
}