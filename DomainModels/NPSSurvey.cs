using SurveyJSAsFormLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SurveyJSAsFormLibrary.DomainModels
{
    [DomainModelForm("nps-survey", "NPS Survey")]
    public class NPSSurvey: DomainModel
    {
        public int NPSScore { get; set; }
        public string DisappointingExperience { get; set; }
        public string ImprovementsRequired { get; set; }
        public List<string> PromoterFeatures { get; set; }
        public bool Rebuy { get; set; }
        public string Testimonial { get; set; }
        public string Email { get; set; }
    }
}
