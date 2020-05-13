using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace hw_4_6.data
{
    public class SimchaFundMgr
    {
        private string _conStr = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=True;";

        public List<Simcha> GetSimchas()
        {
            using (var conn = new SqlConnection(_conStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Simchas";
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<Simcha> simchos = new List<Simcha>();
                while (reader.Read())
                {
                    simchos.Add(new Simcha
                    {
                        SimchaId = (int)reader["SimchaId"],
                        SimchaName = (string)reader["SimchaName"],
                        SimchaDate = (DateTime)reader["SimchaDate"]
                    });
                }
                foreach (Simcha s in simchos)
                {
                    Simcha current = GetTotalContributedForSimcha(s.SimchaId);
                    s.TotalContributed = current.TotalContributed;
                    s.AmountContributed = current.AmountContributed * -1;
                }
                return simchos;
            }
        }
        public void AddSimcha(Simcha simcha)
        {
            int simchaId;
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Simchas(SimchaName, SimchaDate)
                                    VALUES(@name, @date)
                                    SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@name", simcha.SimchaName);
                cmd.Parameters.AddWithValue("@date", simcha.SimchaDate);
                conn.Open();
                simchaId = (int)(decimal)cmd.ExecuteScalar();
            }
            AddContributions(GetContributorsAlwaysInclude(), simchaId);
        }
        private List<Contributor> GetContributorsAlwaysInclude()
        {
            using (var conn = new SqlConnection(_conStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Contributors WHERE AlwaysInclude = 1";
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<Contributor> contributors = new List<Contributor>();
                while (reader.Read())
                {
                    contributors.Add(new Contributor
                    {
                        ContributorId = (int)reader["ContributorId"],
                        ContributionAmount = 5
                    });
                }
                return contributors;
            }
        }
        public int GetTotalContributors()
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT COUNT(*) FROM Contributors";
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
        private Simcha GetTotalContributedForSimcha(int simchaId)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT SUM(ContributionAmount) AS sum, COUNT(*) AS count FROM Contributions cn  
                                    JOIN Simchas s
                                    ON s.SimchaId = cn.SimchaId
                                    WHERE cn.SimchaId = @id";
                cmd.Parameters.AddWithValue("@id", simchaId);
                conn.Open();
                Simcha simcha = new Simcha();
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return new Simcha
                {
                    TotalContributed = (int)reader["count"],
                    AmountContributed = (decimal)reader["sum"]
                };
            }
        }
        public void AddContributor(Contributor contr)
        {
            using (var conn = new SqlConnection(_conStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Contributors(FirstName, LastName, PhoneNumber, AlwaysInclude)
                                    VALUES(@firstname, @lastname, @phonenumber, @include)
                                    SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@firstname", contr.FirstName);
                cmd.Parameters.AddWithValue("@lastname", contr.LastName);
                cmd.Parameters.AddWithValue("@phonenumber", contr.PhoneNumber);
                cmd.Parameters.AddWithValue("@include", contr.AlwaysInclude);
                conn.Open();
                contr.ContributorId = (int)(decimal)cmd.ExecuteScalar();
            }
            AddDeposit(contr);
        }
        public void AddDeposit(Contributor contr)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Deposits(ContributorId, DepositAmount, DepositDate)
                                    VALUES(@id, @amount, @date)";
                cmd.Parameters.AddWithValue("@id", contr.ContributorId);
                cmd.Parameters.AddWithValue("@amount", contr.DepositAmount);
                cmd.Parameters.AddWithValue("@date", contr.DepositDate);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void AddContributions(List<Contributor> contrs, int simchaId)
        {
            using (var conn = new SqlConnection(_conStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Contributions(ContributorId, SimchaId, ContributionAmount)
                                    VALUES(@contrid, @simchaid, @amount)";
                conn.Open();
                foreach (Contributor c in contrs)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@contrid", c.ContributorId);
                    cmd.Parameters.AddWithValue("@simchaid", simchaId);
                    cmd.Parameters.AddWithValue("@amount", c.ContributionAmount * -1);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<Contributor> GetContributors()
        {
            using (var conn = new SqlConnection(_conStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT c.*, cn.total_contributions, d.total_deposits, d.date_created
                                    FROM (SELECT * FROM Contributors) AS c
                                    JOIN (SELECT ContributorId, SUM(DepositAmount) AS total_deposits, MIN(DepositDate) AS date_created
                                    FROM Deposits GROUP BY ContributorId) AS d
                                    ON c.ContributorId = d.ContributorId
                                    LEFT JOIN (SELECT ContributorId, SUM(ContributionAmount) AS total_contributions 
                                    FROM Contributions GROUP BY ContributorId) AS cn
                                    ON cn.contributorid = d.ContributorId                                  
                                    GROUP BY d.ContributorId, cn.total_contributions, d.total_deposits, 
                                    c.ContributorId, c.AlwaysInclude, c.FirstName, c.LastName, c.PhoneNumber, d.date_created";
                conn.Open();
                List<Contributor> contributors = new List<Contributor>();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    contributors.Add(new Contributor
                    {
                        ContributorId = (int)reader["ContributorId"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        PhoneNumber = (string)reader["PhoneNumber"],
                        AlwaysInclude = (bool)reader["AlwaysInclude"],
                        DateCreated = (DateTime)reader["date_created"],
                        Balance = ((decimal)reader["total_deposits"]) + (reader.GetOrNull<decimal>("total_contributions"))
                    });
                }
                return contributors;
            }
        }
        public Simcha GetSimchaById(int SimchaId)
        {
            using (var conn = new SqlConnection(_conStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Simchas WHERE SimchaId = @id";
                cmd.Parameters.AddWithValue("@id", SimchaId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return new Simcha
                {
                    SimchaId = (int)reader["SimchaId"],
                    SimchaName = (string)reader["SimchaName"],
                    SimchaDate = (DateTime)reader["SimchaDate"]
                };
            }
        }
        public List<int> GetIdsThatContributed(int SimchaId)
        {
            using (var conn = new SqlConnection(_conStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT ContributorId FROM Simchas s
                                    JOIN Contributions cn
                                    ON s.SimchaId = cn.simchaId
                                    WHERE s.SimchaId = @id";
                cmd.Parameters.AddWithValue("@id", SimchaId);
                List<int> ids = new List<int>();
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ids.Add((int)reader["ContributorId"]);
                }
                return ids;
            }
        }
        private List<Transaction> GetDepositsById(int contrId)
        {
            using (var conn = new SqlConnection(_conStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Deposits WHERE ContributorId = @id";
                cmd.Parameters.AddWithValue("@id", contrId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<Transaction> deposits = new List<Transaction>();
                while (reader.Read())
                {
                    deposits.Add(new Transaction
                    {
                        Amount = (decimal)reader["DepositAmount"],
                        Action = "Deposit",
                        Date = (DateTime)reader["DepositDate"]
                    });
                }
                return deposits;
            }
        }
        private List<Transaction> GetContributionsById(int contrId)
        {
            using (var conn = new SqlConnection(_conStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT s.SimchaDate, s.SimchaName, c.ContributionAmount 
                                    FROM Contributions c JOIN Simchas s
                                    ON c.simchaid = s.SimchaId
                                    WHERE ContributorId = @id";
                cmd.Parameters.AddWithValue("@id", contrId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<Transaction> contributions = new List<Transaction>();
                while (reader.Read())
                {
                    contributions.Add(new Transaction
                    {
                        Amount = (decimal)reader["ContributionAmount"],
                        Action = (string)reader["SimchaName"],
                        Date = (DateTime)reader["SimchaDate"]
                    });
                }
                return contributions;
            }
        }
        private Contributor GetContributorById(int contrId)
        {
            using (var conn = new SqlConnection(_conStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT FirstName, LastName FROM Contributors WHERE ContributorId = @id";
                cmd.Parameters.AddWithValue("@id", contrId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return new Contributor
                {
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"]
                };
            }
        }
        public Contributor GetHistoryById(int contrId)
        {
            var contributor = new Contributor();
            contributor.Transactions.AddRange(GetDepositsById(contrId));
            contributor.Transactions.AddRange(GetContributionsById(contrId));
            contributor.Transactions = contributor.Transactions.OrderByDescending(trans => trans.Date).ToList();
            foreach (Transaction trans in contributor.Transactions)
            {
                contributor.Balance += trans.Amount;
            }
            contributor.FirstName = GetContributorById(contrId).FirstName;
            contributor.LastName = GetContributorById(contrId).LastName;
            return contributor;
        }
        public void UpdateContributor(Contributor contr)
        {
            using (var conn = new SqlConnection(_conStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Contributors SET FirstName = @firstname, LastName = @lastName, 
                                    PhoneNumber = @phoneNumber, AlwaysInclude = @alwaysInclude
                                    WHERE ContributorId = @id";
                cmd.Parameters.AddWithValue("@firstname", contr.FirstName);
                cmd.Parameters.AddWithValue("@lastName", contr.LastName);
                cmd.Parameters.AddWithValue("@phoneNumber", contr.PhoneNumber);
                cmd.Parameters.AddWithValue("@alwaysInclude", contr.AlwaysInclude);
                cmd.Parameters.AddWithValue("@id", contr.ContributorId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
