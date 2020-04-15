using Harmony;
using SCP550;

namespace scp550.Harmony
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.CallCmdRequestHideTag))]
	class HideTagPatch
	{
		private static bool Prefix(CharacterClassManager __instance)
		{
			bool a = Plugin.GetPlayer(__instance.gameObject).queryProcessor.PlayerId == EventHandlers.scpPlayer?.queryProcessor.PlayerId;
			if (a) __instance.TargetConsolePrint(__instance.connectionToClient, "What are you doing?", "green");
			return !a;
		}
	}
}
