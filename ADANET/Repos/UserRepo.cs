using ADANET.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ADANET.Repos {
   public class UserRepo {
      private readonly string connString;

      public UserRepo(string connString) {
         this.connString = connString;
      }

      public void Create(User user) {

         // SQL Expression
         var sql = @"INSERT INTO[dbo].[User]
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
                  Console.WriteLine(Convert.ToInt32(idParam.Value));
            }
         }
      }
   }
}
