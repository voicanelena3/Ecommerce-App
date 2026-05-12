using System.Data;
using Microsoft.Data.SqlClient;
using Repository.Data;
using Repository.Models;

namespace Repository.Repositories
{
    public interface IUserRepository
    {
        User? GetUserByEmail(string email);
        User? GetUserById(int userId);
        bool CreateUser(User user);
        bool UserExists(string email);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDataAccessLayer _dal;

        public UserRepository(IDataAccessLayer dal)
        {
            _dal = dal;
        }

        public User? GetUserByEmail(string email)
        {
            string query = "SELECT Id, Email, FirstName, LastName, PasswordHash, CreatedAt FROM Users WHERE Email = @Email";
            SqlParameter[] parameters = new[] { new SqlParameter("@Email", email) };

            DataTable dt = _dal.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                return MapToUser(dt.Rows[0]);
            }

            return null;
        }

        public User? GetUserById(int userId)
        {
            string query = "SELECT Id, Email, FirstName, LastName, PasswordHash, CreatedAt FROM Users WHERE Id = @Id";
            SqlParameter[] parameters = new[] { new SqlParameter("@Id", userId) };

            DataTable dt = _dal.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                return MapToUser(dt.Rows[0]);
            }

            return null;
        }

        public bool CreateUser(User user)
        {
            string query = @"INSERT INTO Users (Email, FirstName, LastName, PasswordHash, CreatedAt)
                             VALUES (@Email, @FirstName, @LastName, @PasswordHash, @CreatedAt)";

            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@FirstName", user.FirstName),
                new SqlParameter("@LastName", user.LastName),
                new SqlParameter("@PasswordHash", user.PasswordHash),
                new SqlParameter("@CreatedAt", user.CreatedAt)
            };

            int result = _dal.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool UserExists(string email)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
            SqlParameter[] parameters = new[] { new SqlParameter("@Email", email) };

            object? result = _dal.ExecuteScalar(query, parameters);
            return result != null && (int)result > 0;
        }

        private User MapToUser(DataRow row)
        {
            return new User
            {
                Id = (int)row["Id"],
                Email = row["Email"].ToString() ?? string.Empty,
                FirstName = row["FirstName"].ToString() ?? string.Empty,
                LastName = row["LastName"].ToString() ?? string.Empty,
                PasswordHash = row["PasswordHash"].ToString() ?? string.Empty,
                CreatedAt = (DateTime)row["CreatedAt"]
            };
        }
    }
}
