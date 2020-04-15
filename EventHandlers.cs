using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EXILED;
using EXILED.Extensions;
using MEC;
using UnityEngine;
using SCP550;

namespace SCP550
{
	partial class EventHandlers
	{
		public Plugin plugin;
		public EventHandlers(Plugin plugin) => this.plugin = plugin;

		private static Dictionary<Pickup, float> scpPickups = new Dictionary<Pickup, float>();
		private List<int> ffPlayers = new List<int>();
		internal static ReferenceHub scpPlayer;
		private static bool isHidden;
		private static Vector3 GhoulSpawnPos = new Vector3(0, 1001, 8);
		private static bool hasTag;
		private bool isRoundStarted;
		private static bool isRotating;
		private static int maxHP;
		private const float dur = 327;
		private static System.Random rand = new System.Random();

		private static List<CoroutineHandle> coroutines = new List<CoroutineHandle>();

		public void OnWaitingForPlayers()
		{
			Configs.ReloadConfig();
		}

		public void OnRoundStart()
		{
			isRoundStarted = true;
			isRotating = true;
			scpPickups.Clear();
			ffPlayers.Clear();
			scpPlayer = null;

		}

		public void OnRoundEnd()
		{
			isRoundStarted = false;

			Timing.KillCoroutines(coroutines);
			coroutines.Clear();
		}

		public void OnRoundRestart()
		{
			// In case the round is force restarted
			isRoundStarted = false;

			Timing.KillCoroutines(coroutines);
			coroutines.Clear();
		}

		public void OnPickupItem(ref PickupItemEvent ev)
		{
			if (ev.Player.queryProcessor.PlayerId == SCP550?.queryProcessor.PlayerId)
			{
				ev.Allow = false;
			}
		}

		public void OnPlayerHurt(ref PlayerHurtEvent ev)
		{
			if (ev.Attacker.queryProcessor.PlayerId == SCP550.queryProcessor.PlayerId)
			{
				ev.Attacker.AddHealth(ev.Amount / 4);
			}
		}

		private static void KillSCP550(bool setRank = true)
		{
			if (setRank)
			{
				SCP550.SetRank("", "default");
				if (hasTag) SCP550.RefreshTag();
				if (isHidden) SCP550.HideTag();
				GameCore.Console.singleton.TypeCommand($"/cassie " + Configs.scprepripcassie);
				SCP550 = null;
			}
		}

		public void OnPocketDimensionEnter(PocketDimEnterEvent ev)
		{
			if (ev.Player.queryProcessor.PlayerId == scpPlayer?.queryProcessor.PlayerId && !Configs.scpFriendlyFire)
			{
				ev.Allow = false;
			}
		}

		public void OnCheckRoundEnd(ref CheckRoundEndEvent ev)
		{
			List<Team> pList = Player.GetHubs().Where(x => x.queryProcessor.PlayerId != scpPlayer?.queryProcessor.PlayerId).Select(x => Player.GetTeam(x)).ToList();

			
			if ((!pList.Contains(Team.CHI) && !pList.Contains(Team.CDP) && !pList.Contains(Team.MTF) && !pList.Contains(Team.RSC) && ((pList.Contains(Team.SCP) && scpPlayer != null) || !pList.Contains(Team.SCP) && scpPlayer != null)) ||
				(Configs.winWithTutorial && !pList.Contains(Team.CHI) && !pList.Contains(Team.CDP) && !pList.Contains(Team.MTF) && !pList.Contains(Team.RSC) && pList.Contains(Team.TUT) && scpPlayer != null))
			{
				ev.LeadingTeam = RoundSummary.LeadingTeam.Anomalies;
				ev.ForceEnd = true;
			}

			else if (scpPlayer != null && !pList.Contains(Team.SCP) && (pList.Contains(Team.CDP) || pList.Contains(Team.CHI) || pList.Contains(Team.MTF) || pList.Contains(Team.RSC)))
			{
				ev.Allow = false;
			}
		}

		public void OnCheckEscape(ref CheckEscapeEvent ev)
		{
			if (ev.Player.queryProcessor.PlayerId == scpPlayer?.queryProcessor.PlayerId) ev.Allow = false;
		}

		public void OnSetClass(SetClassEvent ev)
		{
			if (ev.Player.queryProcessor.PlayerId == scpPlayer?.queryProcessor.PlayerId)
			{
				KillSCP550();
			}
		}

