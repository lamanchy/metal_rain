using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entities {
    public class Mothership : MovingEntity {
        [Header("Mothership specific")]
        public float FreezeValue;

        public float MaxFreeze;

        public int FreezePerSecond;
        public int ThawPerSecond;
        
        private float FreezePerTick => FreezePerSecond / 60f;
        private float ThawPerTick => ThawPerSecond / 60f;

        public bool IsThawing => FreezeValue < MaxFreeze;

        protected override void FixedUpdate() {
            base.FixedUpdate();
            FreezeValue = IsPowered 
                ? Mathf.Min(MaxFreeze, FreezeValue + FreezePerTick) 
                : Mathf.Max(0, FreezeValue - ThawPerTick);

            if (Math.Abs(FreezeValue) < 0.01f) {
                Explode();
            }
        }

        public override void Explode() {
            base.Explode();
            SceneManager.LoadScene("GameOverScene");
        }
    }
}
