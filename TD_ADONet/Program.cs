using EmployeDatas.Oracle;
using EmployeDatas.Mysql;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;

namespace ClientCommande
{
    class Program
    {
        static void Main(string[] args)
        {

            // Partie Oracle
            string host = "freesio.lyc-bonaparte.fr";
            int port = 21521;
            string sid = "slam";
            string login = "kerebelado";
            string pwd = "sio";
            try
            {
                EmployeOracle empOracle = new EmployeOracle(host, port, sid, login, pwd);
                empOracle.OuvrirConnection();
                //empOracle.AfficherTousLesCours();
                //empOracle.AfficherNbProjets();
                empOracle.AffichersalaireMoyenParProjet();
                //empOracle.InsereCours("BR099", "Apprentissage JBDC", 4);
                empOracle.FermerConnection();
            }
            catch (OracleException ex)
            {
                Console.WriteLine("Erreur Oracle " + ex.Message);
            }

            //// Partie Mysql
            //string hostMysql = "127.0.0.1";
            //int portMysql = 3306;
            //string baseMysql = "dbadonet";
            //string uidMysql = "employeado";
            //string pwdMysql = "employeado";
            //try
            //{

            //    EmployeMysql cnMysql = new EmployeMysql(hostMysql,portMysql,baseMysql,uidMysql,pwdMysql);
            //    cnMysql.OuvrirConnection();
            //    Console.WriteLine("connecté Mysql");
            //    cnMysql.AugmenterSalaireCurseur();
            //    cnMysql.FermerConnection();
            //    Console.WriteLine("déconnecté Mysql");
            //}
            //catch (MySqlException ex)
            //{
            //    Console.WriteLine("Erreur Mysql " + ex.Message);
            //}
            Console.ReadKey();
        }
    }
}
