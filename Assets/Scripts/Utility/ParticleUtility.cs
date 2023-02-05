using System;
using UnityEngine;

namespace TetrisClone.Utility
{
    public class ParticleUtility : MonoBehaviour
    {
        public ParticleSystem[] allParticleSystems;

        private void Start()
        {
            allParticleSystems = GetComponentsInChildren<ParticleSystem>();
        }

        public void PlayParticles()
        {
            foreach (var particles in allParticleSystems)
            {
                particles.Stop();
                particles.Play();
            }
        }
    }
}
