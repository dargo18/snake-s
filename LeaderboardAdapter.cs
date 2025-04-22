using System.Collections.Generic;

namespace snake
{
    internal class LeaderboardAdapter
    {
        private LeaderboardActivity leaderboardActivity;
        private List<User> leaderboardUsers;

        public LeaderboardAdapter(LeaderboardActivity leaderboardActivity, List<User> leaderboardUsers)
        {
            this.leaderboardActivity = leaderboardActivity;
            this.leaderboardUsers = leaderboardUsers;
        }
    }
}