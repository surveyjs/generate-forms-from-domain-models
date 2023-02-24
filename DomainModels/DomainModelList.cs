using SurveyJSAsFormLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SurveyJSAsFormLibrary.DomainModels
{
    public class DomainModelInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public string Title { get; set; }
    }
    public class DomainModelList
    {
        private static Dictionary<string, DomainModelInfo> domainsValue;
        private static Dictionary<string, DomainModelInfo> domains
        {
            get {
                if(domainsValue == null)
                {
                    domainsValue = new Dictionary<string, DomainModelInfo>();
                    loadDomainList();
                }
                return domainsValue;
            }
        }
        private static void loadDomainList()
        {
            Assembly domainAssembly = Assembly.GetExecutingAssembly();
            foreach (Type type in domainAssembly.GetTypes())
            {
                if (!typeof(DomainModel).IsAssignableFrom(type)) continue;

                foreach (CustomAttributeData attr in type.CustomAttributes)
                {
                    if (attr.AttributeType == typeof(DomainModelFormAttribute))
                    {
                        string name = (string)attr.ConstructorArguments[0].Value;
                        string title = (string)attr.ConstructorArguments[1].Value;

                        domainsValue[name] = new DomainModelInfo() { Name = name, Type = type, Title = getTitle(name, title) };
                    }
                }
            }
        }
        private static string getTitle(string name, string title)
        {
            if (!string.IsNullOrEmpty(title)) return title;
            string[] words = name.Split("-");
            string res = "";
            foreach(string word in words)
            {
                res += char.ToUpper(word[0]) + word.Substring(1) + ' ';
            }
            return res + "Form";
        }

        public static Type GetTypeByFormName(string name)
        {
            DomainModelInfo res;
            domains.TryGetValue(name, out res);
            return res != null ? res.Type : null;
        }
        public static IList<DomainModelInfo> GetAllForms()
        {
            var list = new List<DomainModelInfo>();
            foreach(DomainModelInfo info in domains.Values)
            {
                list.Add(info);
            }
            return list;
        }
    }
}