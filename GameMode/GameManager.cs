using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const float GAME_TIME = 8 * 60;
	private float timeSpent;
	private Coroutine countDownCoroutine = null;

	private bool gameStarted = false;

	private void Start()
	{
		timeSpent = 0;
        countDownCoroutine = StartCoroutine(CountDownToStart(5f));
	}

	private void Update()
	{
		if (gameStarted)
		{
			timeSpent += Time.deltaTime;
			EventController.FireEvent(new GameTimeMessage(GAME_TIME, timeSpent, GAME_TIME - timeSpent));

			if(timeSpent >= GAME_TIME)
			{
				EndGame();
			}
		}
	}

	private void EndGame()
	{
		gameStarted = false;
		//Make sure the time displays zero
		EventController.FireEvent(new GameTimeMessage(GAME_TIME, timeSpent, 0));
		EventController.FireEvent(new EndGameMessage());
	}

	protected IEnumerator CountDownToStart(float totalTime)
	{
		
		float time = 0;

		while (time != totalTime)
		{
            EventController.FireEvent(new GameTimeMessage(GAME_TIME, timeSpent, totalTime - time));

			time = Mathf.Min(time + Time.deltaTime, totalTime);
			yield return null;
			
		}
		EventController.FireEvent(new GameStartMessage());
		gameStarted = true;
	}

}
