using System.Collections;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public enum TurnState
{
	Movement,
	Attack
}

public class TurnsManager : MonoBehaviour
{
	public Tank npcTank;
	public Tank playerTank;
	public TankAI npcAI;
	private int turnCount = 1;
	private int bestScore = 0;
	private Vector2 npcPrecision;
	private int turnPhase = 0;
	public static TurnsManager instance;
	//if turnPhase = 0 is NPC turn, if turnPhase = 1 is Player turn

	[Header("UI Components")]
	[SerializeField] private TextMeshProUGUI turnTx;
	[SerializeField] private TextMeshProUGUI playerTx;
	[SerializeField] private TextMeshProUGUI phaseTx;
	[SerializeField] private TextMeshProUGUI endTx;
	[SerializeField] private TextMeshProUGUI bestTx;
	[SerializeField] private Slider npc_hp;
	[SerializeField] private Slider npc_lvl;
	[SerializeField] private Slider player_hp;

	private TurnState currentTurnState;

	private void Awake()
	{
		instance = this;
		npcAI = npcTank.GetComponent<TankAI>();
	}

	private void Start()
	{
		LoadSavedData();
		currentTurnState = TurnState.Movement;
		ContinueTurn();
	}

	private void LoadSavedData()
	{
		bestScore = PlayerPrefs.GetInt("bestScore");
		npcPrecision = new Vector2(PlayerPrefs.GetFloat("npcPrecision_M"), PlayerPrefs.GetFloat("npcPrecision_S"));
		npcAI.SetPrecision(npcPrecision);
		bestTx.text = "Best game: " + bestScore + " turns.";

		npc_lvl.value = (npcPrecision.x + npcPrecision.y) / 2; 
	}

	private void SaveData()
	{
		PlayerPrefs.SetInt("bestScore", bestScore);
		PlayerPrefs.SetFloat("npcPrecision_M", npcAI.GetPrecision().x);
		PlayerPrefs.SetFloat("npcPrecision_S", npcAI.GetPrecision().y);

	}

	public void ContinueTurn()
	{
		if (npcTank.alive && playerTank.alive)
		{
			UpdatePhaseTurn();
			player_hp.value = playerTank.HP;
			npc_hp.value = npcTank.HP;
			npc_lvl.value = (npcAI.GetPrecision().x + npcAI.GetPrecision().y ) / 2;

			StartTurnState();
		}
		else
		{
			endTx.transform.parent.gameObject.SetActive(true);
			player_hp.value = playerTank.HP;
			npc_hp.value = npcTank.HP;

			EndGame();

			npcTank.gameObject.SetActive(false);
			playerTank.gameObject.SetActive(false);
		}
	}

	private void UpdatePhaseTurn()
	{
		phaseTx.text = "Phase: " + currentTurnState.ToString();

		switch (turnPhase)
		{
			case 0:
				playerTx.text = "NPC";
				break;
			case 1:
				playerTx.text = "Player";
				break;
		}
	}

	private void StartTurnState()
	{
		switch (currentTurnState)
		{
			case TurnState.Movement:
				MovementTurn();
				break;
			case TurnState.Attack:
				AttackTurn();
				break;
		}
	}

	private void EndGame()
	{
		if (bestScore == 0 || turnCount < bestScore)
		{
			bestScore = turnCount;
			npcAI.UpdatePrecision(true);
		}

		bestTx.text = "Best game: " + bestScore + " turns.";

		endTx.text = "nobody wins(?)";

		if (!npcTank.alive)
		{
			endTx.text = "You win";
			npcAI.UpdatePrecision(true);
			npcAI.UpdatePrecision(true);
		}
		if (!playerTank.alive)
		{
			endTx.text = "NPC wins";
			npcAI.UpdatePrecision(false);
		}

		SaveData();

	}

	public void RestartGame()
	{
		endTx.transform.parent.gameObject.SetActive(false);
		turnCount = 0;
		turnPhase = 0;
		currentTurnState = TurnState.Movement;
		npcTank.ResetTank();
		playerTank.ResetTank();
		player_hp.value = playerTank.HP;
		npc_hp.value = npcTank.HP;
		turnTx.text = "Turn: " + turnCount.ToString();
		ContinueTurn();
	}

	private void MovementTurn()
	{
		switch (turnPhase)
		{
			case 0:
				// NPC turn
				npcTank.canMove = true;
				turnPhase = 1;
				npcAI.Move();
				break;
			case 1:
				// Player turn
				turnPhase = 0;
				currentTurnState = TurnState.Attack;
				playerTank.canMove = true;
				break;
		}
	}

	private void AttackTurn()
	{
		switch (turnPhase)
		{
			case 0:
				// NPC turn
				turnPhase = 1;
				npcTank.canAttack = true;
				npcAI.Attack();
				break;
			case 1:
				// Player turn
				turnPhase = 0;
				currentTurnState = TurnState.Movement;
				turnCount++;
				turnTx.text = "Turn: " + turnCount.ToString();
				playerTank.canAttack = true;
				break;
		}
	}


}
