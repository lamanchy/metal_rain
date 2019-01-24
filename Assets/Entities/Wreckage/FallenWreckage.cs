using System.Collections.Generic;
using Entities.Buildings;
using UnityEngine;

namespace Entities.Wreckage {
    public class FallenWreckage : StaticEntity {
        protected override void Start() {
            base.Start();
            ExtractionCoil.OnCoilBuilt += OnCoilBuilt;
        }

        private void OnDestroy() {
            ExtractionCoil.OnCoilBuilt -= OnCoilBuilt;
        }

        private void OnCoilBuilt(ExtractionCoil coil) => coil.OnWreckageFallen(this);
    }
}
