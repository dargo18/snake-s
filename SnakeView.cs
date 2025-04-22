using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Util;
using Android.Views;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Media;
using Android.Content.Res;

namespace snake
{
    public class SnakeView : View
    {
        private const int SnakeSize = 20;
        private const int GameWidth = 800;
        private const int GameHeight = 800;
        private List<Point> snake;
        private Point food;
        private Point direction;
        private bool gameOver;
        private Random rand;
        private Handler handler;
        private Runnable gameLoopRunnable;
        private float startX, startY;
        private int score;
        private int topScore;
        private DatabaseService _dbService;
        private string username;
        private MediaPlayer bgMusic;
        private MediaPlayer sfxEat;
        private MediaPlayer sfxGameOver;

        public SnakeView(Context context, string username) : base(context)
        {
            this.username = username;
            _dbService = new DatabaseService();
            snake = new List<Point>
            {
                new Point(10, 10),
                new Point(10, 9),
                new Point(10, 8)
            };
            direction = new Point(0, 1);
            rand = new Random();
            gameOver = false;
            score = 0;

            handler = new Handler(Looper.MainLooper);
            gameLoopRunnable = new Runnable(GameLoop);

            InitAudio();
            LoadTopScore();
            SpawnFood();
            StartGameLoop();

            AudioControlReceiver.OnAudioCommand += HandleAudioBroadcast;
        }

        public SnakeView(Context context) : base(context)
        {
        }

        private void InitAudio()
        {
            bgMusic = new MediaPlayer();
            AssetFileDescriptor afd = Context.Assets.OpenFd("background_music.mp3");
            bgMusic.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
            bgMusic.Prepare();
            bgMusic.Looping = true;
            bgMusic.Start();


            sfxEat = new MediaPlayer();
            AssetFileDescriptor eatFd = Context.Assets.OpenFd("eat_sound.wav");
            sfxEat.SetDataSource(eatFd.FileDescriptor, eatFd.StartOffset, eatFd.Length);
            sfxEat.Prepare();

            sfxGameOver = new MediaPlayer();
            AssetFileDescriptor gameoverFd = Context.Assets.OpenFd("gameover_sound.wav");
            sfxGameOver.SetDataSource(gameoverFd.FileDescriptor, gameoverFd.StartOffset, gameoverFd.Length);
            sfxGameOver.Prepare();


        }

        private void HandleAudioBroadcast(string action)
        {
            if (action == "snake.TOGGLE_MUSIC")
            {
                if (bgMusic.IsPlaying) bgMusic.Pause();
                else bgMusic.Start();
            }
            else if (action == "snake.TOGGLE_SFX")
            {
                sfxEat.SetVolume(0, 0);
                sfxGameOver.SetVolume(0, 0);
            }
        }

        private async void LoadTopScore()
        {
            var user = await _dbService.GetUser(username);
            if (user != null)
            {
                topScore = user.Score;
            }
        }

        private async void SaveTopScore()
        {
            if (score > topScore)
            {
                topScore = score;
                await _dbService.UpdateUserScore(username, topScore);
            }
        }

        private void GameLoop()
        {
            if (gameOver)
            {
                SaveTopScore();
                bgMusic?.Stop();
                sfxGameOver?.Start();
                return;
            }

            Update();
            PostInvalidate();
            handler.PostDelayed(gameLoopRunnable, 100);
        }

        private void StartGameLoop()
        {
            handler.Post(gameLoopRunnable);
        }

        private void Update()
        {
            var head = snake.First();
            var newHead = new Point(head.X + direction.X, head.Y + direction.Y);

            if (newHead.X < 0 || newHead.X >= GameWidth / SnakeSize || newHead.Y < 0 || newHead.Y >= GameHeight / SnakeSize || snake.Contains(newHead))
            {
                gameOver = true;
                return;
            }

            snake.Insert(0, newHead);

            if (newHead.Equals(food))
            {
                score++;
                if (score > topScore)
                {
                    topScore = score;
                    _dbService.UpdateUserScore(username, topScore);
                }
                sfxEat.Start();
                SpawnFood();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }
        }

        private void SpawnFood()
        {
            Point newFood;
            bool foodOnSnake;
            int maxAttempts = 100;
            int attempts = 0;

            do
            {
                newFood = new Point(rand.Next(0, GameWidth / SnakeSize), rand.Next(0, GameHeight / SnakeSize));
                foodOnSnake = snake.Contains(newFood);
                attempts++;
            }
            while (foodOnSnake && attempts < maxAttempts);

            food = attempts < maxAttempts ? newFood : new Point(0, 0);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            int centerX = (Width - GameWidth) / 2;
            int centerY = (Height - GameHeight) / 2;

            Paint backgroundPaint = new Paint { Color = Color.LightGray };
            canvas.DrawRect(centerX, centerY, centerX + GameWidth, centerY + GameHeight, backgroundPaint);

            Paint scorePaint = new Paint { Color = Color.Black, TextSize = 50, TextAlign = Paint.Align.Center };
            canvas.DrawText($"Score: {score}  Top Score: {topScore}", Width / 2, 100, scorePaint);

            Paint snakePaint = new Paint { Color = Color.Green };
            foreach (var segment in snake)
            {
                canvas.DrawRect(centerX + segment.X * SnakeSize, centerY + segment.Y * SnakeSize,
                                centerX + (segment.X + 1) * SnakeSize, centerY + (segment.Y + 1) * SnakeSize, snakePaint);
            }

            Paint foodPaint = new Paint { Color = Color.Red };
            canvas.DrawRect(centerX + food.X * SnakeSize, centerY + food.Y * SnakeSize,
                            centerX + (food.X + 1) * SnakeSize, centerY + (food.Y + 1) * SnakeSize, foodPaint);

            if (gameOver)
            {
                Paint gameOverPaint = new Paint { Color = Color.White, TextSize = 100, TextAlign = Paint.Align.Center };
                canvas.DrawText("Game Over", Width / 2, Height / 2, gameOverPaint);
            }
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            bgMusic?.Stop();
            bgMusic?.Release();
            bgMusic = null;

            sfxEat?.Release();
            sfxEat = null;

            sfxGameOver?.Release();
            sfxGameOver = null;
        }

    }
}