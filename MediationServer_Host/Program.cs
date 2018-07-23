using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ClassLibrary;
using System.IO;

namespace MediationServer_Host
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(MediationService.MediationServer));
            host.Opened += new EventHandler(initServer);
            host.Open();
            Console.WriteLine("Service Hosted Sucessfully");
            Console.Read();
            host.Close();

           /*
            var dir = new DirectoryInfo("D:\\");
            foreach (FileInfo f in dir.GetFiles())
            {
                user1.userFiles.Add(f.Name, f.Length);
            }

            //myDal.createTable(DAL.userTableName, DAL.catagoriesUsersString);
            myDal.insertUser(user2);*/
            
        }

        private static void initServer(object sender, EventArgs e)
        {
            ServerService.MediationServerClient server = new ServerService.MediationServerClient();
            server.setUsers();
        }
    }
}
