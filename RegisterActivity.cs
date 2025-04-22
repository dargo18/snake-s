using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.Threading.Tasks;

namespace snake
{
    [Activity(Label = "Register")]
    public class RegisterActivity : Activity
    {
        private DatabaseService _dbService;
        private EditText _registerUsername;
        private EditText _registerPassword;
        private Button _registerConfirmButton;
        private Button _backToLoginButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_register);

            _dbService = new DatabaseService();

            _registerUsername = FindViewById<EditText>(Resource.Id.registerUsername);
            _registerPassword = FindViewById<EditText>(Resource.Id.registerPassword);
            _registerConfirmButton = FindViewById<Button>(Resource.Id.registerConfirmButton);
            _backToLoginButton = FindViewById<Button>(Resource.Id.backToLoginButton);

            _registerConfirmButton.Click += async (sender, e) => await OnRegisterClicked();
            _backToLoginButton.Click += (sender, e) => Finish(); // Closes the RegisterActivity
        }

        private async Task OnRegisterClicked()
        {
            if (string.IsNullOrWhiteSpace(_registerUsername.Text) || string.IsNullOrWhiteSpace(_registerPassword.Text))
            {
                Toast.MakeText(this, "Please enter all fields!", ToastLength.Short).Show();
                return;
            }

            await _dbService.RegisterUser(_registerUsername.Text, _registerPassword.Text);
            Toast.MakeText(this, "User Registered Successfully!", ToastLength.Short).Show();

            // Navigate back to login screen
            Finish();
        }
    }
}
