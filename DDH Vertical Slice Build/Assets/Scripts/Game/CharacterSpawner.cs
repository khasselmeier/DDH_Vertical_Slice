using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject characterOnePrefab;  // Prefab for Character One
    public GameObject characterTwoPrefab;  // Prefab for Character Two
    public Transform spawnPoint;           // The point where the character will spawn

    void Start()
    {
        // Get the selected character from PlayerPrefs
        string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Character One");
        Debug.Log("Selected Character from PlayerPrefs: " + selectedCharacter);

        // Spawn the character based on the selection
        SpawnCharacter(selectedCharacter);
    }

    // Method to spawn the character based on the selected type
    public void SpawnCharacter(string selectedCharacter)
    {
        GameObject selectedPrefab = null;

        // Determine which character to spawn based on the selected character type
        if (selectedCharacter == "Character One")
        {
            selectedPrefab = characterOnePrefab;
            Debug.Log("Character One prefab selected.");
        }
        else if (selectedCharacter == "Character Two")
        {
            selectedPrefab = characterTwoPrefab;
            Debug.Log("Character Two prefab selected.");
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