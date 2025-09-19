using UnityEngine;

public class Predator : MonoBehaviour
{
    [Header("Predator Settings")]
    public float energy = 10;
    public float age = 0;
    public float maxAge = 20;
    public float speed = 1f;
    public float visionRange = 5f;

    [Header("Predator States")]
    public bool isAlive = true;
    public PredatorState currentState = PredatorState.Exploring;

    private Vector3 destination;
    private float h;

    private void Start()
    {
        destination = transform.position;
    }

    public void Simulate(float h)
    {
        if (!isAlive) return;

        this.h = h;

        switch (currentState)
        {
            case PredatorState.Exploring:
                Explore();
                break;
            case PredatorState.SearchingFood:
                SearchFood();
                break;
            case PredatorState.Eating:
                Eat();
                break;
        }

        Move();
        Age();
        CheckState();
    }

    void Explore()
    {
        // Si hay comida a la vista, cambiar de estado
        Bunny nearestBunny = FindNearestBunny();
        if (nearestBunny != null)
        {
            currentState = PredatorState.SearchingFood;
            destination = nearestBunny.transform.position;
            return;
        }

        // Si ya lleg� al destino, elegir uno nuevo
        if (Vector3.Distance(transform.position, destination) < 0.1f)
        {
            SelectNewDestination();
        }
    }

    void SearchFood()
    {
        Bunny nearestBunny = FindNearestBunny();
        if (nearestBunny == null)
        {
            // Si no hay comida, volver a explorar
            currentState = PredatorState.Exploring;
            return;
        }

        destination = nearestBunny.transform.position;

        // Si est� suficientemente cerca, pasar a comer
        if (Vector3.Distance(transform.position, nearestBunny.transform.position) < 0.2f)
        {
            currentState = PredatorState.Eating;
        }
    }

    void Eat()
    {
        Collider2D foodHit = Physics2D.OverlapCircle(transform.position, 0.2f, LayerMask.GetMask("Bunnies"));
        if (foodHit != null)
        {
            Bunny food = foodHit.GetComponent<Bunny>();
            if (food != null)
            {
                energy += food.age;
                Destroy(food.gameObject);
            }
        }

        // Despu�s de comer vuelve a explorar
        currentState = PredatorState.Exploring;
    }

    void Flee()
    {
        SelectNewDestination();
        currentState = PredatorState.Exploring;
    }

    void SelectNewDestination()
    {
        Vector3 direction = new Vector3(
            Random.Range(-visionRange, visionRange),
            Random.Range(-visionRange, visionRange),
            0
        );

        Vector3 targetPoint = transform.position + direction;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, visionRange, LayerMask.GetMask("Obstacles"));

        if (hit.collider != null)
        {
            float offset = transform.localScale.magnitude * 0.5f;
            destination = hit.point - (Vector2)direction.normalized * offset;
        }
        else
        {
            destination = targetPoint;
        }
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            speed * h
        );

        energy -= speed * h;
    }

    void Age()
    {
        age += h;
    }

    void CheckState()
    {
        if (energy <= 0 || age > maxAge)
        {
            isAlive = false;
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(destination, 0.2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, destination);
    }

    Bunny FindNearestBunny()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, visionRange, LayerMask.GetMask("Bunnies"));
        Debug.Log($"Predator {name} encontr� {hits.Length} colliders en su rango");
        Bunny nearest = null;
        float minDist = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            Bunny food = hit.GetComponent<Bunny>();
            if (food != null)
            {
                float dist = Vector2.Distance(transform.position, food.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = food;
                }
            }
        }

        return nearest;
    }
}
