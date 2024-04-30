using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MouthTrainer.Core
{
    public interface IDraggable
    {
        public virtual bool AvailableToDrag() 
        {
            return true;
        }
        public abstract void OnDragStart();
        public abstract void OnDrag();
        public abstract void OnDragEnd();
    }
}