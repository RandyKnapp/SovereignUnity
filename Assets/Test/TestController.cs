using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sovereign.Test
{
	public class TestController : MonoBehaviour, IPlayerMessenger
	{
		[SerializeField]
		private InputField commandInputField;
		[SerializeField]
		private Button enterCommandButton;

		private GameManager game;
		private Player player;

		private void Start()
		{
			enterCommandButton.onClick.AddListener(OnEnterCommandButtonClicked);

			game = new GameManager(this);
			player = new Player {
				Id = "0",
				Name = "Test"
			};
		}

		private void OnDestroy()
		{
			enterCommandButton.onClick.RemoveListener(OnEnterCommandButtonClicked);
		}

		private void Update()
		{
			if (Input.GetKeyUp(KeyCode.Return))
			{
				OnEnterCommandButtonClicked();
			}
		}

		private void OnEnterCommandButtonClicked()
		{
			ProcessCommandString(commandInputField.text);
			ClearAndReactivateTextField();
		}

		private void ClearAndReactivateTextField()
		{
			commandInputField.text = "";
			commandInputField.ActivateInputField();
		}

		private void ProcessCommandString(string commandString)
		{
			List<string> args = new List<string>(commandString.Split(' ' ));
			string command = args[0];
			args = args.GetRange(1, args.Count - 1);
			game.HandleCommand(player, command, args);
		}

		public void PostMessageToPlayer(Player player, string message)
		{
			Debug.Log("To " + player.Name + ": " + message);
		}

		public void PostMessageToAllPlayers(string message)
		{
			PostMessageToPlayer(player, message);
		}
	}
}