using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum ItemType
    {
        Rock,
        Gold
    }

    public ItemType itemType;
    public int amount = 1; // amt to add to player's inventory

    private bool isPlayerInRange = false;
    private PlayerBehavior player; // ref to the player

    private void Start()
    {
        if (itemType == ItemType.Rock)
        {
            amount = Random.Range(1, 5); // random value for rocks
        }
        else if (itemType == ItemType.Gold)
        {
            amount = Random.Range(10, 30); // random value for gold
        }

        //Debug.Log($"{itemType} pickup created with amount: {amount}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = other.GetComponent<PlayerBehavior>();
            //Debug.Log("Player entered pickup range.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null; // clears player reference
            //Debug.Log("Player exited pickup range.");
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F)) // Check for 'F' key
        {
            //Debug.Log("Attempting to collect item...");

            if (player != null)
            {
                switch (itemType)
                {
                    case ItemType.Rock:
                        player.AddAmmo(amount); // add ammo in PlayerBehavior
                        break;
                    case ItemType.Gold:
                        player.AddGold(amount); // add gold in PlayerBehavior
                        break;
                }

                // Update the UI
                GameUI.instance.UpdateAmmoText(); // update ammo UI
                GameUI.instance.UpdateGoldText(player.gold); // update gold UI

                // destroy item after collection
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Player reference is null!");
            }
        }
    }
}
