using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CaltropSlowEffect : MonoBehaviour {

    private ParticleSystem m_System;
    private ParticleSystem.Particle[] m_Particles;
    private int numParticlesAlive;
    private float finishLifeTime = 0f;

    [SerializeField] private float minDistancetoCollision = 0f;

    private void LateUpdate()
    {
        InitializeIfNeeded();

        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        numParticlesAlive = m_System.GetParticles(m_Particles);

        // Apply the particle changes to the particle system
        m_System.SetParticles(m_Particles, numParticlesAlive);
    }

    private void InitializeIfNeeded()
    {
        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
    }

    void OnParticleCollision(GameObject other) {
        
        InitializeIfNeeded();

        switch (other.gameObject.layer) {

            case 10: //Fall off

                // Change only the particles that are alive
                for (int i = 0; i < numParticlesAlive; i++) {

                    //getting the position of the player and the particle
                    Vector3 dis = m_Particles[i].position - other.transform.position;

                    //if it is near
                    if (dis.magnitude <= minDistancetoCollision) {

                        m_Particles[i].remainingLifetime = finishLifeTime;

                    }

                }

                // Apply the particle changes to the particle system
                m_System.SetParticles(m_Particles, numParticlesAlive);

                break;

        }

        if (other.gameObject.CompareTag(Global.gameObjectTag_Player)) {

            // Change only the particles that are alive
            for (int i = 0; i < numParticlesAlive; i++) {

                //getting the position of the player and the particle
                Vector3 dis = m_Particles[i].position - other.transform.position;

                //if it is near
                if (dis.magnitude <= minDistancetoCollision) {

                    m_Particles[i].remainingLifetime = finishLifeTime;
                    other.GetComponent<Player>().caltropDetectedEvent();

                }

                // Apply the particle changes to the particle system
                m_System.SetParticles(m_Particles, numParticlesAlive);

            }
        }
        else if (other.gameObject.CompareTag(Global.gameObjectTag_PlayerSmoked)) {

            // Change only the particles that are alive
            for (int i = 0; i < numParticlesAlive; i++) {

                //getting the position of the player and the particle
                Vector3 dis = m_Particles[i].position - other.transform.position;

                //if it is near
                if (dis.magnitude <= minDistancetoCollision) {

                    m_Particles[i].remainingLifetime = finishLifeTime;
                    other.GetComponent<Player>().caltropDetectedEvent();

                }

                // Apply the particle changes to the particle system
                m_System.SetParticles(m_Particles, numParticlesAlive);

            }

        }
        
        

    }

}
