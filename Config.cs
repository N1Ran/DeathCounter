using NLog;
using Torch;

namespace DeathCounter
{
    public class Config : ViewModel
    {
        private bool _enable = true;
        private bool _announceSuicide = true;
        private bool _announceCollisionDeath = true;

        public bool Enable
        {
            get => _enable;
            set
            {
                _enable = value;
                OnPropertyChanged();
            }
        }

        public bool AnnounceSuicide
        {
            get => _announceSuicide;
            set
            {
                _announceSuicide = value;
                OnPropertyChanged();
            }
        }

        public bool AnnounceCollisionDeath
        {
            get => _announceCollisionDeath;
            set
            {
                _announceCollisionDeath = value;
                OnPropertyChanged();
            }
        }
        
        
    }
}