using ADANET_FRAMEWORK.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ADANET_FRAMEWORK.Repos {
    public class UserRepo {
        private readonly string connString;

        public UserRepo(string connString) {
            this.connString = connString;
        }

        public int Create(User user) {

            // SQL Expression
            var sql = @"INSERT INTO [dbo].[User]
                                ([Name]
                                ,[Age])
                          VALUES
                                (@Name
                                ,@Age);
                     SET @id=SCOPE_IDENTITY()";

            // Creating connection and opening
            using (var connection = new SqlConnection(this.connString)) {
                connection.Open();

                // command creating
                using (var command = new SqlCommand(sql, connection)) {

                    #region Adding parameters
                    #region InputParams
                    var nameParam = new SqlParameter("@Name", user.Name);

                    command.Parameters.Add(nameParam);


                    command.Parameters.AddWithValue("@Age", user.Age);
                    #endregion

                    #region OutputParam
                    var idParam = new SqlParameter {
                        ParameterName = "@id",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Direction = System.Data.ParameterDirection.Output
                    };

                    command.Parameters.Add(idParam);
                    #endregion

                    #endregion

                    if (command.ExecuteNonQuery() == 0)
                        throw new Exception("User was not inserted");
                    else
                        return Convert.ToInt32(idParam.Value);
                }
            }
        }

        public User Read(int id) {
            var sql = @"SELECT * FROM [dbo].[User] WHERE Id = @Id";

            using (var connection = new SqlConnection(this.connString)) {
                connection.Open();
                using (var command = new SqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader()) {
                        reader.Read();

                        return new User {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Age = Convert.ToInt32(reader["Age"])
                        };
                    }
                }
            }
        }

        public List<User> GetAll() {
            var sql = @"SELECT * FROM [dbo].[User]";

            using (var connection = new SqlConnection(this.connString)) {
                connection.Open();

                var users = new List<User>();

                using (var command = new SqlCommand(sql, connection)) {
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            users.Add(new User {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Age = Convert.ToInt32(reader["Age"])
                            });
                        }
                    }
                }

                return users;
            }
        }

        public List<User> GetByName(string name) {
            var sql = @"SELECT * FROM [dbo].[User] WHERE Name = @Name";

            using (var connection = new SqlConnection(this.connString)) {
                connection.Open();

                var users = new List<User>();

                using (var command = new SqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@Name", name);

                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            users.Add(new User {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Age = Convert.ToInt32(reader["Age"])
                            });
                        }
                    }
                }

                return users;
            }
        }

        public void Update(int id, User user) {
            var sql = @"UPDATE [dbo].[User] SET [Name] = @Name, [Age] = @Age WHERE Id = @Id";

            using (var connection = new SqlConnection(this.connString)) {
                connection.Open();

                using (var command = new SqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Age", user.Age);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id) {
            var sql = @"DELETE FROM [dbo].[User]
                     WHERE Id = @Id";

            using (var connection = new SqlConnection(this.connString)) {
                connection.Open();
                using (var command = new SqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@Id", id);

                    if (command.ExecuteNonQuery() == 0)
                        throw new Exception("User was not deleted");
                }
            }
        }

        public void DeleteMultiple(List<int> ids) {
            var parameters = new string[ids.Count];

            for (int i = 0; i < ids.Count; i++) {
                parameters[i] = string.Format("@Id{0}", i);
            }

            var sql = @"DELETE FROM [dbo].[User] WHERE Id IN(" + string.Join(", ", parameters) + ")";

            using (var connection = new SqlConnection(this.connString)) {
                connection.Open();
                using (var command = new SqlCommand(sql, connection)) {
                    for (int i = 0; i < ids.Count; i++) {
                        command.Parameters.AddWithValue(parameters[i], ids[i]);
                    }
                    if (command.ExecuteNonQuery() == 0)
                        throw new Exception("User was not deleted");
                }
            }
        }

        public void DeleteMultipleWithTransaction(List<int> ids) {
            var sql = @"DELETE FROM [User] WHERE Id = @Id";

            using (var connection = new SqlConnection(this.connString)) {
                connection.Open();
                var transaction = connection.BeginTransaction("DeleteTransaction");
                using (var command = connection.CreateCommand()) {
                    command.Transaction = transaction;
                    int i = 0;
                    try {
                        foreach (var id in ids) {
                            i++;

                            command.CommandText = $"{sql}{i}";
                            command.Parameters.AddWithValue($"@Id{i}", id);

                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();

                    } catch (Exception ex) {
                        Console.WriteLine(ex.Message);

                        try {
                            transaction.Rollback();
                        } catch (Exception ex2) {
                            Console.WriteLine(ex2.Message);
                        }
                    }
                }
            }
        }
    }
}
