using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using ClassLibrary;
using System.IO;

namespace TestMain
{
    class Program
    {

        static ConfigData configData = new ConfigData();

        static void Main(string[] args)
        {
            DAL dal = DAL.getInstance();

            dal.createTable("Users", DAL.catagoriesUsersString);
            /*User user1 = new User("ran", "1234", "ran@hotmail.com");
             User user2 = new User("ran2", "1234", "ran2@gmail.com");
             User user3 = new User("or", "1234", "or@walla.com");
             User user4 = new User("micha", "1234", "micha@gmail.com");



             /*ServiceReference1.MediationServerClient server = new ServiceReference1.MediationServerClient();
             server.SignUp(JsonConvert.SerializeObject(user1, Formatting.Indented));
             server.SignUp(JsonConvert.SerializeObject(user2, Formatting.Indented));
             server.SignUp(JsonConvert.SerializeObject(user3, Formatting.Indented));
             server.SignUp(JsonConvert.SerializeObject(user4, Formatting.Indented));*/
        }

    }
}
