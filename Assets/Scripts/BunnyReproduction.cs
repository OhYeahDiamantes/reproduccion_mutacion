using UnityEngine;

public class BunnyReproduction : MonoBehaviour
{
    private Bunny bunny;
    public GameObject bunnyPrefab; // Prefab del hijo
   
    private float minEnergyToReproduce = 30f;
    private float energyCost = 30f;

    private void Awake()
    {
        bunny = GetComponent<Bunny>();
    }

    public void TryReproduce()
    {
        // Solo hembras
        if (bunny.gender != BunnyGender.Female) return;

        // Cooldown
        if (Time.time - bunny.lastReproductionTime < bunny.reproductionCooldown) return;

        // Busca macho cercano
        Bunny mate = FindNearbyMale();
        if (mate == null) return;

        // Ambos con suficiente energía
        if (bunny.energy < minEnergyToReproduce || mate.energy < minEnergyToReproduce) return;


        // Realiza reproducción
        SpawnOffspring(mate);
        bunny.lastReproductionTime = Time.time;

        // Coste de energía
        bunny.energy -= energyCost;
        mate.energy -= energyCost;
    }

    Bunny FindNearbyMale()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, bunny.visionRange, LayerMask.GetMask("Bunnies"));
        foreach (var h in hits)
        {
            Bunny other = h.GetComponent<Bunny>();
            if (other != null && other != bunny && other.gender == BunnyGender.Male)
            {
                return other;
            }
        }
        return null;
    }

    void SpawnOffspring(Bunny mate)
    {
        int babies = Random.Range(1, 6); // de 1 a 5
        for (int i = 0; i < babies; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * 0.5f;
            pos.z = 0;
            GameObject childObj = Instantiate(bunnyPrefab, pos, Quaternion.identity);

            Bunny child = childObj.GetComponent<Bunny>();
            if (child != null)
            {
                // MITOSIS
                // Mutación aquí
                child.speed = MutateStat(bunny.speed, 0.2f);
                child.visionRange = MutateStat(bunny.visionRange, 0.2f);
                child.maxAge = MutateStat(bunny.maxAge, 0.2f);
            }
        }

        // Coste de energía
        bunny.energy -= 30f;
        mate.energy -= 30f;
    }

    float MutateStat(float parentStat, float mutationRate)
    {
        float factor = Random.Range(1f - mutationRate, 1f + mutationRate);
        return parentStat * factor;
    }
}
