using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeDatas.Oracle
{
    class EmployeOracle
    {
        /// <summary>
        /// Chaîne indiquant l'URL de la base de données
        /// </summary>
        private string host;
        /// <summary>
        /// Chaîne indiquant le port d'écoute sur le serveur base de données
        /// </summary>
        private int port;
        /// <summary>
        /// Chaîne indiquant le sid de la BD
        /// </summary>
        private string db;
        /// <summary>
        /// Chaîne indiquant le login de l'utilisateur de la BD
        /// </summary>
        private string login;
        /// <summary>
        /// Chaîne indiquant le mot de passe de connexion
        /// </summary>
        private string pwd;
        /// <summary>
        /// L'objet connexion de la BD
        /// </summary>
        private OracleConnection connexion;

        public EmployeOracle(string host, int port, string db, string login, string pwd)
        {
            this.host = host;
            this.port = port;
            this.db = db;
            this.login = login;
            this.pwd = pwd;
            string cs = String.Format("Data Source= " +
                    "(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP) (HOST = {0})(PORT = {1}))" +
                    "(CONNECT_DATA = (SERVICE_NAME = {2}))); User Id = {3}; Password = {4};"
                    , this.host, this.port, this.db, this.login, this.pwd);
            try
            {
                this.connexion = new OracleConnection(cs);
            }
            catch (OracleException ex)
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
            catch (OracleException ex)
            {
                Console.WriteLine("Erreur Oracle " + ex.Message);
            }

        }
        public void FermerConnection()
        {
            try
            {
                connexion.Close();
            }
            catch (OracleException ex)
            {
                Console.WriteLine("Erreur Oracle " + ex.Message);
            }
        }
        public void AfficherTousLesCours()
        {
            string sql = "select codecours, libellecours, nbjours from cours";
            try
            {
                OracleCommand cmd = new OracleCommand(sql, this.connexion);
                OracleDataReader resultat = cmd.ExecuteReader();
                while (resultat.Read())
                {
                    string c = "Code : " + resultat.GetString(0) + "Libelle : " + resultat.GetString(1) + "NbJours : " + resultat.GetInt16(2);
                    Console.WriteLine(c);
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine("Erreur Oracle " + ex.Message);
            }
        }
        public void AfficherNbProjets()
        {
            string sql = "select count(*) from projet";
            try
            {
                OracleCommand cmd = new OracleCommand(sql, this.connexion);
                var resultat = cmd.ExecuteScalar();
                Console.WriteLine(resultat);
            }
            catch (OracleException ex)
            {
                Console.WriteLine("Erreur Oracle " + ex.Message);
            }
        }
        public void AffichersalaireMoyenParProjet()
        {
            string sql = "select coalesce(employe.codeprojet, 'aucun') as codeprojet, AVG(employe.salaire) as moyennesalaire, coalesce(projet.nomprojet, 'null') as nomprojet, count(employe.numemp) as nbemploye from employe left join projet on employe.codeprojet=projet.codeprojet group by coalesce(employe.codeprojet, 'aucun'), coalesce(projet.nomprojet, 'null')";
            try
            {
                OracleCommand cmd = new OracleCommand(sql, this.connexion);
                OracleDataReader resultat = cmd.ExecuteReader();
                while (resultat.Read())
                {
                    string c = "Code projet : " +resultat.GetString(0)+ " | Salaire moyen : " + resultat.GetDouble(1) + " | Nom projet : " + resultat.GetString(2) + " | Nombre d'employés : "+ resultat.GetInt16(3);
                    Console.WriteLine(c);
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine("Erreur Oracle " + ex.Message);
            }

        }
        public void InsereCours(string codecours, string libelleCours, int nbJours)
        {
            string sql = "INSERT INTO cours(codecours,libellecours,nbjours) VALUES (:codecours, :libelleCours, :nbjours)";
            try
            {
                OracleCommand cmd = new OracleCommand(sql, this.connexion);
                cmd.Parameters.Add(new OracleParameter("codecours", OracleDbType.Varchar2));
                cmd.Parameters.Add(new OracleParameter("libellecours", OracleDbType.Varchar2));
                cmd.Parameters.Add(new OracleParameter("nbjours", OracleDbType.Int16));
                cmd.Parameters["codecours"].Value = codecours;
                cmd.Parameters["libellecours"].Value = libelleCours;
                cmd.Parameters["nbjours"].Value = nbJours;
                cmd.ExecuteNonQuery();
                Console.WriteLine("Ligne insérée");
            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
