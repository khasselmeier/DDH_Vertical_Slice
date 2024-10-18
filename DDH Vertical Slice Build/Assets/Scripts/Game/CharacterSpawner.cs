using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject characterOnePrefab;  // Prefab for Moldrock
    public GameObject characterTwoPrefab;  // Prefab for Thalgrim
    public Transform spawnPoint;           // The point where the character will spawn

    void Start()
    {
        // Get the selected character from PlayerPrefs
        string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Moldrock");
        Debug.Log("Selected Character from PlayerPrefs: " + selectedCharacter);

        // Spawn the character based on the selection
        SpawnCharacter(selectedCharacter);
    }

    // Method to spawn the character based on the selected type
    public void SpawnCharacter(string selectedCharacter)
    {
        GameObject selectedPrefab = null;

        // Determine which character to spawn based on the selected character type
        if (selectedCharacter == "Moldrock")
        {
            selectedPrefab = characterOnePrefab;
            Debug.Log("Moldrock prefab selected.");
        }
        else if (selectedCharacter == "Thalgrim")
        {
            selectedPrefab = characterTwoPrefab;
            Debug.Log("Thalgrim prefab selected.");
        }
        else
        {
            Debug.LogError("Unknown character selected.");
        }

        // Spawn the character at the spawn point
        if (selectedPrefab != null && spawnPoint != null)
        {
            GameObject playerInstance = Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);
            playerInstance.tag = "Player"; // makes sure the player is tagged as "Player"
            Debug.Log($"Spawned {selectedCharacter} and tagged as: {playerInstance.tag}");
        }
        else
        {
            if (selectedPrefab == null)
            {
                Debug.LogError("Selected prefab is null!");
            }

            if (spawnPoint == null)
            {
                Debug.LogError("Spawn point is missing!");
            }
        }
    }
}