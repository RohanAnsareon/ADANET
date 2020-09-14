using ADANET.Models;
using ADANET.Repos;
using System;

namespace ADANET {
   class Program {
      static void Main(string[] args) {
         var repo = new UserRepo(
            @"Server=(LocalDb)\MSSQLLocalDB;Database=BlaBlaMart;Trusted_Connection=True;"
         );

         repo.Create(new User {
            Name = "Nikolay",
            Age = 24
         });
      }
   }
}
