using UnityEngine;

public class CriaBehaviour : MonoBehaviour
{
    [Header("Cria Settings")]
    public float velocidad = 2f;
    public float rangoVision = 5f;
    public float tiempoCambioDireccion = 2f;
    public float energia = 50f;

    [Header("Escape y Bordes")]
    public LayerMask depredadorMask;
    public LayerMask obstaculosMask;
    public float distanciaHuir = 6f;

    [Header("Crecimiento")]
    public float tiempoParaAdulto = 30f; // tiempo en segundos para crecer
    public GameObject bunnyPrefab;      // Prefab del conejo adulto

    private Vector3 direccionAleatoria;
    private float temporizador;
    private float edad = 0f; // contador de tiempo de vida como cría

    void Start()
    {
        CambiarDireccionAleatoria();
    }

    void Update()
    {
        //  crecer con el tiempo
        edad += Time.deltaTime;
        if (edad >= tiempoParaAdulto)
        {
            ConvertirseEnAdulto();
            return;
        }

        // 1. Si hay depredador cerca -> huir
        if (DepredadorCerca())
        {
            Huir();
            return;
        }

        // 2. Buscar comida cercana
        GameObject comida = BuscarComidaCercana();
        if (comida != null)
        {
            Vector3 direccion = (comida.transform.position - transform.position).normalized;
            Mover(direccion, velocidad);

            if (Vector3.Distance(transform.position, comida.transform.position) < 0.2f)
            {
                Comer(comida);
            }
        }
        else
        {
            Mover(direccionAleatoria, velocidad * 0.5f);

            temporizador -= Time.deltaTime;
            if (temporizador <= 0)
            {
                CambiarDireccionAleatoria();
            }
        }
    }

    bool DepredadorCerca()
    {
        Collider2D predator = Physics2D.OverlapCircle(transform.position, rangoVision, depredadorMask);
        return predator != null;
    }

    void Huir()
    {
        Collider2D[] predators = Physics2D.OverlapCircleAll(transform.position, rangoVision, depredadorMask);
        if (predators.Length == 0) return;

        Vector3 predatorPos = predators[0].transform.position;
        float minDist = Mathf.Infinity;
        foreach (var p in predators)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                predatorPos = p.transform.position;
            }
        }

        Vector3 fleeDir = (transform.position - predatorPos).normalized;
        Mover(fleeDir, velocidad * 1.5f);
    }

    GameObject BuscarComidaCercana()
    {
        GameObject[] comidas = GameObject.FindGameObjectsWithTag("Comida");
        GameObject objetivoMasCercano = null;
        float distanciaMin = Mathf.Infinity;

        foreach (GameObject c in comidas)
        {
            float distancia = Vector3.Distance(transform.position, c.transform.position);
            if (distancia < rangoVision && distancia < distanciaMin)
            {
                distanciaMin = distancia;
                objetivoMasCercano = c;
            }
        }
        return objetivoMasCercano;
    }

    void Comer(GameObject comida)
    {
        Food food = comida.GetComponent<Food>();
        if (food != null)
        {
            energia += food.nutrition;
            Destroy(comida);
            Debug.Log("La cria comió y ahora tiene energia: " + energia);
        }
    }

    void Mover(Vector3 direccion, float velocidadActual)
    {
        Vector3 nuevoDestino = transform.position + direccion * velocidadActual * Time.deltaTime;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direccion, 0.5f, obstaculosMask);
        if (hit.collider != null)
        {
            CambiarDireccionAleatoria();
        }
        else
        {
            transform.position = nuevoDestino;
        }
    }

    void CambiarDireccionAleatoria()
    {
        direccionAleatoria = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
        temporizador = tiempoCambioDireccion;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoVision);
    }

    // convertir cría en adulto
    void ConvertirseEnAdulto()
    {
        if (bunnyPrefab == null)
        {
            Debug.LogWarning(" No se asignó el prefab de Bunny en CriaBehaviour!");
            return;
        }

        GameObject adulto = Instantiate(bunnyPrefab, transform.position, Quaternion.identity);

        // Pasar energía inicial
        Bunny adultoScript = adulto.GetComponent<Bunny>();
        if (adultoScript != null)
        {
            adultoScript.energy = this.energia;
        }
        //Estoy sufriendo aAaaA
        Destroy(this.gameObject); // eliminar la cría
        Debug.Log("La cría se convirtió en un Bunny!");
    }
}