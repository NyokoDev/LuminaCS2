using Lumina.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.UI
{
    public static class WarningNotification
    {
        public static void SendRestartNotification()
        {
            string errorMsg = $"[LUMINA] Operation completed. Restart your game to apply changes.";
            GlobalPaths.SendMessage(errorMsg);
        }

        public static void CannotChangeMessage()
        {
            string errorMsg = $"[LUMINA] This setting cannot be switched while in-game.";
            GlobalPaths.SendMessage(errorMsg);
        }

    }
}
