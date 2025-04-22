using System;
using System.IO;
using SQLite;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Threading;

namespace snake
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Store a hashed password in production
        public int Score { get; set; } // Added score system
    }

    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UserDatabase.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<User>().Wait();
        }

        public Task<int> RegisterUser(string username, string password)
        {
            var user = new User { Username = username, Password = password, Score = 0 }; // Initialize score
            return _database.InsertAsync(user);
        }

        public async Task<User> LoginUser(string username, string password)
        {
            return await _database.Table<User>().Where(u => u.Username == username && u.Password == password).FirstOrDefaultAsync();
        }
    }