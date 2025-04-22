using Android.App;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace snake
{
    [Activity(Label = "Leaderboard")]
    public class LeaderboardActivity : Activity
    {
        private DatabaseService _dbService;
        private ListView _leaderboardListView;
        private Button _backButton;
        private List<User> _leaderboardUsers;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_leaderboard);

            try
            {
                _dbService = new DatabaseService();
                _leaderboardListView = FindViewById<ListView>(Resource.Id.leaderboardListView);
                _backButton = FindViewById<Button>(Resource.Id.backButton);

                _backButton.Click += (sender, e) => Finish(); // Go back to the game

                await LoadLeaderboard();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Leaderboard initialization failed - " + ex.Message);
                Toast.MakeText(this, "Failed to load leaderboard", ToastLength.Short).Show();
            }
        }

        private async Task LoadLeaderboard()
        {
            try
            {
                _leaderboardUsers = await _dbService.GetTopUsers();

                if (_leaderboardUsers == null || !_leaderboardUsers.Any())
                {
                    RunOnUiThread(() => Toast.MakeText(this, "No leaderboard data available.", ToastLength.Short).Show());
                    return;
                }

                RunOnUiThread(() =>
                {
                    try
                    {
                        LeaderboardAdapter adapter = new LeaderboardAdapter(this, _leaderboardUsers);
                        _leaderboardListView.Adapter = (IListAdapter)adapter;
                        Console.WriteLine("DEBUG: Leaderboard adapter set successfully.");
                    }
                    catch (InvalidCastException castEx)
                    {
                        Console.WriteLine("ERROR: Invalid cast when setting leaderboard adapter - " + castEx.Message);
                        Toast.MakeText(this, "Error displaying leaderboard: Invalid data format", ToastLength.Short).Show();
                    }
                    catch (Exception uiEx)
                    {
                        Console.WriteLine("ERROR: Failed to set leaderboard adapter - " + uiEx.Message);
                        Toast.MakeText(this, "Error displaying leaderboard", ToastLength.Short).Show();
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Failed to load leaderboard - " + ex.Message);
                RunOnUiThread(() => Toast.MakeText(this, "Error loading leaderboard", ToastLength.Short).Show());
            }
        }
    }
}