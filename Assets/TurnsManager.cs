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
	private TankAI npcAI;

	[SerializeField]
	private TextMeshProUGUI turnTx, playerTx, phaseTx, endTx;
	[SerializeField]
	private Slider npc_hp, player_hp;

	//if turnPhase = 0 is NPC turn, if turnPhase = 1 is Player turn
	public int turnCount = 1;
	private int turnPhase=0;
	public static TurnsManager instance;

	private TurnState currentTurnState;

	private void Awake()
	{
		instance = this;
		npcAI = npcTank.GetComponent<TankAI>();
	}

	private void Start()
	{
		currentTurnState = TurnState.Movement;
		ContinueTurn();
	}

	public void ContinueTurn()
	{
		if (npcTank.alive && playerTank.alive)
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



			switch (currentTurnState)
			{
				case TurnState.Movement:
					MovementTurn();
					break;

				case TurnState.Attack:
					AttackTurn();
					break;
			}

			player_hp.value = playerTank.HP;
			npc_hp.value = npcTank.HP;
		}
		else
		{
			endTx.transform.parent.gameObject.SetActive(true);

			player_hp.value = playerTank.HP;
			npc_hp.value = npcTank.HP;

			if (!npcTank.alive &&  !playerTank.alive)
			{
				endTx.text = "nobody wins(?)";
			}

			if (!npcTank.alive)
			{
				endTx.text = "You win";
			}
			if (!playerTank.alive)
			{
				endTx.text = "NPC wins";
			}

			npcTank.gameObject.SetActive(false);
			playerTank.gameObject.SetActive(false);

		}

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

		switch(turnPhase) {

			case 0:
				//NPC
				npcTank.canMove = true;
				turnPhase = 1;
				npcAI.Move();
				break;
			case 1:
				//Player
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
				//NPC
				turnPhase = 1;
				npcTank.canAttack = true;
				npcAI.Attack();
				break;
			case 1:
				//Player
				turnPhase = 0;
				currentTurnState = TurnState.Movement;
				turnTx.text = "Turn: " + turnCount.ToString();
				playerTank.canAttack = true;
				break;

		}
	}

}
