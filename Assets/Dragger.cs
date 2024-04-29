using MouthTrainer.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MouthTrainer.Input
{
    public class Dragger : MonoBehaviour
    {
        public InputAction leftMouseButton;
        public InputAction mouseMovement;
        public float maxDragDistance = 10;
        // 64 - это слой "Interactable"
        public LayerMask checkingInLayer = 64;

        // Можно и одну переменную использовать для удобства,
        // И часто делать GetComponent().
        // Но я предпочту кэшировать.
        private GameObject _currentDraggable;
        private Vector3 _hitOffset;
        private float _currentDragDistance;
        private IDraggable _currentDraggableInterface;

        #region Unity

        private void OnEnable()
        {
            leftMouseButton.Enable();
            mouseMovement.Enable();
        }

        private void Start()
        {
            leftMouseButton.started += DragStart;
            mouseMovement.performed += DoDrag;
            leftMouseButton.canceled += DragEnd;
        }

        private void OnDisable()
        {
            leftMouseButton.Disable();
            mouseMovement.Disable();
        }

        #endregion

        #region Dragging
        private void DragStart(InputAction.CallbackContext ctx)
        {
            Vector3 mouseDir = GetMouseDir();

            if (Physics.Raycast(transform.position,
                mouseDir,
                out RaycastHit hit,
                checkingInLayer)
                )
                if (hit.transform.TryGetComponent(out IDraggable comp))
                {
                    _currentDraggable = hit.transform.gameObject;
                    _currentDraggableInterface = comp;
                    _currentDraggableInterface.OnDragStart();

                    _currentDragDistance = Vector3.Distance(transform.position,
                        _currentDraggable.transform.position);
                    _currentDragDistance = Mathf.Clamp(_currentDragDistance,0,maxDragDistance);

                    _hitOffset = hit.transform.position - hit.point;
                }
        }
        private void DoDrag(InputAction.CallbackContext ctx)
        {
            if (_currentDraggable == null)
                return;

            Vector3 toMouseDir = GetMouseDir();

            if (Physics.Raycast(transform.position,
                toMouseDir,
                out RaycastHit hit,
                ~checkingInLayer)
                )
                _currentDraggable.transform.position = hit.point + _hitOffset;
            else
                _currentDraggable.transform.position =
                    transform.position + toMouseDir * _currentDragDistance + _hitOffset;

             // Вызов функции лучше делать после всего,
             // Чтобы при использовании функции интерфейса
             // У объекта сразу был обновлённый transform
             _currentDraggableInterface.OnDrag();
        }
        private void DragEnd(InputAction.CallbackContext ctx) 
        {
            if (_currentDraggable == null)
                return;

            _currentDraggableInterface.OnDragEnd();
            _currentDraggableInterface = null;
            _currentDraggable = null;
        }
        #endregion

        private Vector3 GetMouseDir() 
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
                (Vector3)Mouse.current.position.value + Vector3.forward);
            return (mouseWorld - transform.position).normalized;
        }
    }
}