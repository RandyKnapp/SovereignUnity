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
				output += "\nPopulation: " + village.Population.Count;
				int maleCount = village.Population.Count(p => p.Sex == "Male" && !p.IsChild);
				int femaleCount = village.Population.Count(p => p.Sex == "Female" && !p.IsChild);
				int maleChildCount = village.Population.Count(p => p.Sex == "Male" && p.IsChild);
				int femaleChildCount = village.Population.Count(p => p.Sex == "Female" && p.IsChild);
				output += " (Adults - M: " + maleCount + ", F: " + femaleCount + " / Children - M: " + maleChildCount + ", F: " + femaleChildCount + ")";
				foreach (Person person in village.Population)
				{
					output += "\n  [" + person.Id + "] " + (person.Title != null ? person.Title + " " : "") + person.Name + " - " + person.Sex + ", " + person.Age + ", " + person.Class.Name;
					if (person.Starving)
					{
						output += " (Starving)";
					}
				}
				if (village.Graveyard.Count > 0)
				{
					output += "\n  [Dead: " + village.Graveyard.Count + "]";
				}
				output += "\nResources: ";
				foreach (Resource resource in village.Resources)
				{
					output += "\n  " + resource.Name + ": " + resource.Count;
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
