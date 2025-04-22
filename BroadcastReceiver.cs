using Android.App;
using Android.Content;
using System;

[BroadcastReceiver(Enabled = true)]
[IntentFilter(new[] { "snake.TOGGLE_MUSIC", "snake.TOGGLE_SFX" })]
public class AudioControlReceiver : BroadcastReceiver
{
    public static Action<string> OnAudioCommand;

    public override void OnReceive(Context context, Intent intent)
    {
        string action = intent.Action;
        OnAudioCommand?.Invoke(action);
    }
}
