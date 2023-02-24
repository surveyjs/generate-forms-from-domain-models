using SurveyJSAsFormLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SurveyJSAsFormLibrary.DomainModels
{
    public class PersonInformation: DomainModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required] 
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        public override string ToString()
        {
            string res = FirstName;
            if(!string.IsNullOrEmpty(LastName))
            {
                if (!string.IsNullOrEmpty(res)) res += ' ';
                res += LastName;
            }
            return !string.IsNullOrEmpty(res) ? res : base.ToString();
        }

    }
}
