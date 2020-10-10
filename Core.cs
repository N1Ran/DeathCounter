using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Session;
using Torch.Session;
using Torch.Utils;
using VRage.Game.ModAPI;

namespace DeathCounter
{
    public class Core:TorchPluginBase

    {
        private TorchSessionManager _sessionManager;
        public static IChatManagerServer ChatManager;

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);

            _sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            ChatManager = Torch.Managers.GetManager<IChatManagerServer>();
            if (_sessionManager != null)
                _sessionManager.SessionStateChanged += SessionChanged;
        }

        private void SessionChanged(ITorchSession session, TorchSessionState newstate)
        {
            switch (newstate)
            {
                case TorchSessionState.Loading:
                    
                    break;
                case TorchSessionState.Loaded:
                    Load();
                    break;
                case TorchSessionState.Unloading:
                    break;
                case TorchSessionState.Unloaded:
                    break;
            }
        }

        private void Load()
        {
            DamagePatch.Init();

        }




    }
}
