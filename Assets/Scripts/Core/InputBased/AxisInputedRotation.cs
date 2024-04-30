using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MouthTrainer.Input {
    public class AxisInputedRotation : MonoBehaviour
    {
        [Tooltip("ƒействие ввода, определ€ющиее поворот")]
        public InputAction axisMovementInput;
        [Tooltip("—корость поворота всего transform'а")]
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