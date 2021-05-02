﻿using MySql.Data.MySqlClient;
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
        MySqlConnection connexionAdo;

        public EmployeMysql(string host, int port, string db, string login, string pwd)
        {
            this.host = host;
            this.port = port;
            this.db = db;
            this.login = login;
            this.pwd = pwd;
            string csMysql = String.Format("Server = {0}; Port = {1}; Database = {2}; " + "Uid = {3}; " + "Pwd = {4}", host, port, db, login, pwd);
            this.connexionAdo = new MySqlConnection(csMysql);
        }
        public void Ouvrir()
        {
            try
            {
                this.connexionAdo.Open();
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
                this.connexionAdo.Close();
            }
            catch (MySqlException ex)
            {

                Console.WriteLine(ex.Message);
            }
            
        }
        public void InserCategorie(string categorie)
        {
            string requete = @"insert into categorie(libelle) values (@categ);";
            string requeteId = @"select last_insert_id() from categorie;";
            try
            {
                MySqlCommand cmdMySql = new MySqlCommand(requete,this.connexionAdo);
                MySqlCommand cmdId = new MySqlCommand(requeteId, this.connexionAdo);
                cmdMySql.Parameters.AddWithValue("categ", categorie);
                cmdMySql.ExecuteNonQuery();
                var increment = cmdMySql.LastInsertedId;
                var incrementv2 = cmdId.ExecuteScalar();
                Console.WriteLine("Il y a une catégorie inséré et son identifiant est : "+increment);
                Console.WriteLine("derniere id v2 : " + incrementv2);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void InsereCategarieCours(List<String> parametres)
        {
            MySqlCommand cmdMySqlUne;
            MySqlCommand cmdMySqlDeux;
            MySqlTransaction transMySql = this.connexionAdo.BeginTransaction();
            string requeteUne = @"insert into categorie (libelle) values (@categ);";
            string requeteDeux = @"insert into cours (codecours, libellecours, nbjours, idcategorie) values (@codecours, @libellecours,@nbjours, @idcategfk);";
            try
            {
                cmdMySqlUne = new MySqlCommand(requeteUne, this.connexionAdo);
                cmdMySqlDeux = new MySqlCommand(requeteDeux, this.connexionAdo);
                //parametre de la première requete
                cmdMySqlUne.Parameters.AddWithValue("@categ", parametres[0]);
                var increment = cmdMySqlUne.LastInsertedId;

                //parametre de la seconde requete
                cmdMySqlDeux.Parameters.AddWithValue("@idcategfk", increment);
                for (int i = 1; i < parametres.Count; i++)
                {
                    String[] tablo = parametres[i].Split(';');

                    cmdMySqlDeux.Parameters.AddWithValue("@codecours",tablo[0]);
                    cmdMySqlDeux.Parameters.AddWithValue("@libellecours",tablo[1]);
                    cmdMySqlDeux.Parameters.AddWithValue("@nbjours", tablo[2]);
                    MySqlDataReader reader = cmdMySqlDeux.ExecuteReader();
                    while (reader.Read())
                    {
                        string affichage = "cours : " + reader.GetString(0) + " libelle : " + reader.GetString(1) + " durée : " + reader.GetInt16(2) + " jours  categorie : " + reader.GetInt16(3);
                        Console.WriteLine(affichage);
                    }
                    reader.Close();
                }
                transMySql.Commit();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
