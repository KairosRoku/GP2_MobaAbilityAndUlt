using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerAbilities : MonoBehaviour
{
    public GameObject iceWallPrefab;
    public TextMeshPro floatingTextPrefab;  // Assign in Inspector
    public Transform playerCamera;  // Assign the player's camera in Inspector
    public float wallDistance = 2f;
    public float wallDuration = 10f;
    public float freezeRadius = 5f;
    public float freezeDuration = 3f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CreateIceWall();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            FreezeEnemies();
        }
    }

    void CreateIceWall()
    {
        Vector3 spawnPosition = transform.position + transform.forward * wallDistance;

        // Rotate the wall so the flat side faces the player
        Quaternion wallRotation = Quaternion.LookRotation(transform.right);

        GameObject iceWall = Instantiate(iceWallPrefab, spawnPosition, wallRotation);

        // Add floating TextMeshPro counter above the wall
        TextMeshPro wallText = Instantiate(floatingTextPrefab, iceWall.transform.position, Quaternion.identity, iceWall.transform);
        wallText.transform.localPosition = new Vector3(0, 2, 0);  // Offset above the wall

        // Start countdown and destroy the wall after duration
        StartCoroutine(WallCountdown(wallText, iceWall, wallDuration));

        // Make the text always face the player
        StartCoroutine(BillboardText(wallText));
    }

    IEnumerator WallCountdown(TextMeshPro wallText, GameObject iceWall, float duration)
    {
        float timeRemaining = duration;
        while (timeRemaining > 0)
        {
            wallText.text = $"Wall: {timeRemaining:F1}s";  // Update timer
            yield return null;
            timeRemaining -= Time.deltaTime;
        }
        Destroy(iceWall);  // Destroy the wall
    }

    void FreezeEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, freezeRadius);
        foreach (Collider enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                StartCoroutine(FreezeEnemy(enemy.gameObject));
            }
        }
    }

    IEnumerator FreezeEnemy(GameObject enemy)
    {
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.Freeze(freezeDuration);
        }
        yield return new WaitForSeconds(freezeDuration);
        if (enemyAI != null)
        {
            enemyAI.Unfreeze();
        }
    }

    IEnumerator BillboardText(TextMeshPro textMesh)
    {
        while (textMesh != null)
        {
            // Rotate the text to always face the player's camera
            textMesh.transform.rotation = Quaternion.LookRotation(textMesh.transform.position - playerCamera.position);
            yield return null;
        }
    }
}
