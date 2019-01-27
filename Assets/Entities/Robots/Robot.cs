using UnityEngine;

namespace Entities {
    public class Robot : MovingEntity
    {
        public AudioSource source;
        protected Vector3 lastPosition;

        protected override void Start()
        {
            base.Start();

            source.Pause();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if(Vector3.Distance(transform.position, lastPosition) > Mathf.Epsilon)
            {
                source.UnPause();
            }
            else
            {
                source.Pause();
            }

            lastPosition = transform.position;
        }
    }
}
