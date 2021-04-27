using Oracle.ManagedDataAccess.Client;
using System;

namespace EmployeDatas.Oracle
{
    class EmployeOracle
    {
        private String host;        // chaine indiquant l'URL de la base de données
        private int port;           // entier indiquant le port d'écoute sur le serveur base de données
        private String db;          // chaine indiquant le sid de la BD
        private String login;       // chaine indiquant le login de l'utilisateur de  la base de données
        private String pwd;         // chaine indiquant le mot de passe de connexion
        private OracleConnection connexion; // l'objet connexion de votre base de données

        public EmployeOracle(string host, int port, string db, string login, string pwd)
        {
            this.host = host;
            this.port = port;
            this.db = db;
            this.login = login;
            this.pwd = pwd;
            String cs = String.Format("Data Source= " +
                    "(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = {0})(PORT = {1}))" +
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

        public void Ouvrir()
        {
            try
            {
                this.connexion.Open();
            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void Fermer()
        {
            try
            {
                this.connexion.Close();
            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void afficherTousLesCours()
        {
            String sql = "select codecours, libellecours, nbjours from cours";
            try
            {
                OracleCommand cmd = new OracleCommand(sql, this.connexion);
                OracleDataReader resultat = cmd.ExecuteReader();
                while (resultat.Read())
                {
                    String c = "Code : " + resultat.GetString(0) + "  Libelle : " + resultat.GetString(1) + " Nb jours : "
                            + resultat.GetInt16(2);
                    Console.WriteLine(c);

                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void afficherNbProjets()
        {
            String sql = "select count(*) nb from projet";
            try
            {
                OracleCommand cmd = new OracleCommand(sql, this.connexion);
                var nbProjets = cmd.ExecuteScalar();
                Console.WriteLine("Nombre de projets : " + nbProjets);
            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public void AfficherSalaireMoyenParProjet()
        {
            // on va gérer de 2 façons la possibilité de retour de valeur nulle
            // en amont au niveau de la requête
            //ou
            // par l'application au moment d'exploiter le retour des données de la base
            String sql = "select coalesce(projet.codeprojet, 'Aucun') codeprojet, coalesce(nomprojet, 'Null'),  round(avg(salaire),2) moysalaire "
                + " from employe left join projet on employe.codeprojet=projet.codeprojet"
                + " group by projet.codeprojet, nomprojet";
            try
            {
                OracleCommand cmd = new OracleCommand(sql, this.connexion);
                OracleDataReader resultat = cmd.ExecuteReader();
                while (resultat.Read())
                {
                    String mess = "Code projet : " + resultat.GetString(0) + " \tnom projet : " + (resultat.IsDBNull(1) ? "---" : resultat.GetString(1))
                        + " \t\tNombre employés : " + resultat.GetDecimal(2);

                    Console.WriteLine(mess);
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public int AugmenterSalaireCurseur()
        {
            OracleCommand cmd;
            OracleTransaction transOracle = this.connexion.BeginTransaction();
            String sql = @"select numemp, nomemp, prenomemp, poste, salaire, prime, codeprojet, superieur from employe where codeprojet='PR1'";
            try
            {
                cmd = new OracleCommand(sql, connexion);
                OracleDataReader resultat = cmd.ExecuteReader();
                int nb = 0;
                while (resultat.Read())
                {
                    string sqlUpdate = @"update employe set salaire =salaire*1.03 where numemp=:numemp";
                    OracleCommand cmdUpdate = new OracleCommand(sqlUpdate, this.connexion);
                    cmdUpdate.Parameters.Add(new OracleParameter("numemp", OracleDbType.Int16, System.Data.ParameterDirection.Input));
                    cmdUpdate.Parameters[0].Value = resultat.GetValue(0);
                    cmdUpdate.ExecuteNonQuery();
                    nb++;
                }
                transOracle.Commit();
                resultat.Close();
                return nb;

            }
            catch (OracleException ex)
            {
                transOracle.Rollback();
                Console.WriteLine(ex.Message);
                throw new Exception("Erreur à la méthode augmenterSalaireCurseur");
            }
        }

        public void AfficherEmployesSalaire(double salairePlancher)
        {
            String sql = "select numemp, nomemp, prenomemp, salaire from employe where salaire < :salaireMini";

            try
            {
                OracleCommand cmd = new OracleCommand(sql, this.connexion);
                cmd.Parameters.Add(new OracleParameter("salaireMini", OracleDbType.Decimal, System.Data.ParameterDirection.Input));
                cmd.Parameters["salaireMini"].Value = salairePlancher;
                OracleDataReader resultat = cmd.ExecuteReader();
                while (resultat.Read())
                {
                    String mess = "num : " + Convert.ToInt16(resultat["numemp"]) + " \tnom : " + resultat["nomemp"].ToString()
                            + " \tprénom : " + resultat["prenomemp"] + "\t\tsalaire : "
                            + Convert.ToDecimal(resultat["salaire"]);
                    Console.WriteLine(mess);
                }
                resultat.Close();
            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AfficheSalaireEmploye(int numemp)
        {
            String sql = "select numemp, nomemp, prenomemp, salaire from employe where numemp = :numEmp";

            try
            {
                OracleCommand cmd = new OracleCommand(sql, this.connexion);
                cmd.Parameters.Add(new OracleParameter("numEmp", OracleDbType.Int16, System.Data.ParameterDirection.Input));
                cmd.Parameters["numEmp"].Value = numemp;
                OracleDataReader resultat = cmd.ExecuteReader();
                String mess;
                if (resultat.Read())
                {
                    mess = "num : " + resultat.GetInt16(0) + " \tnom : " + resultat.GetString(1) + " \tprénom : "
                            + resultat.GetString(2) + "\t\tsalaire : " + resultat.GetDouble(3);

                }
                else
                {
                    mess = String.Format("l'employé numéro {0} n'existe pas", numemp);
                }
                Console.WriteLine(mess);
                resultat.Close();
            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void InsereCours(String code, String nom, int nbJours)
        {
            String sql = "insert into cours(codecours,libellecours, nbjours) values(:code, :libelle,:nbJours)";
            try
            {
                OracleCommand cmd = new OracleCommand(sql, this.connexion);
                cmd.Parameters.Add(new OracleParameter("code", OracleDbType.Char, System.Data.ParameterDirection.Input));
                cmd.Parameters.Add(new OracleParameter("libelle", OracleDbType.Char, System.Data.ParameterDirection.Input));
                cmd.Parameters.Add(new OracleParameter("nbJours", OracleDbType.Int16, System.Data.ParameterDirection.Input));
                cmd.Parameters[0].Value = code;
                cmd.Parameters[1].Value = nom;
                cmd.Parameters[2].Value = nbJours;
                int nb = cmd.ExecuteNonQuery();
                Console.WriteLine("Nombre de lignes insérées : " + nb);
            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void supprimeCours(String code)
        {
            String sql = "delete from cours where codecours = :code";
            try
            {
                OracleCommand cmd = new OracleCommand(sql, this.connexion);
                cmd.Parameters.Add(new OracleParameter("code", OracleDbType.Char, System.Data.ParameterDirection.Input));
                cmd.Parameters[0].Value = code;
                int nb = cmd.ExecuteNonQuery();
                Console.WriteLine("Nombre de lignes supprimées : " + nb);
            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AugmenterSalaire(String codeProjet, decimal pourcent)
        {
            String sql = "update employe set salaire=salaire * (1+:pourcent/100) where codeprojet = :code";
            try
            {
                OracleCommand cmd = new OracleCommand(sql, this.connexion);
                cmd.Parameters.Add(new OracleParameter("pourcent", OracleDbType.Decimal, System.Data.ParameterDirection.Input));
                cmd.Parameters.Add(new OracleParameter("code", OracleDbType.Char, System.Data.ParameterDirection.Input));
                cmd.Parameters[0].Value = pourcent;
                cmd.Parameters[1].Value = codeProjet;
                int nb = cmd.ExecuteNonQuery();
                Console.WriteLine("Nombre de lignes mises à jour : " + nb);
            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
