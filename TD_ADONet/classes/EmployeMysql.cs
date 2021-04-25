using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeDatas.Mysql
{
    class EmployeMysql
    {
        private string host;
        private int port;
        private string db;
        private string login;
        private string pwd;
        private MySqlConnection connexion;

        public EmployeMysql(string host, int port, string db, string login, string pwd)
        {
            this.host = host;
            this.port = port;
            this.db = db;
            this.login = login;
            this.pwd = pwd;
            string cs = string.Format("Server = {0}; Port={1} ;Database = {2}; " + "Login = {3}; " + "Pwd = {4}", host, port, db, login, pwd);
            try
            {
                this.connexion = new MySqlConnection(cs);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        public void OuvrirConnection()
        {
            try
            {
                connexion.Open();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur MySql" + ex.Message);
            }
        }
        public void FermerConnection()
        {
            try
            {
                connexion.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur MySql" + ex.Message);
            }
        }
        public int AugmenterSalaireCurseur()
        {
            MySqlCommand cmd;
            MySqlTransaction transaction = this.connexion.BeginTransaction();
            string sql = @"select * from employe where codeprojet='PR1'";
            try
            {
                cmd = new MySqlCommand(sql, this.connexion);
                MySqlDataReader resultat = cmd.ExecuteReader();
                int nbMaj = 0;
                while (resultat.Read())
                {
                    string sqlUpdate = @"update employe set salaire=salaire*1.03 where @numemp= @numemp";
                    MySqlCommand cmdUpdate = new MySqlCommand(sqlUpdate, this.connexion);
                    cmdUpdate.Parameters.Add("@numemp", MySqlDbType.Int16);
                    cmdUpdate.ExecuteNonQuery();
                    nbMaj++;
                }
                transaction.Commit();
                resultat.Close();
                return nbMaj;
            }
            catch (MySqlException ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
                throw new Exception("Erreur à la méthode AugmenterSalaireCurseur");
            }
        }
    }
}
