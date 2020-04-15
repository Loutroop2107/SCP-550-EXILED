using System.Collections.Generic;

namespace SCP550
{
	internal static class Configs
	{
		internal static List<int> spawnitems;

		internal static int health;

		internal static bool scpFriendlyFire;
		internal static bool tutorialFriendlyFire;
		internal static bool winWithTutorial;
		internal static string spawnGhoulbcmsg;
		internal static uint spawnGhoulbctime;
		internal static string scprepripcassie;
		internal static string scpspawnripcassie;
		internal static bool canUseMedicalItems;

		internal static float corrodeDistance;
		internal static float rotateInterval;
		internal static float corrodeInterval;
		internal static float corrodeHostInterval;
		internal static string Broadcast;
		internal static string sucinra550;
		internal static string ammobox;
		internal static string errorinra;

		internal static void ReloadConfig()
		{
			Configs.health = Plugin.Config.GetInt("550_health", 200);
			Configs.rotateInterval = Plugin.Config.GetFloat("550_rotate_interval", 120f);
			Configs.scpFriendlyFire = Plugin.Config.GetBool("550_scp_friendly_fire", false);
			Configs.winWithTutorial = Plugin.Config.GetBool("550_win_with_tutorial", false);
			Configs.tutorialFriendlyFire = Plugin.Config.GetBool("550_tutorial_friendly_fire", false);
			Configs.spawnitems = Plugin.Config.GetIntList("550_spawn_items");
			Configs.canUseMedicalItems = Plugin.Config.GetBool("550_can_use_medical_items", true);
			Configs.ammobox = Plugin.Config.GetString("550_ammo_box", "150:150:150");

			if (Configs.spawnitems == null || Configs.spawnitems.Count == 0)
			{
				Configs.spawnitems = new List<int>() { 7, 21, 30, 17};
			}
		}
	}
}
