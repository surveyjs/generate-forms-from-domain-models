using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyJSAsFormLibrary.Attributes
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false)]
    sealed class DomainModelFormAttribute : Attribute
    {
        private string name;
        private string title;
        public DomainModelFormAttribute(string name, string title = "")
        {
            this.name = name;
            this.title = title;
        }
        public string Name { get { return name; } }
        public string Title { get { return title; } }
    }
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false)]
    sealed class FormPageAttribute : Attribute
    {
        public FormPageAttribute()
        {
        }
        public string Title { get; set; }
    }
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false)]
    sealed class FormFieldAttribute : Attribute
    {
        public FormFieldAttribute()
        {
        }
        public string FieldType { get; set; }
        public string InputType { get; set; }
        public string ChoicesByUrl { get; set; }
        public string[] Choices { get; set; }
    }
}
