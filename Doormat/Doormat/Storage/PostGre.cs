﻿using System;
using System.Collections.Generic;
using System.Text;
using DoormatCore.Games;
using DoormatCore.Helpers;
using DoormatCore.Sites;
using Npgsql;
using System.Reflection;
using static DoormatCore.Sites.BaseSite;

namespace DoormatCore.Storage
{
    class PostGre : SQLBase
    {
        NpgsqlConnection Connection = new NpgsqlConnection();

        public PostGre(string ConnectionString) : base(ConnectionString)
        {
            Logger.DumpLog("Create PostGre Connection", 6);
            Connection = new NpgsqlConnection(ConnectionString);
            Connection.Open();
        }
        public override SessionStats AddSessionStats(SessionStats Stats)
        {
            return PerformInsert<SessionStats>(Stats);
        }

        public override DiceBet GetBet(int InternalId)
        {
            return PerformGet<DiceBet>(InternalId);
        }

        public override DiceBet GetBet(string BetId)
        {
            DiceBet[] bets = PerformFind<DiceBet>("BetID=@1", "", BetId);
            if (bets.Length > 0)
                return bets[0];
            return null;
        }

        public override DiceBet[] GetBets()
        {
            return PerformFind<DiceBet>("");
        }

        public override DiceBet[] GetBets(Site Site)
        {
            return PerformFind<DiceBet>("Site=@1", "", Site.Id);
        }

        public override DiceBet[] GetBets(User User)
        {
            return PerformFind<DiceBet>("Site=@1", "", User.Id);
        }

        public override Currency[] GetCurrenciesForSite(Site Site)
        {
            return PerformFind<Currency>("Site=@1", "", Site.Id);
        }

        public override Currency GetCurrency(int Id)
        {
            return PerformGet<Currency>(Id);
        }

        public override Currency GetCurrency(string Nameorsymbol)
        {
            Currency[] Currencies = PerformFind<Currency>("Name=@1 or Symbol=@1", "", Nameorsymbol);
            if (Currencies.Length > 0)
                return Currencies[0];
            return null;
        }

        public override Seed GetSeed(int Id)
        {
            return PerformGet<Seed>(Id);
        }

        public override Seed GetSeed(string ServerSeedHash, string ServerSeed = null)
        {
            Seed[] Currencies = PerformFind<Seed>("ServerSeedHash=@1 and (ServerSeed=@2 or ServerSeed is NULL)", "", ServerSeedHash, ServerSeed);
            if (Currencies.Length > 0)
                return Currencies[0];
            return null;
        }

        public override Seed[] GetSeeds()
        {
            return PerformFind<Seed>("");
        }

        public override SessionStats[] GetSessionStats()
        {
            return PerformFind<SessionStats>("");
        }

        public override SessionStats[] GetSessionStats(Site Site)
        {
            return PerformFind<SessionStats>("Site=@1", "", Site.Id);
        }

        public override SessionStats[] GetSessionStats(User User)
        {
            return PerformFind<SessionStats>("User=@1", "", User.Id);
        }

        public override Site GetSite(int Id)
        {
            return PerformGet<Site>(Id);
        }

        public override Site GetSite(string SiteName)
        {
            Site[] Sites = PerformFind<Site>("Name=@1", "", SiteName);
            if (Sites.Length > 0)
                return Sites[0];
            return null;
        }

        public override Site GetSites(bool ActiveOnly)
        {
            /*Site[] Sites = PerformFind<Site>("Name=@1", "", SiteName);
            if (Sites.Length > 0)
                return Sites[0];
            return null;
            */
            throw new NotImplementedException();
        }

        public override User GetUser(int Id)
        {
            return PerformGet<User>(Id);
        }

        public override User GetUserById(string Id)
        {
            User[] Users = PerformFind<User>("UserId=@1", "", Id);
            if (Users.Length > 0)
                return Users[0];
            return null;
        }

        public override User GetUserByName(string Name)
        {
            User[] Users = PerformFind<User>("UserName=@1", "", Name);
            if (Users.Length > 0)
                return Users[0];
            return null;
        }


        public override DiceBet UpdateBet(DiceBet NewBet)
        {
            return PerformUpdate<DiceBet>(NewBet);
        }

