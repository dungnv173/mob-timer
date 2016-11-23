using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private int _currentDriver = -1;
        ObservableCollection<Driver> _drivers = new ObservableCollection<Driver>();
        private bool _sessionStoped = true;

        public MainPage()
        {
            InitializeComponent();
            TeamMemberList.ItemsSource = _drivers;
        }

        private void AddMember(object sender, EventArgs e)
        {
            string name = TeamMemberEntry.Text;
            if (name.Length > 0)
            {
                _drivers.Add(new Driver(name));
            }
            TeamMemberEntry.Text = "";
            TeamMemberEntry.Focus();
        }

        private void StartMobSession(object sender, EventArgs e)
        {
            if (_drivers.Count == 0)
            {
                DisplayAlert("Something's wrong!", "This team has no members?", "Let me check");
                return;
            }
            _sessionStoped = false;
            _currentDriver = 0;
            StartTimer();
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }

        private void StopMobSession(object sender, EventArgs e)
        {
            _sessionStoped = true;
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        private void OnAbsent(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            Debug.WriteLine(mi.CommandParameter);
        }

        private void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            Debug.WriteLine(mi.CommandParameter);
        }

        private bool ShowSwitchDriverMessase()
        {
            if (_sessionStoped)
            {
                return false;
            }
            CrossLocalNotifications.Current.Show("Time to switch!", "Next driver should be " + NextDriver().Name);
            return true;
        }

        private Driver NextDriver()
        {
            int nextDriver = _currentDriver++%_drivers.Count;
            return _drivers[nextDriver];
        }

        private void StartTimer()
        {
#if DEBUG
            Device.StartTimer(new TimeSpan(0, 0, 0, 15, 0), ShowSwitchDriverMessase);
            _currentDriver = 0;
            if (_drivers.Count == 0)
            {
                _drivers.Add(new Driver("D 1"));
                _drivers.Add(new Driver("D 2"));
                _drivers.Add(new Driver("D 3"));
            }
#else
            Device.StartTimer(new TimeSpan(0, 0, Convert.ToInt32(DriverSwitchInterval.Text), 0, 0), ShowSwitchDriverMessase);
#endif
            CrossLocalNotifications.Current.Show("Session started", NextDriver().Name + ", you got " + DriverSwitchInterval.Text + " minutes");
        }
    }

    class Driver
    {
        public Driver(string name)
        {
            Name = name;
        }
        public string Name { set; get; }
    }
}
