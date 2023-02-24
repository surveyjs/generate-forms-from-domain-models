using SurveyJSAsFormLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyJSAsFormLibrary.DomainModels
{
    [DomainModelForm("job-application", "Job Application Form (auto generated)")]
    public sealed class JobApplication: PersonInformation
    {
        [FormField(ChoicesByUrl = "https://surveyjs.io/api/CountriesExample")]
        public string Country { get; set; }
        [Display(Name = "City/Town")]
        public string City { get; set; }
        [Display(Name = "Zip Code")]
        public string Zip { get; set; }
        [FormField(FieldType = "comment")]
        public string Address { get; set; }
        [FormField(InputType = "email")]
        public string Email { get; set; }
        [Display(Name = "Expected salary (in US dollars)")]
        public int Salary { get; set; }
        [FormField(Choices = new string[] { "frontend", "backend", "fullstack", "intern" })]
        public string Position { get; set; }
        [Display(Name = "Date available to start work")]
        public DateTime StartDate { get; set; }
    }
}