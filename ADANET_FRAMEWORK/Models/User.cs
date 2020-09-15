using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADANET_FRAMEWORK.Models {
   public class User {
      public int Id { get; set; }
      public string Name { get; set; }

      private int age;
      public int Age {
         get { return age; }
         set {
            if (value < 18) throw new Exception("Age must be greater than 17");
            this.age = value;
         }
      }

      public override string ToString()
      {
         return JsonConvert.SerializeObject(this);
      }

   }
}
