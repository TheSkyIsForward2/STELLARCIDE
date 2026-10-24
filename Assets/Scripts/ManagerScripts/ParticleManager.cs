using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    ParticleSystem[] particles;
    
    private ParticleSystem damageParticleInstance;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particles = Resources.LoadAll<ParticleSystem>("Prefabs/VFX Prefabs/Particles");
        GameManager.Instance.ParticleManager = this;
    }

    public void SpawnParticle(Transform transform, int index, Vector2 attackDirection)
    {
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.right, attackDirection);
        
        damageParticleInstance = Instantiate(particles[index], transform.position, spawnRotation);
    }
}
