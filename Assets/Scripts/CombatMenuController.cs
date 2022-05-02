using TMPro;
using UnityEngine;

public class CombatMenuController : MonoBehaviour
{
    public GameObject GameController;

    public GameObject ArmyStr;

    public GameObject NmeArmyStr;

    // Start is called before the first frame update
    void Start()
    {
        GameController = GameObject.Find("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController != null)
        {
            var gameState = GameController.GetComponent<GameState>();
            if (gameState != null)
            {
                var textMesh = ArmyStr.GetComponent<TextMeshProUGUI>();
                if (textMesh != null)
                {
                    textMesh.text = string.Format("YOUR ARMY: {0}", gameState.MyArmy.GetComponent<Movescript>().ArmyPower);
                }

                var nmeTextMesh = NmeArmyStr.GetComponent<TextMeshProUGUI>();
                if (nmeTextMesh != null)
                {
                    nmeTextMesh.text = string.Format("ENEMY ARMY: {0}", gameState.CurrentEmeny.GetComponent<ImprovedAiScript>().ArmyPower);
                }
            }
        }
    }
}
