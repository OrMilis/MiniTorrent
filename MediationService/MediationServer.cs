using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ClassLibrary;

namespace MediationService
{
    public class SetComparer : EqualityComparer<User>
    {
        public override bool Equals(User x, User y)
        {
            //Console.WriteLine("comp1");
            if (x.mail.Equals(y.mail))
                return true;
            return false;
        }

        public override int GetHashCode(User obj)
        {
            //Console.WriteLine("comp 2");
            return obj.mail.GetHashCode();
        }
    }

    public class MediationServer : IMediationServer
    {
        public static SetComparer setComp = new SetComparer();
        public static List<User> activeUsers = new List<User>();
        public static Dictionary<string, HashSet<User>> files = new Dictionary<string, HashSet<User>>();
        public static List<User> allUsers = new List<User>();

        public bool SignIn(string userDataString)
        {

            User user = JsonConvert.DeserializeObject<User>(userDataString);
            if (DAL.getInstance().isExist(user))
            {
                user.isActive = true;
                activeUsers.Add(user);
                addUserToFileDictionary(user);
                DAL.getInstance().updateUserStatus(user);
                return true;
            }
            return false;
        }

        public bool SignOut(string userDataString)
        {
            User user = JsonConvert.DeserializeObject<User>(userDataString);

            if (DAL.getInstance().isExist(user))
            {
                user.isActive = false;
                activeUsers.RemoveAll(x => x.mail == user.mail);
                DAL.getInstance().updateUserStatus(user);
                removeUserToFileDictionary(user);
                return true;
            }
            return false;
        }

        public string RequestFiles(string fileName)
        {
            HashSet<User> userList = new HashSet<User>();
            Console.WriteLine(fileName);
            //files.TryGetValue(fileName.Trim('\0'), out userList);
            files.TryGetValue(fileName, out userList);
            return JsonConvert.SerializeObject(userList);
        }

        public string getAllFiles()
        {
            return JsonConvert.SerializeObject(files, Formatting.Indented);
        }

        public string getAllActiveUsers()
        {
            return JsonConvert.SerializeObject(activeUsers, Formatting.Indented);
        }

        private void addUserToFileDictionary(User user)
        {
            foreach (string s in user.userFiles.Keys)
            {
                if (!files.ContainsKey(s))
                    files.Add(s, new HashSet<User>(setComp));
                files[s].Add(user);
            }
        }

        private void removeUserToFileDictionary(User user)
        {
            foreach (string s in user.userFiles.Keys)
            {
                if (files.ContainsKey(s))
                {
                    if (files[s].Contains(user))
                        files[s].Remove(user);
                }
            }
        }

        public bool SignUp(string userDataString)
        {
            //cars[0].Add(dr[1].ToString().Trim().ToUpper())
            User user = JsonConvert.DeserializeObject<User>(userDataString);
            Console.WriteLine(user);
            if (DAL.getInstance().isExist(user))
            {
                Console.WriteLine("no possible");
                return false;
            }
            else
            {
                allUsers.Add(user);
                DAL.getInstance().insertUser(user);
                Console.WriteLine("insert complete");
                return true;
            }
        }

        public string SearchUser(string userName)
        {
            List<User> allEqualsUsers = new List<User>();
            foreach (User user in allUsers)
            {
                if (user.name.Equals(userName))
                    allEqualsUsers.Add(user);
            }
            return JsonConvert.SerializeObject(allEqualsUsers);
        }

        public void setUsers()
        {
            DAL dal = DAL.getInstance();
            string listData = dal.GetAllUsers();
            allUsers = JsonConvert.DeserializeObject<List<User>>(listData);

        }

        public bool deleteUser(string userData)
        {
            User user = JsonConvert.DeserializeObject<User>(userData);
            DAL dal = DAL.getInstance();
            //Console.WriteLine("the active before:");
            //Console.WriteLine(JsonConvert.SerializeObject(activeUsers, Formatting.Indented));
            //if(user.isActive == true)
            //    Console.WriteLine(  activeUsers.Remove(user));
            activeUsers.RemoveAll(x => x.mail.GetHashCode() == user.mail.GetHashCode());
            //Console.WriteLine("the active after:");
            //Console.WriteLine(JsonConvert.SerializeObject(activeUsers, Formatting.Indented));
            allUsers.RemoveAll(x => x.mail.GetHashCode() == user.mail.GetHashCode());
            return (dal.deleteUser(user));

        }

        public bool UpdateUser(User user)
        {
            DAL dal = DAL.getInstance();
            if (activeUsers.Contains(user))
            {
                foreach (User existUser in activeUsers)
                {
                    if (existUser.mail.GetHashCode() == user.GetHashCode())
                    {
                        existUser.name = user.name;
                        existUser.password = user.password;
                    }
                }
            }
            foreach (User existUser in allUsers)
            {
                if (existUser.mail.GetHashCode() == user.GetHashCode())
                {
                    existUser.name = user.name;
                    existUser.password = user.password;
                }
            }
            return dal.updateUser(user);
        }

        public string GetAllUsers()
        {
            return JsonConvert.SerializeObject(allUsers, Formatting.Indented);
        }

        public void resetAllActiveUsersAndActiveUsers()
        {
            allUsers = null;
            activeUsers = null;
        }

        public int add(int x, int y)
        {
            return x + y;
        }
    }


}
