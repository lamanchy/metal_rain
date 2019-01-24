using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Wreckage;
using Meteor;
using UnityEditor;
using UnityEngine;

namespace Entities.Buildings {
    public class ExtractionCoil : StaticEntity {
        [Header("Coil specific")]
        public int ExtractionRange;
        public int ExtractionPerTick;

        private GameObject effectStart;
        private GameObject effectEnd;

        private SortedSet<FallenWreckage> wreckageInRange;

        private bool isExtracting;
        private EnergyTransferEffect transferLightning;
        private EnergyTransferEffect effectLightning;

        public static event Action<ExtractionCoil> OnCoilBuilt;

        protected override void Start() {
            effectStart = transform.Find("EffectStart").gameObject;
            effectEnd = transform.Find("EffectEnd").gameObject;

            base.Start();

            wreckageInRange = new SortedSet<FallenWreckage>(new ObjectDistanceComparer(this));
            OnCoilBuilt?.Invoke(this);
            FallingWreckage.OnWreckageFallen += OnWreckageFallen;
        }

        private void OnDestroy() {
            FallingWreckage.OnWreckageFallen -= OnWreckageFallen;
        }

        protected override void FixedUpdate() {
            if (!IsPowered) {
                return;
            }

            // Clear destroyed wreckage
            while (wreckageInRange.Count > 0 && wreckageInRange.Min == null) {
                wreckageInRange.Remove(wreckageInRange.Min);
            }

            // Extract from nearest wreckage
            if (!isExtracting && wreckageInRange.Count > 0 && wreckageInRange.Min != null) {
                StartCoroutine(Extract());
            }

            base.FixedUpdate();
        }

        private IEnumerator Extract() {
            isExtracting = true;
            while (wreckageInRange.Count > 0) {
                using (new EnergyTransferEffect(gameObject, wreckageInRange.Min.gameObject)) {
                    while (wreckageInRange.Min != null && wreckageInRange.Min.IsPowered) {
                        TransferEnergy(-ExtractionPerTick, wreckageInRange.Min);
                        yield return null;
                    }
                }
                if (wreckageInRange.Min == null) {
                    wreckageInRange.Remove(wreckageInRange.Min);
                    continue;
                }
                wreckageInRange.Min.DestroySelf();
                wreckageInRange.Remove(wreckageInRange.Min);
            }
            isExtracting = false;
        }

        protected override void PowerUp() {
            base.PowerUp();
            effectLightning = new EnergyTransferEffect(effectStart, effectEnd);
        }

        protected override void PowerDown() {
            base.PowerDown();
            effectLightning?.Dispose();
        }

        public void OnWreckageFallen(FallenWreckage wreckage) {
            if (Vector3.Distance(transform.position, wreckage.transform.position) > ExtractionRange) {
                return;
            }
            wreckageInRange.Add(wreckage);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawWireSphere(transform.position, ExtractionRange);
        }
    }
}
