using MouthTrainer.Core;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MouthTrainer.Input
{
    public class Dragger : MonoBehaviour
    {
        [Tooltip("Действие взаимодействия")]
        public InputAction interaction;
        [Tooltip("Действие изменения ввода для взятого объекта")]
        public InputAction holdedMovement;
        [Tooltip("Действие группировки")]
        public InputAction groupingButton;
        [Tooltip("Макс. количество объектов для группировки")]
        public int groupingAmount = 3;
        [Tooltip("Максмальная дистанция, на которую можно отодвинуть объект")]
        public float maxDragDistance = 10;
        // 64 - это слой "Interactable"
        [Tooltip("Слой проверки взаимодействующих")]
        public LayerMask checkingInLayer = 64;

        #region grouping
        private GameObject _groupper;
        private Dictionary<Transform, Transform> _objectToParent;
        #endregion

        #region dragging
        private GameObject _currentDraggable;
        private Vector3 _hitOffset;
        private float _currentDragDistance;
        #endregion

        #region Unity

        private void OnEnable()
        {
            interaction.Enable();
            holdedMovement.Enable();
            groupingButton.Enable();
        }

        private void Start()
        {
            interaction.started += DragStart;
            holdedMovement.performed += DoDrag;
            interaction.canceled += DragEnd;
        }

        private void OnDisable()
        {
            interaction.Disable();
            holdedMovement.Disable();
            groupingButton.Disable();
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
            {
                if (hit.transform.TryGetComponent(out IDraggable comp) && comp.AvailableToDrag())
                {
                    if (groupingButton.IsPressed())
                    {
                        if (_groupper == null)
                        {
                            _groupper = new GameObject("Grouping of " + transform.name);
                            _objectToParent = new();
                        }

                        _currentDraggable = _groupper;

                        if (_groupper?.transform.childCount < groupingAmount)
                        {
                            if (!_objectToParent.ContainsKey(hit.transform))
                                _objectToParent.Add(hit.transform, hit.transform.parent);
                            hit.transform.parent = _groupper.transform;
                        }
                        else
                            return;
                    }
                    else // Попадание по перетаскиваему без группировки
                    {
                        ReleaseGrouping();
                        _currentDraggable = hit.transform.gameObject;
                    }

                    comp.OnDragStart();

                    _hitOffset = _currentDraggable.transform.position - hit.point;

                    _currentDragDistance = Vector3.Distance(transform.position,
                        hit.transform.position - _hitOffset);
                    _currentDragDistance = Mathf.Clamp(_currentDragDistance, 0, maxDragDistance);
                }
            }
            else // Нажатие в пустоту
            {
                ReleaseGrouping();
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
            foreach (IDraggable draggable in GetAllAvailableInterfaces())
                draggable.OnDrag();
        }
        private void DragEnd(InputAction.CallbackContext ctx) 
        {
            if (_currentDraggable == null)
                return;

            foreach(IDraggable draggable in GetAllAvailableInterfaces())
                draggable.OnDragEnd();

            _currentDraggable = null;
        }
        #endregion

        private Vector3 GetMouseDir() 
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
                (Vector3)Mouse.current.position.value + Vector3.forward);
            return (mouseWorld - transform.position).normalized;
        }
        private List<IDraggable> GetAllAvailableInterfaces() 
        {
            List<IDraggable> res = new List<IDraggable>();
            if (_groupper == null)
            {
                res.Add(_currentDraggable.GetComponent<IDraggable>());
            }
            else 
            {
                res.AddRange(_groupper.GetComponentsInChildren<IDraggable>());
            }

            return res;
        }
        private void ReleaseGrouping() 
        {
            if (_groupper == null)
                return;

            for(int i = 0; i < _groupper.transform.childCount; i++) 
            {
                Transform child = _groupper.transform.GetChild(i);
                child.parent = _objectToParent[child];
                i--;
            }

            Destroy(_groupper);
            _objectToParent = null;
        }
    }
}