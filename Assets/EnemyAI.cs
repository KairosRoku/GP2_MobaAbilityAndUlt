using UnityEngine;
using TMPro;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float changeDirectionInterval = 2f;
    public TextMeshPro floatingTextPrefab;  // Assign in Inspector
    public Transform playerCamera;  // Assign the player's camera in Inspector

    private bool isFrozen = false;
    private Color originalColor;
    private Renderer enemyRenderer;
    private TextMeshPro floatingTextInstance;

    private void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        originalColor = enemyRenderer.material.color;

        // Create floating text above the enemy
        floatingTextInstance = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
        floatingTextInstance.transform.localPosition = new Vector3(0, 2, 0);  // Offset above enemy
        floatingTextInstance.gameObject.SetActive(false);  // Hide initially
    }

    private void Update()
    {
        if (!isFrozen)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        // Make the floating text always face the player's camera
        floatingTextInstance.transform.rotation = Quaternion.LookRotation(floatingTextInstance.transform.position - playerCamera.position);
    }

    public void Freeze(float duration)
    {
        isFrozen = true;
        enemyRenderer.material.color = Color.blue;  // Change color to blue
        floatingTextInstance.gameObject.SetActive(true);  // Show timer text
        StartCoroutine(FreezeCountdown(duration));
    }

    public void Unfreeze()
    {
        isFrozen = false;
        enemyRenderer.material.color = originalColor;  // Revert color
        floatingTextInstance.gameObject.SetActive(false);  // Hide text
    }

    IEnumerator FreezeCountdown(float duration)
    {
        float timeRemaining = duration;
        while (timeRemaining > 0)
        {
            floatingTextInstance.text = $"Frozen: {timeRemaining:F1}s";  // Update timer
            yield return null;
            timeRemaining -= Time.deltaTime;
        }
        Unfreeze();
    }
}
