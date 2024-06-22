using UnityEngine;

namespace VisualEffects
{
    public class ParticleSystemSimulationAdjust : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private float _startSimulationTime = 10.0f;

        private void Reset()
        {
            _particles = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            _particles.Simulate(_startSimulationTime);
            _particles.Play();
        }
    }
}