        public override Currency UpdateCurrency(Currency NewCurrency)
        {
            return PerformUpdate<Currency>(NewCurrency);
        }


        public override Seed UpdateSeed(Seed NewSeed)
        {
            return PerformUpdate<Seed>(NewSeed);
        }

        public override Site UpdateSite(Site NewSite)
        {
            return PerformUpdate<Site>(NewSite);
        }

        public override User UpdateUser(User NewUser)
        {
            return PerformUpdate<User>(NewUser);
        }

        public override BaseSite.LoginParamValue UpdateValue(BaseSite.LoginParamValue NewValue)
        {
            return PerformUpdate<LoginParamValue>(NewValue);
        }


        protected override void CreateBetT()
        {
            CreateTable(typeof(DiceBet));
        }

        protected override void CreateCurrencyT()
        {
            CreateTable(typeof(Currency));
        }

        protected override void CreateLoginParamT()
        {
            CreateTable(typeof(LoginParamValue));
        }

        protected override void CreateSeedT()
        {
            CreateTable(typeof(Seed));
        }

        protected override void CreateSessionStatsT()
        {
            CreateTable(typeof(SessionStats));
        }

        protected override void CreateSiteT()
        {
            CreateTable(typeof(Site));
        }

        protected override void CreateUserT()
        {
            CreateTable(typeof(User));
        }
        public override string GetConnectionString()
        {
            throw new NotImplementedException();
        }


        public override LoginParameter UpdateParameter(LoginParameter LoginParameter)
        {
            throw new NotImplementedException();
        }

        public override LoginParameter GetParameter(int Id)
        {
            throw new NotImplementedException();
        }


        string GetDBType(string TypeName)
        {
            string DBName = "";
            switch (TypeName.ToLower())
            {
                case "decimal": DBName = "decimal(35,20)"; break;
                case "double": DBName = "decimal(35,20)"; break;
                case "int": DBName = "int"; break;
                case "long": DBName = "bigint"; break;
                case "string": DBName = "nvarchar(500)"; break;
                case "byte": DBName = ""; break;

                default: DBName = "int"; break;
            }
            return DBName;
        }

        private void CreateTable(Type type)
        {


            string TableName = type.GetCustomAttribute<PersistentTableName>().TableName;

            NpgsqlCommand CheckTableExists = new NpgsqlCommand("select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA='dbo' and TABLE_NAME='" + TableName + "'", Connection);
            NpgsqlDataReader Reader = CheckTableExists.ExecuteReader();
            if (Reader.HasRows)
            {
                List<string> Columns = new List<string>();
                while (Reader.Read())
                {
                    Columns.Add(Reader[0].ToString());
                }

                foreach (PropertyInfo PI in type.GetProperties())
                {
                    if (!PI.PropertyType.IsArray)
                    {
                        bool found = false;
                        foreach (string x in Columns)
                        {
                            if (PI.Name.ToLower() == x.ToLower())
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            string Query = "alter table " + TableName + " add " + PI.Name + " " + GetDBType(PI.PropertyType.Name);
                            NpgsqlCommand AddColumn = new NpgsqlCommand(Query, Connection);
                            AddColumn.ExecuteNonQuery();
                        }
                    }
                }

            }
            else
            {
                string query = "Create table dbo." + TableName + "(id int identity(1,1) primary key";
                foreach (PropertyInfo PI in type.GetProperties())
                {
                    if (!PI.PropertyType.IsArray)
                    {
                        if (PI.Name.ToLower() != "id")
                        {
                            query += ", " + PI.Name + " " + GetDBType(PI.PropertyType.Name);

                        }
                    }
                }
                query += ")";
                NpgsqlCommand CreateTable = new NpgsqlCommand(query, Connection);
                CreateTable.ExecuteNonQuery();
            }
        }

        private string ConstructSelect(Type type)
        {
            string TableName = type.GetCustomAttribute<PersistentTableName>().TableName;

            string query = "SELECT ";
            bool first = true;
            foreach (PropertyInfo PI in type.GetProperties())
            {
                if (!PI.PropertyType.IsArray)
                {
                    if (!first)
                    {
                        query += ", ";
                    }
                    first = false;
                    query += "[" + TableName + "].[" + PI.Name + "]";
                }
            }
            query += " FROM " + "[" + TableName + "] ";
            return query;
        }

