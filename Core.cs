using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using NLog;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.Session;
using Torch.Utils;
using VRage.Game.ModAPI;

namespace DeathCounter
{
    public class Core:TorchPluginBase , IWpfPlugin

    {
        public static readonly Logger Log = LogManager.GetLogger("DeathCounterPlugin");

        private TorchSessionManager _sessionManager;
        public static IChatManagerServer ChatManager;

        public static Core Instance { get; private set; }
        private Control _control;
        public UserControl GetControl() => _control ?? (_control = new Control(this));
        private Persistent<Config> _config;
        public Config Config => _config?.Data;
        
        public void Save() => _config.Save();

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);

            Instance = this;
            
            _sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            ChatManager = Torch.Managers.GetManager<IChatManagerServer>();
            if (_sessionManager != null)
                _sessionManager.SessionStateChanged += SessionChanged;
            
            LoadConfig();
        }

        private void LoadConfig()
        {
            var configFile = Path.Combine(StoragePath, "DeathCounterPlugin.cfg");

            try 
            {

                _config = Persistent<Config>.Load(configFile);

            }
            catch (Exception e) 
            {
                Log.Error("Failed to load DeathCounter Plugin",e);
            }
            

            if (_config?.Data != null)
            {
                return;
            }
            Log.Info("Created Default Config, because none was found!");

            _config = new Persistent<Config>(configFile, new Config());
            _config.Save();

        }

        private void SessionChanged(ITorchSession session, TorchSessionState newstate)
        {
            switch (newstate)
            {
                case TorchSessionState.Loading:
                    
                    break;
                case TorchSessionState.Loaded:
                    DamagePatch.Init();
                    break;
                case TorchSessionState.Unloading:
                    break;
                case TorchSessionState.Unloaded:
                    break;
            }
        }

    }
}
