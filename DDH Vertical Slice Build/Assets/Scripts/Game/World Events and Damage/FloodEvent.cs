using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloodEvent : MonoBehaviour
{
    public float floodChance = 0.1f; // 10% chance for a flood to occur per check
    public float floodDuration = 10f;
    public float floodDamageInterval = 1f; // how often the player takes damage per second
    public int damagePerTick = 5;
    public Image floodScreenOverlay;

    private bool isFlooding = false;
    private float timeSinceLastCheck = 0f;
    public float checkInterval = 30f;

    private PlayerBehavior player;
    public float fadeSpeed = 1f;
    private Coroutine fadeCoroutine;

    void Start()
    {
        player = FindObjectOfType<PlayerBehavior>(); // finds the player in the scene
        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        if (floodScreenOverlay == null)
        {
            Debug.LogError("Flood screen overlay not assigned.");
        }
    }

    void Update()
    {
        if (!isFlooding)
        {
            // check periodically if a flood should start
            timeSinceLastCheck += Time.deltaTime;
            if (timeSinceLastCheck >= checkInterval)
            {
                timeSinceLastCheck = 0f;
                TryStartFlood();
            }
        }
    }

    void TryStartFlood()
    {
        // random chance to start a flood
        if (Random.value <= floodChance)
        {
            StartFlood();
        }
    }

    void StartFlood()
    {
        Debug.Log("Flood has started!");
        isFlooding = true;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeScreen(true)); // fade the screen to blue

        StartCoroutine(FloodCoroutine());
    }

    IEnumerator FloodCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < floodDuration)
        {
            if (player != null)
            {
                player.TakeDamage(damagePerTick); // deal dmg to the player
                Debug.Log($"Player took {damagePerTick} damage from flooding.");
            }

            // wait for the dmg interval before dealing dmg again
            yield return new WaitForSeconds(floodDamageInterval);
            elapsedTime += floodDamageInterval;
        }

        EndFlood();
    }

    void EndFlood()
    {
        Debug.Log("Flood has ended.");
        isFlooding = false;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeScreen(false)); // fade the screen back to normal
    }

    IEnumerator FadeScreen(bool fadeIn)
    {
        Color overlayColor = floodScreenOverlay.color;
        float targetAlpha = fadeIn ? 0.5f : 0f; // 50% blue when the flood is active, 0% when it's over

        while (!Mathf.Approximately(overlayColor.a, targetAlpha))
        {
            overlayColor.a = Mathf.MoveTowards(overlayColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            floodScreenOverlay.color = overlayColor;
            yield return null;
        }
    }
}