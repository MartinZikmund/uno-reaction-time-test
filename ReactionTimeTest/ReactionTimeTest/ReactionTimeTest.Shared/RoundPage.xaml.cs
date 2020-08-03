using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ReactionTimeTest.Shared
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoundPage : Page
    {
        private Random _random = new Random();
        private DispatcherTimer _timer = new DispatcherTimer();
        private Stopwatch _stopwatch = new Stopwatch();
        private RoundState _currentState = RoundState.Wait;
        private int _roundNumber = 1;
        private double _totalMilliseconds = 0;

        private enum RoundState
        {
            Wait,
            Tap,
            TooEarly,
            Results
        }

        public RoundPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdateDisplay();
            StartWaitTimer();
        }

        private void UpdateDisplay()
        {
            WaitGrid.Visibility = Visibility.Collapsed;
            TapGrid.Visibility = Visibility.Collapsed;
            TooEarlyGrid.Visibility = Visibility.Collapsed;
            ResultsGrid.Visibility = Visibility.Collapsed;

            switch (_currentState)
            {
                case RoundState.Wait:
                    WaitGrid.Visibility = Visibility.Visible;
                    break;
                case RoundState.Tap:
                    TapGrid.Visibility = Visibility.Visible;
                    break;
                case RoundState.TooEarly:
                    TooEarlyGrid.Visibility = Visibility.Visible;
                    break;
                case RoundState.Results:
                    ResultsGrid.Visibility = Visibility.Visible;
                    break;
            }


        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_currentState == RoundState.Wait)
            {
                // too early
                _currentState = RoundState.TooEarly;
                _stopwatch.Stop();
                StopWaitTimer();
            }
            else if (_currentState == RoundState.TooEarly ||
                (_currentState == RoundState.Results && _roundNumber < 5))
            {
                if (_currentState != RoundState.TooEarly)
                {
                    _roundNumber++;
                }
                _currentState = RoundState.Wait;
                StartWaitTimer();
            }
            else if (_currentState == RoundState.Results)
            {
                Frame.Navigate(typeof(ResultsPage), (int)(_totalMilliseconds / 5));
            }
            else if (_currentState == RoundState.Tap)
            {
                _stopwatch.Stop();
                _totalMilliseconds += (int)_stopwatch.ElapsedMilliseconds;
                _currentState = RoundState.Results;
                AverageTimeRun.Text = ((int)(_totalMilliseconds / _roundNumber)).ToString();
                RoundTimeRun.Text = ((int)_stopwatch.ElapsedMilliseconds).ToString();
                RoundNumberRun.Text = _roundNumber.ToString();
            }

            UpdateDisplay();
        }

        private void StartWaitTimer()
        {
            _timer = _timer ?? new DispatcherTimer();
            _timer.Tick += Tick;
            _timer.Interval = TimeSpan.FromSeconds(_random.Next(2, 8));
            _timer.Start();
        }

        private void StopWaitTimer()
        {
            _timer.Stop();
        }

        private void Tick(object sender, object e)
        {
            _timer.Stop();
            _currentState = RoundState.Tap;
            UpdateDisplay();
            ResetStopwatch();
        }

        private void ResetStopwatch()
        {
            _stopwatch.Restart();
        }
    }
}
