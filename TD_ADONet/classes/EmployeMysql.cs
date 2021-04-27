using MySql.Data.MySqlClient;
using System;

namespace EmployeDatas.Mysql
{
    class EmployeMysql
    {
        private String host;        // chaine indiquant l'URL de la base de données
        private int port;           // entier indiquant le port d'écoute sur le serveur base de données
        private String db;          // chaine indiquant le nom de la base de données par défaut
        private String login;       // haine indiquant le login de l'utilisateur de  la base de données
        private String pwd;         // chaine indiquant le mot de passe de connexion
        private MySqlConnection connexion; // l'objet connexion de votre base de données

        public EmployeMysql(string host, int port, string db, string login, string pwd)
        {
            this.host = host;
            this.port = port;
            this.db = db;
            this.login = login;
            this.pwd = pwd;
            String csMysql = String.Format("Server = {0}; Port={1} ;Database = {2}; " +
                    "Uid = {3}; " +
                    "Pwd = {4}", this.host, this.port, this.db, this.login, this.pwd, "MultipleActiveResultSets = True");
            this.connexion = new MySqlConnection(csMysql);
        }
        public void Ouvrir()
        {
            try
            {
                this.connexion.Open();
            }
            catch (MySqlException ex)
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
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AfficherTousLesEmployes()
        {
            String sql = "select * from employe";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, this.connexion);
                MySqlDataReader resultat = cmd.ExecuteReader();
                while (resultat.Read())
                {
                    String message = "numéro : " + resultat.GetInt16("numemp") + "  nom : " + resultat.GetString("nomemp")
                            + " prénom : " + resultat.GetString("prenomemp");
                    Console.WriteLine(message);
                }
                resultat.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AfficherNbSeminaires()
        {
            String sql = "select count(*) nb from seminaire";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, this.connexion);
                var nbProjets = cmd.ExecuteScalar();
                Console.WriteLine("Nombre de séminaires : " + nbProjets);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void AfficherNbInscritsParCours()
        {
            String sql = "select cours.codecours, cours.libellecours, count(inscrit.numemp) as nbInscrits "
                + " from cours left join seminaire on cours.codecours=seminaire.codecours"
                + " left join inscrit on inscrit.codesemi = seminaire.codesemi "
                + " group by cours.codecours, cours.libellecours";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, this.connexion);
                MySqlDataReader resultat = cmd.ExecuteReader();
                while (resultat.Read())
                {
                    String mess = "Code cours : " + resultat.GetString(0) + " \tnom cours : " + resultat.GetString(1)
                            + " \t\tNombre inscrits : " + resultat.GetInt16(2);
                    Console.WriteLine(mess);
                }
                resultat.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Augmenter le salaire de 3% pour les employés qui travaillent sur PR1
        /// (dans un curseur ayant ramené tous les employés)
        /// </summary>
        /// <returns></returns>
        public int AugmenterSalaireCurseur()
        {
            String sql = "select numemp, nomemp, prenomemp, poste, salaire, prime, codeprojet, superieur from employe";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, this.connexion);
                MySqlDataReader resultat = cmd.ExecuteReader();
                int nb = 0;
                while (resultat.Read())
                {
                    //String cp = resultat.IsDBNull(6) ? resultat.GetString("codeprojet"):"XX"  ;
                    //if (cp !="XX" && cp.Trim() == "PR1")
                    if (!resultat.IsDBNull(resultat.GetOrdinal("codeprojet")) && resultat.GetString("codeprojet") == "PR1")
                    {
                        string sqlUpdate = @"update employe set salaire =salaire*1.03 where numemp= @numemp";

                        MySqlCommand cmdUpdate = new MySqlCommand(sqlUpdate, this.connexion);
                        cmdUpdate.Parameters.Add(new MySqlParameter("numemp", MySqlDbType.Int32));
                        cmdUpdate.Parameters[0].Value = resultat.GetValue(0);
                        //cmdUpdate.ExecuteNonQuery();
                        cmdUpdate.ExecuteReaderAsync();
                        nb++;
                    }
                }
                resultat.Close();
                return nb;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Erreur à la méthode augmenterSalaireCurseur");
            }
        }



        /// <summary>
        /// Afficher les projets qui ont plus de 3 employés affectés (paramètre) : codeprojet, nomprojet
        /// </summary>
        /// <param name="nbEmployesMini"></param>
        public void AfficheProjetsNbEmployes(int nbEmployesMini)
        {
            String sql = "select projet.codeprojet, nomprojet, count(*) as nbEmployes"
                + " from employe inner join projet on employe.codeprojet=projet.codeprojet "
                + " group by projet.codeprojet, nomprojet " +
                "   having count(*) > @nb";

            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, this.connexion);
                cmd.Parameters.Add(new MySqlParameter("nb", MySqlDbType.Int32));
                cmd.Parameters["nb"].Value = nbEmployesMini;
                MySqlDataReader resultat = cmd.ExecuteReader();
                while (resultat.Read())
                {
                    String mess = "code : " + resultat.GetString("codeprojet") + " \tnom : " + resultat.GetString("nomprojet")
                            + " \tnb employes : " + resultat.GetInt16("nbEmployes");
                    Console.WriteLine(mess);
                }
                resultat.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SeminairesPosterieurs(DateTime dateAPartirDe)
        {
            String sql = "select codesemi, cours.codecours, libellecours" + " from seminaire inner join cours"
                + " on seminaire.codecours=cours.codecours" + " where datedebutsem > @datemin";

            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, this.connexion);
                cmd.Parameters.Add(new MySqlParameter("datemin", MySqlDbType.DateTime));
                cmd.Parameters["datemin"].Value = dateAPartirDe;
                MySqlDataReader resultat = cmd.ExecuteReader();
                String mess = "";
                while (resultat.Read())
                {
                    mess += mess = "codesemi : " + resultat.GetString("codesemi") + " \tcodecours : "
                            + resultat.GetString("codecours") + " \tLibellé cours : " + resultat.GetString("libellecours")
                            + "\n";
                }
                if (mess.Length == 0)
                {
                    mess = String.Format("pas de séminaire après le {0}", dateAPartirDe.ToShortDateString());
                }
                Console.WriteLine(mess);

