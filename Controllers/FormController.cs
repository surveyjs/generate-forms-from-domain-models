using Microsoft.AspNetCore.Mvc;
using System;
using SurveyJSAsFormLibrary.Code;
using SurveyJSAsFormLibrary.DomainModels;

namespace SurveyJSAsFormLibrary.Controllers
{
    public class LoadFormData
    {
        public string name { get; set; }
        public string id { get; set; }
    }
    public class SaveFormData
    {
        public string name { get; set; }
        public string data { get; set; }
    }
    public class DomainModelResponse: LoadFormData
    {
        public string data { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class FormController : ControllerBase
    {
        [HttpGet("loadform")]
        public JsonResult LoadFormJSON(string name)
        {
            JSONForm form = new JSONForm(name);
            if(!form.IsValid) return generateFormNotFound(name);
            return new JsonResult(form.GetFormJSON());
        }
        [HttpGet("loaddata")]
        public JsonResult LoadFormData([FromBody] LoadFormData data)
        {
            JSONForm form = new JSONForm(data.name);
            if (!form.IsValid) return generateFormNotFound(data.name);
            return new JsonResult(form.LoadDomainModel(data.id));
        }
        [HttpGet("loadform_and_data")]
        public JsonResult LoadFormJSONAndData(string name, [FromQuery] string id)
        {
            JSONForm form = new JSONForm(name);
            if (!form.IsValid) return generateFormNotFound(name);
            string objData = form.GetModelAsDataString(id);
            dynamic res = new { form = form.GetFormJSON(), data = objData };
            return new JsonResult(res);
        }
        [HttpPost("savedata")]
        public JsonResult SetFormData([FromBody] DomainModelResponse data)
        {
            JSONForm form = new JSONForm(data.name);
            if (!form.IsValid) return generateFormNotFound(data.name);
            form.SaveDomainModelData(data.id, data.data);
            return new JsonResult(null);
        }
        [HttpPost("saveform")]
        public JsonResult SetFormJSON([FromBody] SaveFormData data)
        {
            JSONForm form = new JSONForm(data.name);
            if (!form.IsValid) return generateFormNotFound(data.name);
            form.SaveFormJSON(data.data);
            return new JsonResult(null);
        }
        private JsonResult generateNotFound(string text)
        {
            var res = new JsonResult(null);
            res.StatusCode = 404;
            res.Value = text;
            return res;
        }
        private JsonResult generateFormNotFound(string formName)
        {
            return generateNotFound("Form name is not found: '" + formName + "'");
        }
    }
}