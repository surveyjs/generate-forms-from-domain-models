using SurveyJSAsFormLibrary.Code;
using SurveyJSAsFormLibrary.DomainModels;
using System;
using System.Collections.Generic;

namespace SurveyJSAsFormLibrary.Models
{
    public class FormListModel
    {
        IList<DomainModelInfo> modelsInfo;
        public FormListModel()
        {
            this.modelsInfo = DomainModelList.GetAllForms();
        }
        public IList<DomainModelInfo> FormList {  get { return modelsInfo; } }
        public IList<DomainModel> GetObjectsByType(string formName)
        {
            return new JSONForm(formName).GetAllObjects();
        }
    }
}