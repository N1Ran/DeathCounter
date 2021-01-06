using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using NLog;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.Weapons;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Weapons;
using Torch.API.Managers;
using Torch.API.ModAPI;
using Torch.Mod;
using Torch.Mod.Messages;
using VRage.Game;
using VRage.Game.ModAPI;

namespace DeathCounter
{
    public static class DamagePatch
    {
        private static bool _init;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static ConcurrentDictionary<ulong, int> DeathCounter = new ConcurrentDictionary<ulong, int>();
        public static ConcurrentDictionary<ulong, int> KillStreak = new ConcurrentDictionary<ulong, int>();
        public static ulong _lastKiller;

        private static readonly Config Config = Core.Instance.Config;

        public static void Init()
        {
            if (_init) return;

            _init = true;

            MyAPIGateway.Session.DamageSystem.RegisterBeforeDamageHandler(1,ProcessDamage);
        }


        private static void ProcessDamage(object target, ref MyDamageInformation info)
        {
            if (target == null) return;
            if (!(target is MyCharacter character)) return;
            string victim = "";
            string causeOfDeath = "";
            if (character.Integrity - info.Amount > 0) return;
            victim = character?.DisplayName;
            if (info.Type != null) causeOfDeath = info.Type.String;
            if (string.IsNullOrEmpty(causeOfDeath) || string.IsNullOrEmpty(victim)) return;
            var attackingIdentity = MySession.Static.Players.TryGetIdentity(info.AttackerId);
            var steamId = MySession.Static.Players.TryGetSteamId(character.GetIdentity().IdentityId);
            DeathCounter.AddOrUpdate(steamId, 1, (l, i) => i + 1);
            if (MyEntities.TryGetEntityById(info.AttackerId, out var attacker, true))
            {
                switch (attacker)
                {
                    case MyCubeBlock block:
                        attackingIdentity = MySession.Static.Players.TryGetIdentity(block.CubeGrid.BigOwners.FirstOrDefault());
                        break;
                    case MyVoxelBase _:
                        if (!Config.AnnounceCollisionDeath) return;
                        causeOfDeath = "Smashed into";
                        victim = "a voxel";
                        attackingIdentity = character.GetIdentity();
                        break;
                    case MyCubeGrid grid:
                        if (!Config.AnnounceCollisionDeath) return;
                        causeOfDeath = "Rammed";
                        attackingIdentity = MySession.Static.Players.TryGetIdentity(grid.BigOwners.FirstOrDefault());
                        break;
                    case IMyHandheldGunObject<MyGunBase> rifle:
                        attackingIdentity = MySession.Static.Players.TryGetIdentity(rifle.OwnerIdentityId);
                        break;
                    case IMyHandheldGunObject<MyToolBase> handTool:
                        attackingIdentity = MySession.Static.Players.TryGetIdentity(handTool.OwnerIdentityId);
                        break;
                }

            }
            if (causeOfDeath.Equals("suicide", StringComparison.OrdinalIgnoreCase) && Config.AnnounceSuicide)
            {
                causeOfDeath = "Committed Suicide";
                victim = string.Empty;
                attackingIdentity = character.GetIdentity();
            }

            if (causeOfDeath.Equals("lowpressure", StringComparison.OrdinalIgnoreCase))
            {
                causeOfDeath = "Suffocated";
                victim = string.Empty;
                attackingIdentity = character.GetIdentity();
            }

            if (attackingIdentity == null) return;

            if (causeOfDeath.Equals("Bullet", StringComparison.OrdinalIgnoreCase)) causeOfDeath = "Shot";

            var attackerSteamId = MySession.Static.Players.TryGetSteamId(attackingIdentity.IdentityId);


            MyVisualScriptLogicProvider.SendChatMessage($"{attackingIdentity.DisplayName} {causeOfDeath} {victim}");
            Log.Info($"{character.DisplayName} died = {DeathCounter[steamId]} total death");
            if (((steamId <= 0 || attackerSteamId <= 0) && string.IsNullOrEmpty(victim)) ||
                steamId == attackerSteamId) return;
            {
                if (_lastKiller == attackerSteamId)

                {
                    KillStreak.AddOrUpdate(attackerSteamId, 2, (l, i) => i + 1);

                    MyVisualScriptLogicProvider.SendChatMessage($"{attackingIdentity.DisplayName} on {KillStreak[attackerSteamId]} kill streak");


                }
                else
                {
                    KillStreak.Clear();
                    KillStreak[steamId] = 1;
                }

                _lastKiller = attackerSteamId;
            }


        }
    }
}