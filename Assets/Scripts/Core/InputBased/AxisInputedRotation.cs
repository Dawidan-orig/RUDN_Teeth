using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MouthTrainer.Input {
    public class AxisInputedRotation : MonoBehaviour
    {
        public InputAction axisMovementInput;
        public float rotationSpeed = 50;

        private void OnEnable()
        {
            axisMovementInput.Enable();
        }

        private void Update()
        {
            Vector3 movementInput = axisMovementInput.ReadValue<Vector3>();
            transform.rotation *= Quaternion.Euler(movementInput * rotationSpeed * Time.deltaTime);
        }

        private void OnDisable()
        {
            axisMovementInput.Disable();
        }
    }
}