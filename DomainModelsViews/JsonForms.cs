using SurveyJSAsFormLibrary.DomainModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SurveyJSAsFormLibrary.Code
{
    public class JSONForm
    {
        string formName;
        Type formType;
        public JSONForm(string formName)
        {
            this.formName = formName;
            this.formType = DomainModelList.GetTypeByFormName(this.formName);
        }
        public bool IsValid {  get { return this.FormType != null; } }
        public Type FormType { get { return this.formType; } }
        public dynamic GetFormJSON()
        {
            if(!this.IsValid) return null;
            // Get a form JSON schema of the current `FormType` from the database
            string json = DataStorage.GetForm(this.FormType);
            // If the JSON schema is not found, load a pre-generated JSON schema
            // from a file in the Data directory
            if (string.IsNullOrEmpty(json)) {
                json = this.getJsonFromData();
            }
            if(!string.IsNullOrEmpty(json))
                return JsonSerializer.Deserialize(json, typeof(Object));
            return new JSONGeneratorByModelClass(FormType).Generate();
        }
        public void SaveFormJSON(string data)
        {
            if (!this.IsValid) return;
            DataStorage.SaveForm(this.FormType, data);
        }
        public DomainModel CreateNew()
        {
            if (!this.IsValid) return null;
            DomainModel res = Activator.CreateInstance(this.FormType) as DomainModel;
            res.Id = DataStorage.GenerateId();
            return res;
        }
        public DomainModel LoadDomainModel(string id)
        {
            if (!this.IsValid) return null;
            if(string.IsNullOrEmpty(id) || id.ToString() == "new")
            {
                return CreateNew();
            }
            string data = DataStorage.GetData(this.FormType, id);
            return this.createModelFromString(data);
        }
        public string GetModelAsDataString(string id)
        {
            DomainModel obj = LoadDomainModel(id);
            if (obj == null) return string.Empty;
            return this.getObjAsJsonString(obj);
        }
        public void SaveDomainModelData(string id, string data)
        {
            DataStorage.SaveData(this.FormType, id, data);
        }
        public IList<DomainModel> GetAllObjects()
        {
            var res = new List<DomainModel>();
            if (!IsValid) return res;
            foreach(string data in DataStorage.GetAllDataByType(this.FormType))
            {
                res.Add(this.createModelFromString(data));
            }
            return res;
        }
        private string getJsonFromData()
        {
            string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); ;
            filePath = Path.Combine(filePath, "Data", this.formName + ".json");
            if (!File.Exists(filePath)) return string.Empty;
            return File.ReadAllText(filePath);
        }
        private DomainModel createModelFromString(string data)
        {
            return JsonSerializer.Deserialize(data, this.FormType, this.createSerializerOptions()) as DomainModel;
        }
        public string getObjAsJsonString(DomainModel obj)
        {
            return JsonSerializer.Serialize(obj, obj.GetType(), this.createSerializerOptions());
        }
        private JsonSerializerOptions createSerializerOptions()
        {
            var options = new JsonSerializerOptions();
            options.IgnoreNullValues = true;
            options.Converters.Add(new DateTimeConverter());
            return options;
        }
    }
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string data = reader.GetString();
            if (string.IsNullOrEmpty(data)) return DateTime.MinValue;
            DateTime res;
            if (DateTime.TryParse(data, out res)) return res;
            return DateTime.MinValue;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value == DateTime.MinValue)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
            }
        }
    }

}