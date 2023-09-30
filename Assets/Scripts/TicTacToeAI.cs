using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Random = UnityEngine.Random;

public enum TicTacToeState{none, cross, circle}

[System.Serializable]
public struct Moves
{
	public int xMove;
	public int yMove;
}


[System.Serializable]
public enum LevelState
{
	EASY,
	HARD,
}


[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
	
}

public class TicTacToeAI : MonoBehaviour
{

	int _aiLevel;

	TicTacToeState[,] boardState;

	[SerializeField]
	private bool _isPlayerTurn;

	[SerializeField]
	private int _gridSize = 3;
	
	[SerializeField]
	private TicTacToeState playerState = TicTacToeState.cross;
	private TicTacToeState aiState = TicTacToeState.circle;

	[SerializeField]
	private GameObject _xPrefab;

	[SerializeField]
	private GameObject _oPrefab;

	[SerializeField] private TextMeshProUGUI BoardTestUI;

	public UnityEvent onGameStarted;

	[SerializeField] private GameObject RetryPanel;

	//Call This event with the player number to denote the winner
	public WinnerEvent onPlayerWin;

	ClickTrigger[,] _triggers;

	private int[,] Board =  { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

	private int[] _winnerVisuals = { 1, 2 };
	[SerializeField] private List<Moves> easyMoves = new List<Moves>();
	[SerializeField] List<Moves> hardMoves = new List<Moves>();

	
	public LevelState _LevelState = LevelState.EASY;
	
	
	// private area
	private static int _counter = 0;
	private static int _counterZero = 0;
	private static int _tieCounter = 0;
	private void Awake()
	{
		if(onPlayerWin == null){
			onPlayerWin = new WinnerEvent();
		}
	}

	public void StartAI(int AILevel){
		_aiLevel = AILevel;
		if (_aiLevel == 0)
		{
			_LevelState = LevelState.EASY;
		}
		else
		{
			_LevelState = LevelState.HARD;
		}
		StartGame();
	}

	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
	{
		_triggers[myCoordX, myCoordY] = clickTrigger;
	}

	private void StartGame()
	{
		_triggers = new ClickTrigger[_gridSize,_gridSize];
		onGameStarted.Invoke();
		RandomEasyMoves();
	}

	public void PlayerSelects(int coordX, int coordY){
		
		//Debug.LogWarning($"X axis is {coordX.ToString()}  and Y axis is {coordY.ToString()}");
		if (Board[coordX, coordY] == 1 || Board[coordX, coordY] == 2)
		{
			return;
		}
		SetVisual(coordX, coordY, playerState);
		Board[coordX, coordY] = 1;
		CheckWinner();
		_tieCounter++;
	}

	
	// TODO : include check for best move 
	public void AiSelects(Moves aiMoves){
		SetVisual(aiMoves.xMove, aiMoves.yMove, aiState);
		Board[aiMoves.xMove, aiMoves.yMove] = 2;
		CheckWinner();
	}

	
	// random all values in easy moves
	private void RandomEasyMoves()
	{
		Moves tempSwap;
		
		for (int i = 1; i < easyMoves.Count; i++)
		{
			int randomOne = Random.Range(0, easyMoves.Count);
			int randomTwo = Random.Range(0, easyMoves.Count);
			tempSwap = easyMoves[randomOne];
			easyMoves[randomOne] = easyMoves[randomTwo];
			easyMoves[randomTwo] = tempSwap;
		}
		easyMoves.Reverse();
	}

	public Moves AIGeneralMove()
	{
		_tieCounter++;
		if (_LevelState == LevelState.EASY)
		{
			return AIEasyMove();
		}
		else
		{
			return AIHardMove();
		}

		
	}
	private Moves AIEasyMove()
	{
		
		Moves _moves = easyMoves[_counter];
		
		while (Board[_moves.xMove, _moves.yMove] == 1 || Board[_moves.xMove, _moves.yMove] == 2)
		{
			_counter++;
			if (_counter >= 8)
			{
				_counter = 0;
			}
			_moves = easyMoves[_counter];
		}
		
		return _moves;
	}
	
	
	private Moves AIHardMove()
	{
		// check  if player about to win and block the player.
		// if not try winning to perfect moves. 
		Moves _movesOne = new Moves();
		
		for (int i = 0; i < 3; i++)
		{
			if (Board[i, 0] == _winnerVisuals[0] && Board[i, 1] == _winnerVisuals[0])
			{
				// we found a move
				_movesOne.xMove = i;
				_movesOne.yMove = 2;
				if (!((Board[_movesOne.xMove, _movesOne.yMove] == 2 ) || (Board[_movesOne.xMove, _movesOne.yMove] == 1)))
				{
					Debug.LogWarning("Found a stopping move");
					return _movesOne;
				}
			}

			if (Board[i, 1] == _winnerVisuals[0] && Board[i, 2] == _winnerVisuals[0])
			{
				// we have a move
				_movesOne.xMove = i;
				_movesOne.yMove = 0;
				if (!((Board[_movesOne.xMove, _movesOne.yMove] == 2 )  || (Board[_movesOne.xMove, _movesOne.yMove] == 1)))
				{
					return _movesOne;
				}
			}

			if (Board[i, 0] == _winnerVisuals[0] && Board[i, 2] == _winnerVisuals[0])
			{
				// we found a move
				_movesOne.xMove = i;
				_movesOne.yMove = 1;
				if (!((Board[_movesOne.xMove, _movesOne.yMove] == 2 ) ||  (Board[_movesOne.xMove, _movesOne.yMove] == 1)))
				{
					return _movesOne;
				}
			}
		}
		
		for (int i = 0; i < 3; i++)
		{
			if (Board[0, i] == _winnerVisuals[0] && Board[1, i] == _winnerVisuals[0])
			{
				// we found a move
				_movesOne.xMove = 2;
				_movesOne.yMove = i;
				if (!((Board[_movesOne.xMove, _movesOne.yMove] == 2 ) || (Board[_movesOne.xMove, _movesOne.yMove] == 1)))
				{
					Debug.LogWarning("Found a stopping move");
					return _movesOne;
				}
			}

			if (Board[1, i] == _winnerVisuals[0] && Board[2, i] == _winnerVisuals[0])
			{
				// we have a move
				_movesOne.xMove = 0;
				_movesOne.yMove = i;
				if (!((Board[_movesOne.xMove, _movesOne.yMove] == 2 )  || (Board[_movesOne.xMove, _movesOne.yMove] == 1)))
				{
					return _movesOne;
				}
			}

			if (Board[0, i] == _winnerVisuals[0] && Board[2, i] == _winnerVisuals[0])
			{
				// we found a move
				_movesOne.xMove = 1;
				_movesOne.yMove = i;
				if (!((Board[_movesOne.xMove, _movesOne.yMove] == 2 ) ||  (Board[_movesOne.xMove, _movesOne.yMove] == 1)))
				{
					return _movesOne;
				}
			}
		}
		
		_movesOne = DiagonalChecks();
		
		return  _movesOne;
		
		
	}

	public Moves DiagonalChecks()
	{
		Moves _moves;
		if (Board[0, 0] == _winnerVisuals[0] && Board[1, 1] == _winnerVisuals[0])
		{
			// we found a move
			_moves.xMove = 2;
			_moves.yMove = 2;
			if (!((Board[_moves.xMove, _moves.yMove] == 2 )|| (Board[_moves.xMove, _moves.yMove] == 1)))
			{
				return _moves;
			}
		}

		if (Board[0, 0] == _winnerVisuals[0] && Board[2, 2] == _winnerVisuals[0])
		{
			// we found a move
			_moves.xMove = 1;
			_moves.yMove = 1;
			if (!((Board[_moves.xMove, _moves.yMove] == 2 )|| (Board[_moves.xMove, _moves.yMove] == 1)))
			{
				return _moves;
			}
		}

		if (Board[1, 1] == _winnerVisuals[0] && Board[2, 2] == _winnerVisuals[0])
		{
			// we found a move
			_moves.xMove = 0;
			_moves.yMove = 0;
			if (!((Board[_moves.xMove, _moves.yMove] == 2 )|| (Board[_moves.xMove, _moves.yMove] == 1)))
			{
				return _moves;
			}
		}

		if (Board[0, 2] == _winnerVisuals[0] && Board[1, 1] == _winnerVisuals[0])
		{
			// we found a move
			_moves.xMove = 2;
			_moves.yMove = 0;
			if (!((Board[_moves.xMove, _moves.yMove] == 2 )|| (Board[_moves.xMove, _moves.yMove] == 1)))
			{
				return _moves;
			}
		}

		if (Board[0, 2] == _winnerVisuals[0] && Board[2, 0] == _winnerVisuals[0])
		{
			// we found a move
			_moves.xMove = 1;
			_moves.yMove = 1;
			if (!((Board[_moves.xMove, _moves.yMove] == 2 )|| (Board[_moves.xMove, _moves.yMove] == 1)))
			{
				return _moves;
			}
		}

		if (Board[2, 0] == _winnerVisuals[0] && Board[1, 1] == _winnerVisuals[0])
		{
			// we found a move
			_moves.xMove = 0;
			_moves.yMove = 2;
			if (!((Board[_moves.xMove, _moves.yMove] == 2 )|| (Board[_moves.xMove, _moves.yMove] == 1)))
			{
				return _moves;
			}
		}

		return PlayFreeMove();
	}

	private Moves PlayFreeMove()
	{
		Moves _moves = hardMoves[_counterZero];
		while(Board[_moves.xMove, _moves.yMove] == 1 || Board[_moves.xMove, _moves.yMove] == 2)
		{
			_counterZero++;
			if (_counterZero >= 8)
			{
				_counterZero = 0;
				// to check this code latter on again
			}
			_moves = hardMoves[_counterZero];
			
		}
		
		//Debug.LogError($"X move is {_moves.xMove.ToString()}: Y move is {_moves.yMove.ToString()}");
		
		return _moves;
	}

	private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
	{
		Instantiate(
			targetState == TicTacToeState.circle ? _oPrefab : _xPrefab,
			_triggers[coordX, coordY].transform.position,
			Quaternion.identity
		);
	}

	
	// print board to screen to check moves. 
	public void PrintBoard()
	{
		BoardTestUI.text = "";
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				
				BoardTestUI.text += "|" + Board[i, j].ToString() + "|";
			}
			BoardTestUI.text = $"{BoardTestUI.text}\n";
		}
		// TODO : to remove because we dont check the winner here.
	
	}

	public void CheckWinner()
	{
		int count = 0;

		while (count < 2)
		{
			for (int i = 0; i < 3; i++)
			{
				if (Board[i, 0] == _winnerVisuals[count] && Board[i, 1] == _winnerVisuals[count] && Board[i, 2] == _winnerVisuals[count])
				{
				
					WhoWins(count);
					break;
				}

				if (Board[0, i] == _winnerVisuals[count] && Board[1, i] == _winnerVisuals[count] && Board[2, i] == _winnerVisuals[count]){
			
					WhoWins(count);
					break;
				}

				if (Board[0, 0] == _winnerVisuals[count] && Board[1, 1] == _winnerVisuals[count] && Board[2, 2] == _winnerVisuals[count])
				{
			
					WhoWins(count);
					break;
				}

				if (Board[0, 2] == _winnerVisuals[count] && Board[1, 1] == _winnerVisuals[count] && Board[2, 0] == _winnerVisuals[count])
				{
			
					WhoWins(count);
					break;
				}
			}
		
			count++;
		}

		if (_tieCounter >= 8)
		{
			onPlayerWin.Invoke(-1);
			_tieCounter = 0;
		}
		
		
	}

	private void WhoWins(int count)
	{
		if (_winnerVisuals[count] == 1)
		{
			onPlayerWin.Invoke(0);
		}
		else
		{
			onPlayerWin.Invoke(1);
		}
		
		RetryPanel.SetActive(true);
		_tieCounter = 0;

	}
	
}
