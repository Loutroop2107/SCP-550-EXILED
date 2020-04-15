using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCP550;
using EXILED;

namespace SCP550.API
{
    public static class SCP550
    {
		public static ReferenceHub GetScp550()
		{
			return EventHandlers.SCP550;
		}

		public static void Spawn550(ReferenceHub player)
		{
			EventHandlers.SpawnGhoul(player, null, false);
		}
	}
}
