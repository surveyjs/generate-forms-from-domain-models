using Microsoft.AspNetCore.Http;
using SurveyJSAsFormLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SurveyJSAsFormLibrary.DomainModels
{
    public static class DatabaseEmulatorHttpContext
    {
        static IServiceProvider services = null;

        public static IServiceProvider Services
        {
            get { return services; }
            set
            {
                if (services != null)
                {
                    throw new Exception("Can't set once a value has already been set.");
                }
                services = value;
            }
        }
        public static HttpContext Current
        {
            get
            {
                IHttpContextAccessor httpContextAccessor = services.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                return httpContextAccessor?.HttpContext;
            }
        }

    }
    public class DataStorage
    {
        const string idsName = "ids";
        private static ISession currentSession {
            get { return DatabaseEmulatorHttpContext.Current.Session; }
        }
        private static string GetDataFromDatabaseEmulator(string key)
        {
            return currentSession.GetString(key);
        }
        private static void SetDataFromDatabaseEmulator(string key, string data)
        {
            currentSession.SetString(key, data);
            string ids = currentSession.GetString(idsName);
            if (string.IsNullOrEmpty(ids)) ids = string.Empty;
            key += ";";
            if(!ids.Contains(key))
            {
                currentSession.SetString(idsName, ids + key);
            }
        }
        public static IList<string> GetAllIdsByPrefixFromDatabaseEmulator(string keyPrefix)
        {
            var res = new List<string>();
            string ids = currentSession.GetString(idsName);
            if (string.IsNullOrEmpty(ids)) return res;
            string[] allKeys = ids.Split(';');
            foreach (string key in allKeys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith(keyPrefix))
                {
                    res.Add(GetDataFromDatabaseEmulator(key));
                }
            }
            return res;
        }
        public static string GetForm(Type type)
        {
            return GetDataFromDatabaseEmulator(DataStorage.formId(type));
        }
        public static void SaveForm(Type type, string data)
        {
            SetDataFromDatabaseEmulator(DataStorage.formId(type), data);
        }
        public static string GenerateId()
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < 10; i++) sb.Append(random.Next(10));
            return sb.ToString();
        }
        public static string GetData(Type type, string id)
        {
            string data = GetDataFromDatabaseEmulator(DataStorage.absoluteId(type, id));
            if (string.IsNullOrEmpty(data))
            {
                data = getJSONWithId(id);
            }
            return data;
        }
        public static void SaveData(Type type, string id, string data)
        {
            string key = DataStorage.absoluteId(type, id);
            SetDataFromDatabaseEmulator(key, data);
        }
        public static IList<string> GetAllDataByType(Type type)
        {
            return GetAllIdsByPrefixFromDatabaseEmulator(type.Name + "-");
        }
        private static string absoluteId(Type type, string id)
        {
            return type.Name + "-" + id;
        }
        private static string formId(Type type)
        {
            return type.Name + "-form";
        }
        private static string getJSONWithId(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                id = DataStorage.GenerateId();
            }
            return "{ \"Id\": \"" + id + "\"}";
        }
    }
}