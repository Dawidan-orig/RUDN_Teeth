using MouthTrainer.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MouthTrainer.Behaviours
{
    public class Tooth : MonoBehaviour, IDraggable
    {
        public float returningSpeed = 10;
        public Button listeningTo;

        Vector3 _initialPos;

        Vector3 _moveStartpos;
        float _moveStartTime;

        private void Start()
        {
            _initialPos = transform.position;
            listeningTo.onClick.AddListener(RevertToStart);
        }

        public void RevertToStart() 
        {
            _moveStartpos = transform.position;
            _moveStartTime = Time.realtimeSinceStartup;
            StartCoroutine(RevertingProcess());
        }

        private IEnumerator RevertingProcess() 
        {
            const float CLOSE_ENOUGH = 0.01f;

            while (Vector3.Distance(transform.position, _initialPos) >= CLOSE_ENOUGH)
            {
                float progress = (Time.realtimeSinceStartup - _moveStartTime) * returningSpeed;

                transform.position = Vector3.Lerp(_moveStartpos, _initialPos, progress);

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