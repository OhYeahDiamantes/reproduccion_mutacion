using UnityEngine;

public class CriaBehaviour : MonoBehaviour
{
    [Header("Cria Settings")]
    public float velocidad = 2f;              // Velocidad de movimiento
    public float rangoVision = 5f;            // Distancia para detectar comida
    public float tiempoCambioDireccion = 2f;  // Cada cuanto cambia direccion si no hay comida
    public float energia = 50f;               // Energia de la cria (se llena al comer)

    [Header("Escape y Bordes")]
    public LayerMask depredadorMask;          // Asigna en el inspector la capa de "Foxes"
    public LayerMask obstaculosMask;          // Asigna en el inspector la capa de paredes
    public float distanciaHuir = 6f;          // Distancia de escape cuando ve un depredador

    private Vector3 direccionAleatoria;
    private float temporizador;

    void Start()
    {
        // Direccion inicial aleatoria
        CambiarDireccionAleatoria();
    }

    void Update()
    {
        // 1. Si hay depredador cerca -> huir
        if (DepredadorCerca())
        {
            Huir();
            return;
        }
        //A COMER WE SI SI
        // 2. Buscar comida cercana
        GameObject comida = BuscarComidaCercana();

        if (comida != null)
        {
            // Moverse hacia la comida
            Vector3 direccion = (comida.transform.position - transform.position).normalized;
            Mover(direccion, velocidad);

            // Revisar si ya esta encima de la comida
            if (Vector3.Distance(transform.position, comida.transform.position) < 0.2f)
            {
                Comer(comida);
            }
        }
        else
        {
            //Como que no hay comia :c
            // Movimiento aleatorio si no hay comida
            Mover(direccionAleatoria, velocidad * 0.5f);

            temporizador -= Time.deltaTime;
            if (temporizador <= 0)
            {
                CambiarDireccionAleatoria();
            }
        }
    }

    // Detector de depredador
    //Si tu quieres bailar, sopa de caracooool
    bool DepredadorCerca()
    {
        Collider2D predator = Physics2D.OverlapCircle(transform.position, rangoVision, depredadorMask);
        return predator != null;
    }
    
    //Guatameguiconsu yupi pa ti yupi pa ti
    void Huir()
    {
        // Buscar depredador mas cercano
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

        // Direccion contraria
        Vector3 fleeDir = (transform.position - predatorPos).normalized;
        Mover(fleeDir, velocidad * 1.5f); // huye un poco mas rapido
    }

    // Detectar comida
    //Me acabo de tomar una Coca-Cola de café para q rinda, hagan sus preguntas
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
            Debug.Log("La cria comio y ahora tiene energia: " + energia);
        }
    }
    //Soy Franchesco Virgolini FIUUUUM
    // Movimiento y obstaculos
    void Mover(Vector3 direccion, float velocidadActual)
    {
        Vector3 nuevoDestino = transform.position + direccion * velocidadActual * Time.deltaTime;

        // Chequear paredes
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direccion, 0.5f, obstaculosMask);
        if (hit.collider != null)
        {
            // Rebote simple - elige nueva direccion aleatoria
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
        Gizmos.DrawWireSphere(transform.position, rangoVision); // vision de depredador
    }
}