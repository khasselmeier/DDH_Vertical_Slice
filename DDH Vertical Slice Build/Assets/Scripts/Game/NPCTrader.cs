using UnityEngine;
using TMPro;

public class NPCTrader : MonoBehaviour
{
    public int upgradeCostBaseGem = 50;
    public int upgradeCostHighGem = 100;
    public int damageIncrease = 5;
    public TextMeshProUGUI tradePromptText;

    private PlayerBehavior player; 
    private bool isPlayerInRange = false;
    private bool hasTradedBaseGem = false;
    private bool hasTradedHighGem = false;

    private void Start()
    {
        // trade prompt is hidden at the start
        tradePromptText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerBehavior>();
            isPlayerInRange = true;

            // update trade prompt based on player's trade status
            if (!hasTradedBaseGem)
            {
                tradePromptText.gameObject.SetActive(true);
                tradePromptText.text = "Trade 50 gold for a pickaxe to mine gems";
            }
            else if (hasTradedBaseGem && !hasTradedHighGem)
            {
                tradePromptText.gameObject.SetActive(true);
                tradePromptText.text = "Having trouble mining some gems? I can fix that for 100 gold";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null;

            // hide trade prompt UI when the player leaves the range
            tradePromptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInRange && player != null && Input.GetKeyDown(KeyCode.F))
        {
            TradeWithPlayer(player);
        }
    }

    private void TradeWithPlayer(PlayerBehavior player)
    {
        // trading for base gems
        if (!hasTradedBaseGem && player.gold >= upgradeCostBaseGem)
        {
            player.gold -= upgradeCostBaseGem;
            //player.rocks.damage += damageIncrease; //increase rock dmg
            player.canMineBaseGem = true;
            hasTradedBaseGem = true;
            player.hasTraded = true;
            tradePromptText.gameObject.SetActive(false);

            Debug.Log("Player traded for ability to mine base gems");
            //Debug.Log($"Current state: hasTradedBaseGem={hasTradedBaseGem}, canMineBaseGem={player.canMineBaseGem}");

            // Update the UI text for future trades
            tradePromptText.gameObject.SetActive(true);
            tradePromptText.text = "Having trouble mining some gems? I can fix that for 100 gold";
        }
        // trading for high gems
        else if (hasTradedBaseGem && !hasTradedHighGem && player.gold >= upgradeCostHighGem)
        {
            player.gold -= upgradeCostHighGem;
            //player.rocks.damage += damageIncrease;
            player.canMineHighGem = true;
            hasTradedHighGem = true;
            player.hasTraded = true;
            tradePromptText.gameObject.SetActive(false);

            Debug.Log("Player traded for ability to mine high gems");
            //Debug.Log($"Current state: hasTradedHighGem={hasTradedHighGem}, canMineHighGem={player.canMineHighGem}");
        }
        else
        {
            Debug.Log("Not enough gold for the upgrade.");
        }

        GameUI.instance.UpdateGoldText(player.gold); // update gold UI
    }
}