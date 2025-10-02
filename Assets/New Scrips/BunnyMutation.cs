using UnityEngine;

public class BunnyMutation : MonoBehaviour
{
    [Header("Mutation Settings")]
    [Range(0f, 1f)] public float mutationChance = 0.3f; // Probabilidad de mutaci�n
    [Range(0f, 0.5f)] public float mutationStrength = 0.1f; // Qu� tanto var�a un gen

    public void ApplyMutation(Bunny bunny)
    {
        // MUTACI�N EN VELOCIDAD
        if (Random.value < mutationChance)
        {
            float factor = 1f + Random.Range(-mutationStrength, mutationStrength);
            bunny.speed = Mathf.Clamp(bunny.speed * factor, 0.5f, 3f);
        }

        // MUTACI�N EN VISI�N
        if (Random.value < mutationChance)
        {
            float factor = 1f + Random.Range(-mutationStrength, mutationStrength);
            bunny.visionRange = Mathf.Clamp(bunny.visionRange * factor, 2f, 10f);
        }

        // MUTACI�N EN ENERG�A
        if (Random.value < mutationChance)
        {
            float factor = 1f + Random.Range(-mutationStrength, mutationStrength);
            bunny.energy = Mathf.Clamp(bunny.energy * factor, 5f, 20f);
        }

        // MUTACI�N EN EDAD M�XIMA
        if (Random.value < mutationChance)
        {
            float factor = 1f + Random.Range(-mutationStrength, mutationStrength);
            bunny.maxAge = Mathf.Clamp(bunny.maxAge * factor, 10f, 40f);
        }
    }
}