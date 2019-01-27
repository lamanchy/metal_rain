using Entities.Buildings;

namespace Entities.Wreckage {
    public class FallenWreckage : StaticEntity {
        protected override void Start() {
            base.Start();
            ExtractionCoil.OnCoilBuilt += OnCoilBuilt;
        }

        protected override void OnDestroy() {
            ExtractionCoil.OnCoilBuilt -= OnCoilBuilt;
        }

        private void OnCoilBuilt(ExtractionCoil coil) => coil.OnWreckageFallen(this, null);
    }
}
