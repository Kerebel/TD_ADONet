using EmployeDatas.Mysql;
using EmployeDatas.Oracle;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;

namespace TD_ADONet
{
    class Program
    {
        static void Main(string[] args)
        {
            //  Partie Oracle
            String host = "freesio.lyc-bonaparte.fr";
            int port = 21521;
            string sid = "SLAM";
            string login = "rocheado";
            string pwd = "rocheado";
            try
            {
                EmployeOracle empOracle = new EmployeOracle(host, port, sid, login, pwd);
                empOracle.Ouvrir();
                empOracle.afficherTousLesCours();
                empOracle.afficherNbProjets();
                empOracle.AfficherSalaireMoyenParProjet();
                Console.WriteLine("Nombre de lignes curseur mises à jour : " + empOracle.AugmenterSalaireCurseur());
                empOracle.AfficherEmployesSalaire(10000);
                empOracle.AfficheSalaireEmploye(76);
                empOracle.InsereCours("BR099", "Apprentissage JDBC", 4);
                empOracle.supprimeCours("BR099");
                empOracle.AugmenterSalaire("PR2", 2);
                empOracle.Fermer();
                /*
                * 
                * Appel des méthodes
                *
                */

            }
            catch (OracleException ex)
            {
                Console.WriteLine("Erreur Oracle " + ex.Message);
            }

            //  Partie Mysql

            string hostMysql = "127.0.0.1";
            int portMysql = 3306;
            string baseMysql = "dbadonet";
            String loginMysql = "employeado";
            String pwdMysql = "employeado";
            try
            {
                EmployeMysql empMysql = new EmployeMysql(hostMysql, portMysql, baseMysql, loginMysql, pwdMysql);
                empMysql.Ouvrir();
                Console.WriteLine("connecté Mysql");

                empMysql.AfficherTousLesEmployes();
                empMysql.AfficherNbSeminaires();
                empMysql.AfficherNbInscritsParCours();
                Console.WriteLine("Nombre de lignes curseur mises à jour : " + empMysql.AugmenterSalaireCurseur());
                empMysql.AfficheProjetsNbEmployes(3);
                empMysql.SeminairesPosterieurs(new DateTime(2019, 12, 15));
                empMysql.InsereProjet("PR9", "Alerte à Malibu", new DateTime(2019, 11, 20), new DateTime(2020, 6, 30), "Monsieur Jean Tienun");
                empMysql.SupprimeSeminaire("BR0340413");
                empMysql.AjouterNbJoursCours(1, 3);
                empMysql.Fermer();
                Console.WriteLine("déconnecté Mysql");

            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erreur Mysql " + ex.Message);
            }

            Console.ReadKey();
        }
    }
}
