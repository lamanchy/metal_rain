using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSuicider : MonoBehaviour {
    private ParticleSystem particleSystem;
    
    private void Start() {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update() {
        if (particleSystem != null && !particleSystem.IsAlive(true)) {
            Destroy(gameObject);
        }
    }
}
