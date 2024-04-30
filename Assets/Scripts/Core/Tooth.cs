using MouthTrainer.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MouthTrainer.Behaviours
{
    public class Tooth : MonoBehaviour, IDraggable
    {
        [Tooltip("—корость возвращени€ в изначальную позицию")]
        public float returningSpeed = 10;
        [Tooltip(" нопка, которую слушает этот зуб дл€ возврата в изначальную позицию")]
        public Button listeningTo;

        Vector3 _initialLocalPos;

        Vector3 _moveStart_LocalPos;
        float _moveStartTime;

        private void Start()
        {
            _initialLocalPos = transform.localPosition;
            listeningTo.onClick.AddListener(RevertToStart);
        }

        public void RevertToStart() 
        {
            _moveStart_LocalPos = transform.localPosition;
            _moveStartTime = Time.realtimeSinceStartup;
            StartCoroutine(RevertingProcess());
        }

        private IEnumerator RevertingProcess() 
        {
            const float CLOSE_ENOUGH = 0.0001f;

            while (Vector3.Distance(transform.localPosition, _initialLocalPos) >= CLOSE_ENOUGH)
            {
                float progress = (Time.realtimeSinceStartup - _moveStartTime) * returningSpeed;

                transform.localPosition = Vector3.Lerp(_moveStart_LocalPos, _initialLocalPos, progress);

                yield return new WaitForEndOfFrame();
            }
        }

        #region IDraggable
        public void OnDrag()
        {
            
        }

        public void OnDragEnd()
        {
            
        }

        public void OnDragStart()
        {
            
        }
        #endregion
    }
}