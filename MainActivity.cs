using Android.App;
using Android.OS;
using Android.Widget;
using snake;
using System.Threading.Tasks;

[Activity(Label = "SnakeGame", MainLauncher = true)]
public class MainActivity : Activity
{
    private SnakeView snakeView;
    private DatabaseService _dbService;
    private EditText _usernameEntry;
    private EditText _passwordEntry;
    private Button _loginButton;
    private Button _registerButton;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);

        _dbService = new DatabaseService();

        _usernameEntry = FindViewById<EditText>(Resource.Id.usernameEntry);
        _passwordEntry = FindViewById<EditText>(Resource.Id.passwordEntry);
        _loginButton = FindViewById<Button>(Resource.Id.loginButton);
        _registerButton = FindViewById<Button>(Resource.Id.registerButton);

        _loginButton.Click += async (sender, e) => await OnLoginClicked();
        _registerButton.Click += async (sender, e) => await OnRegisterClicked();
    }

    private async Task OnLoginClicked()
    {
        var user = await _dbService.LoginUser(_usernameEntry.Text, _passwordEntry.Text);
        if (user != null)
        {
            Toast.MakeText(this, "Login Success! Welcome " + user.Username, ToastLength.Short).Show();

            // Set the SnakeView as the main view after login
            RunOnUiThread(() => SetContentView(new SnakeView(this, user.Username)));
        }
        else
        {
            Toast.MakeText(this, "Login Failed!", ToastLength.Short).Show();
        }
    }


    private async System.Threading.Tasks.Task OnRegisterClicked()
    {
        StartActivity(typeof(RegisterActivity));
    }

    private void StartGame()
    {
        snakeView = new SnakeView(this);
        SetContentView(snakeView);
    }
}

