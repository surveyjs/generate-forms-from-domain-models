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
    public class DataStorage
    {
        private static Dictionary<string, string> databaseEmulator = new Dictionary<string, string>();
        public static string GetForm(Type type)
        {
            string data;
            DataStorage.databaseEmulator.TryGetValue(DataStorage.formId(type), out data);
            return data;
        }
        public static void SaveForm(Type type, string data)
        {
            DataStorage.databaseEmulator[DataStorage.formId(type)] = data;
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
            string data;
            DataStorage.databaseEmulator.TryGetValue(DataStorage.absoluteId(type, id), out data);
            if (string.IsNullOrEmpty(data))
            {
                data = getJSONWithId(id);
            }
            return data;
        }
        public static void SaveData(Type type, string id, string data)
        {
            string key = DataStorage.absoluteId(type, id);
            DataStorage.databaseEmulator[key] = data;
        }
        public static IList<string> GetAllDataByType(Type type)
        {
            var res = new List<string>();
            foreach(KeyValuePair<string, string> item in databaseEmulator)
            {
                if(item.Key.StartsWith(type.Name + "-"))
                {
                    res.Add(item.Value);
                }
            }
            return res;
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