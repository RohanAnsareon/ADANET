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

      public User Read(int id)
      {
         var sql = @"SELECT * FROM [dbo].[User] WHERE Id = @Id";

         using (var connection = new SqlConnection(this.connString))
         {
            connection.Open();
            using (var command = new SqlCommand(sql, connection))
            {
               command.Parameters.AddWithValue("@Id", id);

               using (var reader = command.ExecuteReader())
               {
                  reader.Read();

                  return new User
                  {
                     Id = Convert.ToInt32(reader["Id"]),
                     Name = reader["Name"].ToString(),
                     Age = Convert.ToInt32(reader["Age"])
                  };
               }
            }
         }
      }

      public void Update(int id, User user)
      {
         var sql = @"UPDATE [dbo].[User] SET [Name] = @Name, [Age] = @Age WHERE Id = @Id";

         using (var connection = new SqlConnection(this.connString))
         {
            connection.Open();

            using (var command = new SqlCommand(sql, connection))
            {
               command.Parameters.AddWithValue("@Id", id);
               command.Parameters.AddWithValue("@Name", user.Name);
               command.Parameters.AddWithValue("@Age", user.Age);

               command.ExecuteNonQuery();
            }
         }
      }

      public void Delete(int id)
      {
         var sql = @"DELETE FROM [dbo].[User]
                     WHERE Id = @Id";

         using (var connection = new SqlConnection(this.connString))
         {
            connection.Open();
            using (var command = new SqlCommand(sql, connection))
            {
               command.Parameters.AddWithValue("@Id", id);

               if (command.ExecuteNonQuery() == 0)
                  throw new Exception("User was not deleted");
            }
         }
      }
   }
}
