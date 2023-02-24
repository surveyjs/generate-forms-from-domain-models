using System.ComponentModel.DataAnnotations;

namespace SurveyJSAsFormLibrary.DomainModels
{
    public class DomainModel
    {
        [Display(AutoGenerateField = true)]
        public string Id { get; set; }
        public override string ToString()
        {
            return this.Id;
        }
    }
}