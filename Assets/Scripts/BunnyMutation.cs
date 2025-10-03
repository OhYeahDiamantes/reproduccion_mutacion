using UnityEngine;

public class BunnyMutation : MonoBehaviour
{
    [Header("Mutation Settings")]
    [Range(0f, 1f)] public float mutationChance = 0.3f; // Probabilidad de mutación
    [Range(0f, 0.5f)] public float mutationStrength = 0.1f; // Qué tanto varía un gen

    public void ApplyMutation(Bunny bunny)
    {
        // MUTACIÓN EN VELOCIDAD
        if (Random.value < mutationChance)
        {
            float factor = 1f + Random.Range(-mutationStrength, mutationStrength);
            bunny.speed = Mathf.Clamp(bunny.speed * factor, 0.5f, 3f);
        }

        // MUTACIÓN EN VISIÓN
        if (Random.value < mutationChance)
        {
            float factor = 1f + Random.Range(-mutationStrength, mutationStrength);
            bunny.visionRange = Mathf.Clamp(bunny.visionRange * factor, 2f, 10f);
        }

        // MUTACIÓN EN ENERGÍA
        if (Random.value < mutationChance)
        {
            float factor = 1f + Random.Range(-mutationStrength, mutationStrength);
            bunny.energy = Mathf.Clamp(bunny.energy * factor, 5f, 20f);
        }

        // MUTACIÓN EN EDAD MÁXIMA
        if (Random.value < mutationChance)
        {
            float factor = 1f + Random.Range(-mutationStrength, mutationStrength);
            bunny.maxAge = Mathf.Clamp(bunny.maxAge * factor, 10f, 40f);
        }
    }
}