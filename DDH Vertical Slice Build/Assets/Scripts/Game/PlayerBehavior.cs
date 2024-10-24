using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public delegate void OnPlayerInitialized(PlayerBehavior player);
    public static event OnPlayerInitialized PlayerInitialized; // event for initializing player

    [Header("Stats")]
    public float moveSpeed;
    public float jumpForce;
    public float distanceToGround = 0.1f;
    public int maxHealth = 100;
    public int currentHealth;
    public int npcHitDamage = 50;

    public LayerMask groundLayer;

    [Header("Components")]
    public Rigidbody rig;
    public MeshRenderer mr;
    public PlayerRocks rocks;
    public GameObject pickaxe;

    [Header("Pickups")]
    public int gold = 0;
    public bool hasTraded = false; // tracks if the player has traded with the NPC
    public bool canMineBaseGem = false; // player must trade once to mine these gems
    public bool canMineHighGem = false; // player must trade twice to mine these gems
    public static int collectedGems = 0;
    public int totalValueOfGems = 0; // quota collected
    private int tradeCount = 0; // track number of trades completed

    private void Start()
    {
        currentHealth = maxHealth; // initialize player health

        if (pickaxe != null)
        {
            var pickaxeRenderer = pickaxe.GetComponent<MeshRenderer>();
            if (pickaxeRenderer != null)
            {
                pickaxeRenderer.enabled = false;
            }

            pickaxe.SetActive(false); // Disable pickaxe at the start

            var pickaxeCollider = pickaxe.GetComponent<Collider>();
            if (pickaxeCollider != null)
            {
                pickaxeCollider.enabled = false;
            }
        }
        else
        {
            Debug.LogError("Pickaxe GameObject is not assigned.");
        }

        PlayerInitialized?.Invoke(this); // notify that player is initialized
    }

    public void Initialize()
    {
        GameUI.instance.Initialize();
        currentHealth = maxHealth; // give player max health at the start
        GameUI.instance.UpdateHealthText(currentHealth, maxHealth); // update health ui
    }

    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();

        if (Input.GetMouseButtonDown(1)) // right mouse button down (button index 1)
            rocks.TryShoot();

        if (Input.GetMouseButtonDown(0)) // left mouse button down (button index 0)
            TryCollectGem();
    }

    void Move()
    {
        // get the input axis
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // calculate a direction relative to where we're facing
        Vector3 dir = (transform.forward * z + transform.right * x) * moveSpeed;
        dir.y = rig.velocity.y;

        // set that as our velocity
        rig.velocity = dir;
    }

    void TryJump()
    {
        // create a ray facing down
        Ray ray = new Ray(transform.position, Vector3.down);

        // shoot the raycast
        if (Physics.Raycast(ray, 1.0f))
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        //Debug.Log("Jumping");
    }

    public void TryCollectGem()
    {
        float detectionRange = 10f; //detection range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange); //get colliders within range

        foreach (var hitCollider in hitColliders)
        {
            GameObject gem = hitCollider.gameObject;

            if (gem.CompareTag("BaseGem") && canMineBaseGem)
            {
                CollectGem(gem, "BaseGem");
                return;
            }
            else if (gem.CompareTag("HighGem") && canMineHighGem)
            {
                CollectGem(gem, "HighGem");
                return;
            }
        }
        //Debug.Log("No collectible gems in range or requirements not met");
    }

    public void CollectGem(GameObject gem, string gemType)
    {
        GemPickup gemPickup = gem.GetComponent<GemPickup>();
        if (gemPickup != null)
        {
            totalValueOfGems += gemPickup.gemValue;
            Destroy(gem);
            collectedGems += 1;

            Debug.Log($"Player mined a {gemType}. Total collected: {collectedGems}, Total Value: {totalValueOfGems}");

            // update UI
            GameUI.instance.UpdateGemsValueText(totalValueOfGems);
        }
    }

    public void PerformTrade()
    {
        tradeCount++;

        if (tradeCount == 1)
        {
            hasTraded = true;
            canMineBaseGem = true;

            // Display the pickaxe after the first trade
            if (pickaxe != null)
            {
                pickaxe.SetActive(true); // Activate the pickaxe GameObject

                var pickaxeRenderer = pickaxe.GetComponent<MeshRenderer>();
                if (pickaxeRenderer != null)
                {
                    pickaxeRenderer.enabled = true; // Enable the MeshRenderer
                    Debug.Log("Pickaxe renderer enabled.");
                }
                else
                {
                    Debug.LogError("MeshRenderer not found on the pickaxe.");
                }

                // Enable the collider if necessary
                var pickaxeCollider = pickaxe.GetComponent<Collider>();
                if (pickaxeCollider != null)
                {
                    pickaxeCollider.enabled = true; // Enable the collider if needed
                    Debug.Log("Pickaxe collider enabled.");
                }
            }
            else
            {
                Debug.LogError("Pickaxe GameObject is not assigned.");
            }

            Debug.Log("First trade complete. Player can now mine 'Base Gems'");
        }
        else if (tradeCount == 2)
        {
            canMineHighGem = true;
            Debug.Log("Second trade complete. Player can now mine 'High Gems'");
        }
    }

    public void ChangeAmmo(int amount)
    {
        // change ammo
        rocks.curAmmo += amount;

        // ammo doesn't exceed max
        rocks.curAmmo = Mathf.Clamp(rocks.curAmmo, 0, rocks.maxAmmo);

        // update UI
        GameUI.instance.UpdateAmmoText();
    }

    public void AddAmmo(int amount)
    {
        rocks.curAmmo = Mathf.Clamp(rocks.curAmmo + amount, 0, rocks.maxAmmo);
        //Debug.Log("Ammo added: " + amount + ". Current ammo: " + rocks.curAmmo);
    }

    public void AddGold(int amount)
    {
        gold += amount;
        //Debug.Log("Gold added: " + amount + ". Total gold: " + gold);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        GameUI.instance.UpdateHealthText(currentHealth, maxHealth); // update health UI
        //Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth);
        GameUI.instance.UpdateHealthText(currentHealth, maxHealth); // update health UI
        //Debug.Log("Player healed: " + healAmount + ". Current health: " + currentHealth);
    }

    private void Die()
    {
        //Debug.Log("Player has died.");
        GameManager.instance.LoseGame();
    }

    public void OnNpcHit()
    {
        Debug.Log("The NPC doesn't enjoy being hit");
        TakeDamage(npcHitDamage); // player takes damage when hitting the NPC
    }
}