        private T PerformInsert<T>(T ValueToInsert) where T : PersistentBase
        {
            Type type = ValueToInsert.GetType();

            string TableName = type.GetCustomAttribute<PersistentTableName>().TableName;
            string query = "SELECT INTO [" + TableName + "](";
            string values = " output INSERTED.ID VALUES(";
            bool first = true;

            int i = 1;

            NpgsqlCommand tmpCommand = new NpgsqlCommand();

            foreach (PropertyInfo PI in type.GetProperties())
            {
                if (!PI.PropertyType.IsArray && PI.Name.ToLower() != "id")
                {
                    if (!first)
                    {
                        query += ", ";
                        values += ",";
                    }
                    first = false;
                    query += "[" + PI.Name + "]";
                    values += "@" + i.ToString();
                    tmpCommand.Parameters.AddWithValue("@" + i++.ToString(), PI.GetValue(ValueToInsert));
                }
            }
            query += ") ";
            tmpCommand.CommandText = query + values;
            //tmpCommand.Connection = SqlConnection //Set sql connection here
            ValueToInsert.Id = (int)tmpCommand.ExecuteScalar();
            return ValueToInsert;
        }

        private T PerformUpdate<T>(T ValueToUpdate) where T : PersistentBase
        {
            if (ValueToUpdate.Id <= 0)
                return PerformInsert<T>(ValueToUpdate);
            Type type = ValueToUpdate.GetType();
            string TableName = type.GetCustomAttribute<PersistentTableName>().TableName;

            string query = "UPDATE [" + TableName + "] set";
            bool first = true;

            int i = 1;
            NpgsqlCommand tmpCommand = new NpgsqlCommand();

            foreach (PropertyInfo PI in type.GetProperties())
            {
                if (!PI.PropertyType.IsArray && PI.Name.ToLower() != "id")
                {
                    if (!first)
                    {
                        query += ", ";
                    }
                    first = false;
                    query += "[" + PI.Name + "] = @" + i.ToString();

                    tmpCommand.Parameters.AddWithValue("@" + i++.ToString(), PI.GetValue(ValueToUpdate));
                }
            }
            query += " WHERE [" + TableName + "].Id = @" + i.ToString();
            tmpCommand.Parameters.AddWithValue("@" + i++.ToString(), ValueToUpdate.Id);
            tmpCommand.CommandText = query;
            //tmpCommand.Connection = SqlConnection //Set sql connection here
            ValueToUpdate.Id = (int)tmpCommand.ExecuteScalar();

            return ValueToUpdate;
        }

        private T[] PerformFind<T>(string Criteria, string Sorting = "", params object[] SqlParams) where T : PersistentBase, new()
        {
            string Select = ConstructSelect(typeof(T));
            if (string.IsNullOrWhiteSpace(Criteria))
                Select += " WHERE " + Criteria;
            if (string.IsNullOrWhiteSpace(Sorting))
                Select += "ORDER BY " + Sorting;
            List<T> results = new List<T>();
            NpgsqlCommand SelectCommand = new NpgsqlCommand(Select/*, SqlConnection*/);
            for (int i = 0; i < SqlParams.Length; i++)
            {
                SelectCommand.Parameters.AddWithValue("@" + i + 1, SqlParams[i]);
            }

            NpgsqlDataReader tmpReader = SelectCommand.ExecuteReader();
            while (tmpReader.Read())
            {
                results.Add(ParseResult<T>(tmpReader));
            }
            return results.ToArray();
        }

        private T PerformGet<T>(int Id) where T : PersistentBase, new()
        {
            T Result = null;
            string Select = ConstructSelect(typeof(T));
            Select += " WHERE [ID]=@1";
            NpgsqlCommand SelectCommand = new NpgsqlCommand(Select/*, SqlConnection*/);
            SelectCommand.Parameters.AddWithValue("@1", Id);
            NpgsqlDataReader tmpReader = SelectCommand.ExecuteReader();
            if (tmpReader.HasRows)
            {
                Result = ParseResult<T>(tmpReader);
            }
            return Result;

        }
    }
}