		public void OnPlayerLeave(PlayerLeaveEvent ev)
		{
			if (ev.Player.queryProcessor.PlayerId == scpPlayer?.queryProcessor.PlayerId)
			{
				KillSCP550(false);
			}
		}

		public void OnPocketDimensionDie(PocketDimDeathEvent ev)
		{
			if (ev.Player.queryProcessor.PlayerId == scpPlayer?.queryProcessor.PlayerId)
			{
				ev.Allow = false;
				ev.Player.plyMovementSync.OverridePosition(GameObject.FindObjectOfType<SpawnpointManager>().GetRandomPosition(RoleType.Scp096).transform.position, 0);
			}
		}

		public void OnPlayerDie(ref PlayerDeathEvent ev)
		{
			List<Team> pList = Player.GetHubs().Where(x => x.queryProcessor.PlayerId != SCP550?.queryProcessor.PlayerId).Select(x => Player.GetTeam(x)).ToList();
			if (ev.Player.queryProcessor.PlayerId == SCP550?.queryProcessor.PlayerId)
			{
				KillSCP550();
				if (ev.Killer == pList.Contains(Team.CHI))
				{
					Cassie.CassieMessage("SCP 5 5 0 CONTAINED SUCCESSFULLY CONTAINMENT UNIT CHAOS", false, false);
					KillSCP550();
				}
				else if (ev.Killer == pList.Contains(Team.MTF))
				{
					Cassie.CassieMessage("SCP 5 5 0 CONTAINED SUCCESSFULLY CONTAINMENT UNIT MTF", false, false);
					KillSCP550();
				}
				else if (ev.Killer == pList.Contains(Team.CDP))
				{
					Cassie.CassieMessage("SCP 5 5 0 CONTAINED SUCCESSFULLY CONTAINMENT UNIT CLASS D", false, false);
					KillSCP550();
				}
				else if (ev.Killer == pList.Contains(Team.RSC))
				{
					Cassie.CassieMessage("SCP 5 5 0 CONTAINED SUCCESSFULLY CONTAINMENT UNIT SCIENTIST", false, false);
					KillSCP550();
				}
				else if (ev.Killer == !pList.Contains(Team.RSC) && !pList.Contains(Team.CDP) && !pList.Contains(Team.MTF) && !pList.Contains(Team.CHI))
				{
					Cassie.CassieMessage("SCP 5 5 0 CONTAINED SUCCESSFULLY", false, false);
					KillSCP550();
				}
			}
		}

			public void OnUseMedicalItem(MedicalItemEvent ev)
		{
			if (!Configs.canUseMedicalItems && ev.Player.queryProcessor.PlayerId == scpPlayer?.queryProcessor.PlayerId && (ev.Item == ItemType.Medkit || ev.Item == ItemType.GunUSP || ev.Item == ItemType.GunProject90 || ev.Item == ItemType.KeycardNTFLieutenant))
			{
				ev.Allow = false;
			}
		}
		public void RunOnRACommandSent(ref RACommandEvent RACom)
		{
			if (string.IsNullOrEmpty(RACom.Command) || !RACom.Command.StartsWith("spawn")) return;
			
			string[] command = RACom.Command.Split(' ');
			ReferenceHub sender = RACom.Sender.SenderId == "SERVER CONSOLE" || RACom.Sender.SenderId == "GAME CONSOLE" ? PlayerManager.localPlayer.GetPlayer() : Player.GetPlayer(RACom.Sender.SenderId);

			switch (command[0].ToLower())
			{
				case "SCP550":
					try
					{
						RACom.Allow = false;
						if (!CheckIfValueIsValid(Int32.Parse(command[1])))
						{
							RACom.Sender.RAMessage(Configs.errorinra);
							return;
						}

						ReferenceHub ChosenPlayer = new ReferenceHub();
						foreach (ReferenceHub hub in Player.GetHubs())
						{
							if (hub.GetPlayerId() == Int32.Parse(command[1]))
							{
								RACom.Sender.RAMessage(Configs.sucinra550);
								SpawnGhoul(hub);
							}
						}
					}
					catch (Exception)
					{
						RACom.Sender.RAMessage(Configs.errorinra);
						return;
					}
					break;
			}
		}
		private bool CheckIfValueIsValid(int id)
		{
			foreach (ReferenceHub hubs in Player.GetHubs())
			{
				if (hubs.GetPlayerId() == id)
					return true;
			}
			return false;
		}
	}
}

