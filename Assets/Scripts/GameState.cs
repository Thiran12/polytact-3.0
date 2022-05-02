using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public bool StopAiMovement = false;

    public bool InCombat = false;

    public GameObject CombatMenu;

    public GameObject GameOver;

    public GameObject MyArmy;

    public GameObject CurrentEmeny;

    public GameObject LastEnemy;

    public GameObject ArmyStr;

    public float LastEnemyTimeout = 5.0f;

    private float EnemyTimoutTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        var textMesh = ArmyStr.GetComponent<TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.text = string.Format("ARMY STR: {0}", MyArmy.GetComponent<Movescript>().ArmyPower);
        }

        // Stop all movement from the ai if we are in combat.
        StopAiMovement = InCombat;

        // If we are no longer in combat.
        if (!InCombat)
        {
            // If there is current nme and its not the same as the last nme and we are not in combat.
            if (CurrentEmeny != null && LastEnemy != CurrentEmeny)
            {
                // we are in combat.
                InCombat = true;
                CombatMenu.SetActive(true);
                LastEnemy = CurrentEmeny;
            }

            EnemyTimoutTimer += Time.deltaTime;

            // Keep the last known nme as long as the timeout allows.
            if (EnemyTimoutTimer > LastEnemyTimeout)
            {
                // then set it to null, this means we can engage in combat with the last known nme again.
                LastEnemy = null;
                EnemyTimoutTimer = 0f;
            }
        }
    }

    public void SetCurrentEnemy(GameObject currentEnemy)
    {
        if (LastEnemy == CurrentEmeny)
        {
            CurrentEmeny = currentEnemy;
        }
    }

    public void FightCurrentEmeny()
    {
        var myArmy = MyArmy.GetComponent<Movescript>();
        var currentNme = CurrentEmeny.GetComponent<ImprovedAiScript>();
        if (myArmy.ArmyPower > currentNme.ArmyPower)
        {
            var targetValue = myArmy.ArmyPower - currentNme.ArmyPower;
            targetValue = targetValue == 0 ? myArmy.ArmyPower / 2 : targetValue;
            var battleResult = Random.Range(0, myArmy.ArmyPower);
            if (battleResult <= targetValue)
            {
                myArmy.ArmyPower += Random.Range(0, currentNme.ArmyPower);
                currentNme.SetIsDead();
            }
            else
            {
                myArmy.ArmyPower -= currentNme.ArmyPower;
                myArmy.IsFleeing = true;
            }
        }
        else
        {
            var targetValue = currentNme.ArmyPower - myArmy.ArmyPower;
            targetValue = targetValue == 0 ? currentNme.ArmyPower / 2 : targetValue;
            var battleResult = Random.Range(0, currentNme.ArmyPower);
            if (battleResult <= targetValue)
            {
                myArmy.ArmyPower += Random.Range(0, currentNme.ArmyPower);
                currentNme.SetIsDead();
            }
            else
            {
                myArmy.ArmyPower -= currentNme.ArmyPower;
                myArmy.IsFleeing = true;
            }
        }

        CurrentEmeny = null;
        CombatMenu.SetActive(false);
        InCombat = false;
    }

    public void FleeCurrentEmeny()
    {
        var myArmy = MyArmy.GetComponent<Movescript>();
        var currentNme = CurrentEmeny.GetComponent<ImprovedAiScript>();
        myArmy.ArmyPower -= Random.Range(0, currentNme.ArmyPower);
        myArmy.IsFleeing = true;
        currentNme.GetComponent<NavMeshAgent>().SetDestination(currentNme.transform.position);
        CombatMenu.SetActive(false);
        CurrentEmeny = null;
        EnemyTimoutTimer = 0f;
        InCombat = false;
    }

    public void EndGame()
    {
        if (GameOver != null)
        {
            GameOver.SetActive(true);
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
