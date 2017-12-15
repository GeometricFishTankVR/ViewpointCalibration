using UnityEngine;
using UnityEngine.EventSystems;

namespace Biglab.UI
{
    public class LaserPointerEventData : PointerEventData
    {
        public GameObject current;

        public LaserPointerEventData( EventSystem e )
            : base( e )
        { }

        public override void Reset()
        {
            current = null;
            base.Reset();
        }
    }
}