                resultat.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void InsereProjet(String codeProjet, String nomProjet, DateTime dateDebut, DateTime dateFin, String nomContact)
        {
            String sql = "insert into projet(codeprojet,nomprojet, debutproj, finprevue, nomcontact) " +
                "values(@code, @nom,@datedebut, @datefin,@nomcontact)";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, this.connexion);
                cmd.Parameters.Add(new MySqlParameter("code", MySqlDbType.String));
                cmd.Parameters["code"].Value = codeProjet;
                cmd.Parameters.Add(new MySqlParameter("nom", MySqlDbType.String));
                cmd.Parameters["nom"].Value = nomProjet;
                cmd.Parameters.Add(new MySqlParameter("datedebut", MySqlDbType.DateTime));
                cmd.Parameters["datedebut"].Value = dateDebut;
                cmd.Parameters.Add(new MySqlParameter("datefin", MySqlDbType.DateTime));
                cmd.Parameters["dateFin"].Value = dateFin;
                cmd.Parameters.Add(new MySqlParameter("nomcontact", MySqlDbType.String));
                cmd.Parameters["nomcontact"].Value = nomContact;
                int nb = cmd.ExecuteNonQuery();
                Console.WriteLine("Nombre de lignes insérées  dans projet :: " + nb);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SupprimeSeminaire(String code)
        {
            String sql = "delete from seminaire where codesemi =@code";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, this.connexion);
                cmd.Parameters.Add(new MySqlParameter("code", MySqlDbType.String));
                cmd.Parameters["code"].Value = code;
                int nb = cmd.ExecuteNonQuery();
                Console.WriteLine("Nombre de séminaires supprimées : " + nb);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AjouterNbJoursCours(int nbAAjouter, int nbJoursPlafond)
        {
            String sql = "update cours set nbjours= nbjours+@nbAAjouter where nbjours< @nbJoursPlafond";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, this.connexion);
                cmd.Parameters.Add(new MySqlParameter("nbAAjouter", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("nbJoursPlafond", MySqlDbType.Int32));
                cmd.Parameters["nbAAjouter"].Value = nbAAjouter;
                cmd.Parameters["nbJoursPlafond"].Value = nbJoursPlafond;
                int nb = cmd.ExecuteNonQuery();
                Console.WriteLine("Nombre de lignes mises à jour : " + nb);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        //public void insereCategorie(String libelle) throws SQLException
        //{
        //    String sql = "insert into categorie(libelle) values(?)";
        //		try {
        //        PreparedStatement statement = this.connection.prepareStatement(sql, Statement.RETURN_GENERATED_KEYS);
        //        statement.setString(1, libelle);
        //        int nb = statement.executeUpdate();
        //        ResultSet rs = statement.getGeneratedKeys();
        //        if (rs.next())
        //        {
        //            System.out.println("clé générée (Mysql) : " + rs.getInt(1));
        //        }
        //    }

        //		catch (SQLException ex) {
        //        System.out.println(ex.getMessage());
        //    }
        //}

        //public void insereCategorieCours(Vector<String> parametres) throws SQLException
        //{
        //    String sql = "insert into categorie(libelle) values(?)";
        //		try {
        //        this.connection.setAutoCommit(false);
        //        PreparedStatement statement = this.connection.prepareStatement(sql, Statement.RETURN_GENERATED_KEYS);
        //        statement.setString(1, parametres.get(0));
        //        statement.executeUpdate();
        //        ResultSet rs = statement.getGeneratedKeys();
        //        int newId = 0;
        //        if (rs.next())
        //        {
        //            newId = rs.getInt(1);
        //        }
        //        for (int i = 1; i < parametres.size(); i++)
        //        {
        //            String[] lesParamsDuCours = parametres.get(i).split(";");
        //            String sqlInsertCours = "insert into cours(codecours, libellecours, nbjours, idcategorie)"
        //                    + " values (?,?,?,?)";
        //            PreparedStatement statementCours = this.connection.prepareStatement(sqlInsertCours);
        //            statementCours.setString(1, lesParamsDuCours[0]);
        //            statementCours.setString(2, lesParamsDuCours[1]);
        //            statementCours.setInt(3, Integer.parseInt(lesParamsDuCours[2]));
        //            statementCours.setInt(4, newId);
        //            statementCours.executeUpdate();
        //        }

        //        this.connection.commit();
        //    }
        //		catch (SQLException ex) {
        //        System.out.println(ex.getMessage());
        //        this.connection.rollback();
        //    }

        //}

    }
}
