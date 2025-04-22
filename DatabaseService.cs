using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using System.Collections.Generic;

namespace snake
{
    public class User
    {
        [PrimaryKey, Unique]
        public string Username { get; set; }

        public string Password { get; set; }

        public int Score { get; set; } // Stores the user's top score
    }

    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UserDatabase.db3");
            Console.WriteLine("DEBUG: Database Path - " + dbPath);

            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<User>().Wait();
        }

        public async Task<int> RegisterUser(string username, string password)
        {
            try
            {
                var existingUser = await _database.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    Console.WriteLine("ERROR: User already exists!");
                    return 0; // User already exists
                }

                var user = new User { Username = username, Password = password, Score = 0 };
                int result = await _database.InsertAsync(user);

                Console.WriteLine($"DEBUG: User {username} registered successfully!");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Failed to register user: " + ex.Message);
                return 0;
            }
        }

        public async Task<User> LoginUser(string username, string password)
        {
            return await _database.Table<User>()
                .Where(u => u.Username == username && u.Password == password)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateUserScore(string username, int newScore)
        {
            try
            {
                var user = await _database.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();

                if (user != null)
                {
                    Console.WriteLine($"DEBUG: Current DB score for {username}: {user.Score}, New Score: {newScore}");

                    if (newScore >= user.Score) // Update score if it's higher or equal
                    {
                        user.Score = newScore;
                        int rowsUpdated = await _database.UpdateAsync(user);
                        Console.WriteLine($"DEBUG: User {user.Username} score updated to {user.Score}. Rows affected: {rowsUpdated}");
                        return rowsUpdated;
                    }
                    else
                    {
                        Console.WriteLine("DEBUG: New score is not higher. No update performed.");
                    }
                }
                else
                {
                    Console.WriteLine("ERROR: User not found in database. Score update failed.");
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Failed to update user score: " + ex.Message);
                return 0;
            }
        }

        public async Task<User> GetUser(string username)
        {
            try
            {
                return await _database.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Failed to fetch user: " + ex.Message);
                return null;
            }
        }

        public async Task<List<User>> GetTopUsers()
        {
            try
            {
                var users = await _database.Table<User>().OrderByDescending(u => u.Score).Take(10).ToListAsync();
                Console.WriteLine($"DEBUG: Leaderboard fetched {users.Count} users.");
                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Failed to fetch leaderboard: " + ex.Message);
                return new List<User>();
            }
        }
    }
}
