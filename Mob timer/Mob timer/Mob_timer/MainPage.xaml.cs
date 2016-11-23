using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.LocalNotifications;
using Xamarin.Forms;

namespace Mob_timer
{
    public partial class MainPage : ContentPage
    {
        private int _intervalInMinutes = 1;
        private int _currentDriver = -1;
        private List<string> _drivers = new List<string>();
        private bool _sessionStoped = true;

        public MainPage()
        {
            InitializeComponent();
        }

        private void AddMember(object sender, EventArgs e)
        {
            _drivers.Add(TeamMemberEntry.Text);
        }

        private void StartMobSession(object sender, EventArgs e)
        {
            _sessionStoped = false;
            _currentDriver = 0;
            StartTimer();
        }

        private void StopMobSession(object sender, EventArgs e)
        {
            _sessionStoped = true;
        }

        private bool ShowSwitchDriverMessase()
        {
            if (_sessionStoped)
            {
                return false;
            }
            CrossLocalNotifications.Current.Show("Time to switch!", "Next driver should be " + NextDriver());
            return true;
        }

        private string NextDriver()
        {
            int nextDriver = _currentDriver++%_drivers.Count;
            return _drivers[nextDriver];
        }

        private void StartTimer()
        {
#if DEBUG
            Device.StartTimer(new TimeSpan(0, 0, 0, 15, 0), ShowSwitchDriverMessase);
            _currentDriver = 0;
            _drivers.Add("D 1");
            _drivers.Add("D 2");
            _drivers.Add("D 3");
#else
            Device.StartTimer(new TimeSpan(0, 0, _intervalInMinutes, 0, 0), ShowSwitchDriverMessase);
           
            if (_drivers.Count == 0)
            {
                DisplayAlert("Something's wrong!", "This team has no members?", "Let me check");

            }
#endif
            ShowSwitchDriverMessase();
        }
    }
}
