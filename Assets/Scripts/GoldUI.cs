using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    private PlayerWallet playerWallet;

    private void Start()
    {
        playerWallet = PlayerWallet.Instance;

        if (playerWallet == null)
        {
            Debug.LogWarning("PlayerWalletÀÌ ¾À¿¡ ¾ø½À´Ï´Ù.");
            return;
        }

        playerWallet.GoldChanged += UpdateGoldText;

        UpdateGoldText(playerWallet.Gold);
    }

    private void OnDestroy()
    {
        if (playerWallet != null)
        {
            playerWallet.GoldChanged -= UpdateGoldText;
        }
    }

    private void UpdateGoldText(int currentGold)
    {
        if (goldText == null)
            return;

        goldText.text = $"°ñµå: {currentGold}";
    }
}
