using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sovereign;

namespace Sovereign.Test
{
	public class TestController : MonoBehaviour, IPlayerMessenger
	{
		[SerializeField]
		private InputField commandInputField;
		[SerializeField]
		private Button enterCommandButton;
		[SerializeField]
		private Text villageDebugText;

		private GameManager game;
		private Player player;

		private void Start()
		{
			enterCommandButton.onClick.AddListener(OnEnterCommandButtonClicked);

			Commands.Initialize(this);
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

			string output = game.GetDebugString() + "\n\n";

			Village village = game.Villages.GetVillage(player);
			if (village != null)
			{
				output += "Village: " + village.Name + " (Owner: " + village.OwnerPlayer.Name + ")";
				output += "\nPopulation:";
				int maleCount = village.Population.Count(p => p.Sex == "Male");
				int femaleCount = village.Population.Count(p => p.Sex == "Female");
				output += "(Male: " + maleCount + ", Female: " + femaleCount + ")";
				foreach (Person person in village.Population)
				{
					output += "\n  [" + person.Id + "] " + person.Name + " - " + person.Sex + ", " + person.Age + ", " + person.Class;
				}
			}

			villageDebugText.text = output;
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
