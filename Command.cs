using System.Text;
using Sandbox.Game.World;
using Torch.Commands;
using Torch.Commands.Permissions;
using Torch;
using Torch.API;
using Torch.Mod;
using Torch.Mod.Messages;
using VRage.Game.ModAPI;

namespace DeathCounter
{
    [Category("death")]
    public class Command:CommandModule
    {
        [Command("count", "Shows how many times user have died since start/restart")]
        [Permission(MyPromoteLevel.None)]
        public void DeathCounter()
        {


            if (DamagePatch.DeathCounter.Count <= 0)
            {
                Context.Respond("No death since start");
                return;
            }
            
            var sb = new StringBuilder();
            sb.AppendLine();
            foreach (var death in DamagePatch.DeathCounter)
            {
                var count = death.Value;
                var playerName = MySession.Static.Players.TryGetIdentityNameFromSteamId(death.Key);

                sb.AppendLine($"{playerName}: {count}");
            }

            if (Context.Player == null)
            {
                Context.Respond(sb.ToString());
                return;
            }

            ModCommunication.SendMessageTo(new DialogMessage("Death Counter", null,sb.ToString()),Context.Player.SteamUserId);
        }
    }
}