using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]

public class BallController : MonoBehaviour
{
    [Header("Power Settings")]
    public float power = 10f;
    public float maxDragDistance = 3f;

    [Header("Shoot Effects")]
    public GameObject shootEffect;
    public AudioClip shootSound;

    [Header("Hole Effects")]
    public GameObject holeEffect;
    public AudioClip holeSound;

    //„ €?— Â«? „—»Êÿ »Â linrender
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;
    private Vector2 dragStart;
    private Vector2 dragEnd;
    private bool isDragging = false;
    public ShotManager shotManager;
    public bool levelCompleted = false;
    private Vector3 startPosition;
    private Quaternion startRotation;

    [Header("Water Effects")]
    public GameObject waterEffect;
    public AudioClip waterSound;

    private AudioSource audioSource;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.03f;

        startPosition = transform.position;
        startRotation = transform.rotation;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        
        if (rb.velocity.magnitude > 0.05f)
            return;


        if (Input.GetMouseButtonDown(0))
        {
            if (shotManager != null && !shotManager.HasShot())
                return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(mousePos, transform.position) < 0.6f)
            {
                dragStart = mousePos;
                isDragging = true;
                lineRenderer.enabled = true;
            }
        }


        if (isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 direction = dragStart - mousePos;
            direction = Vector2.ClampMagnitude(direction, maxDragDistance);

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, (Vector2)transform.position + direction);
        }

        
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            dragEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 force = dragStart - dragEnd;
            force = Vector2.ClampMagnitude(force, maxDragDistance);

            if (force.magnitude > 0.1f)
            {
                rb.AddForce(force * power, ForceMode2D.Impulse);

                if (shotManager != null)
                    shotManager.UseShot();

                if (shootEffect != null)
                {
                    Instantiate(shootEffect, rb.position, Quaternion.identity);
                }


                if (shootSound != null)
                    audioSource.PlayOneShot(shootSound);
            }

            
            isDragging = false;
            lineRenderer.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            StartCoroutine(ResetBall());
            return;
        }

        if (other.CompareTag("Hole"))
        {
            StartCoroutine(FallIntoHole(other.transform));
        }
    }

    private IEnumerator FallIntoHole(Transform hole)
    {
        
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;

        Vector3 startPos = transform.position;
        Vector3 endPos = hole.position;

        Vector3 startScale = transform.localScale;

        float duration = 0.6f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float p = Mathf.SmoothStep(0f, 1f, t / duration);

            transform.position = Vector3.Lerp(startPos, endPos, p);

            float scale;

            if (p < 0.75f)
            {
                
                scale = Mathf.Lerp(1f, 0.9f, p / 0.75f);
            }
            else
            {
                
                scale = Mathf.Lerp(0.9f, 0.35f, (p - 0.75f) / 0.25f);
            }

            transform.localScale = startScale * scale;

           
            transform.Rotate(0f, 0f, 360f * Time.deltaTime);

            yield return null;
        }

        yield return new WaitForSeconds(0.15f);


        if (holeEffect != null)
        {
            Instantiate(holeEffect, hole.position, Quaternion.identity);
        }

        if (holeSound != null)
        {
            audioSource.PlayOneShot(holeSound);
        }

        levelCompleted = true;

        yield return new WaitForSeconds(1f);

        int currentScene = SceneManager.GetActiveScene().buildIndex;

        int baseScore = 100 * currentScene;

        int bonus = 0;

        if (shotManager != null)
        {
            bonus = shotManager.GetRemainingShots() * 50;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(baseScore + bonus);
        }

        if (currentScene < 3)
        {
            SceneManager.LoadScene(currentScene + 1);
        }
        else
        {
            SceneManager.LoadScene("Win");
        }
    }


    private IEnumerator ResetBall()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0f, -0.15f, 0f);

        Vector3 originalScale = transform.localScale;

        float duration = 0.8f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float p = Mathf.SmoothStep(0f, 1f, t / duration);

            transform.position = Vector3.Lerp(startPos, endPos, p);

            float scale = Mathf.Lerp(1f, 0.45f, p);
            transform.localScale = originalScale * scale;

            transform.Rotate(0f, 0f, 220f * Time.deltaTime);

            yield return null;
        }

        if (waterEffect != null)
        {
            Instantiate(waterEffect, transform.position, Quaternion.identity);
        }

        
        if (waterSound != null)
        {
            audioSource.PlayOneShot(waterSound);
        }

        GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(1.3f);

        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.localScale = originalScale;

        GetComponent<SpriteRenderer>().enabled = true;

        rb.simulated = true;
    }
}