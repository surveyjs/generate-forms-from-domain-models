using SurveyJSAsFormLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyJSAsFormLibrary.DomainModels
{
    [DomainModelForm("patient-assessment")]
    public sealed class PatientAssessment : PersonInformation
    {
        [Display(Name = "Social Security number")]
        public string SSN { get; set; }
        [FormField(FieldType = "comment")]
        [Display(Name = "List any concerns you want to talk about during your visit")]
        public string Concerns { get; set; }

        [FormPage(Title = "Health history")]
        [Display(Name = "Do you have diabetes?")]
        public bool Diabetes { get; set; }
        [Display(Name = "High blood pressure?")]
        public bool HighBloodPressure { get; set; }
        [Display(Name = "High cholesterol?")]
        public bool HighCholesterol { get; set; }
        [FormField(FieldType = "comment")]
        [Display(Name = "Do you have other health conditions?")]
        public string OtherHealthConditions { get; set; }

        [FormPage(Title = "Social history")]
        [Display(Name = "Do you smoke cigarettes?")]
        [FormField(FieldType = "radiogroup", Choices = new string[] { "never", "yes", "quit" })]
        public string Cigarettes { get; set; }
        [Display(Name = "How many packs a day?")]
        public int CigarettesPacksPerDay { get; set; }
        [Display(Name = "Date Quit")]
        public DateTime CigarettesDateQuit { get; set; }
        [Display(Name = "Years smoked")]
        public int CigarettesYearsSmoked { get; set; }
        [Display(Name = "Do you vape (e-cigarettes)?")]
        public bool CigarettesVape { get; set; }
        [Display(Name = "Do you drink alcohol?")]
        public bool Alcohol { get; set; }
        [Display(Name = "How many drinks per week?")]
        public int DrinksPerWeek { get; set; }
        [FormField(FieldType = "checkbox", Choices = new string[] { "rarely", "marijuana", "cocaine", "opioids" })]
        [Display(Name = "Do you use recreational drugs?")]
        public string RecreationalDrugs { get; set; }
        [Display(Name = "How many times per month")]
        public string DrugUseTimesPerMonth { get; set; }
        [FormField(Choices = new string[] { "high-school", "trade-school", "college", "post-graduate" })]
        [Display(Name = "What is your highest level of education completed?")]
        public string Education { get; set; }
        [FormField(Choices = new string[] { "married", "partnership", "divorced", "separated", "single", "widow" })]
        [Display(Name = "What is your marital status?")]
        public string MaritalStatus { get; set; }
        [Display(Name = "Are you sexually active?")]
        public bool SexuallyActive {get; set; }
        [Display(Name = "How many sexual partners do you have?")]
        public int SexualPartnersNumber { get; set; }
        [FormField(FieldType ="radiogroup", Choices = new string[] { "men", "women", "both" })]
        public string SexualPartnersGender { get; set; }
        [Display(Name = "Do you use contraception?")]
        public bool Contraception { get; set; }
        [FormField(FieldType = "comment")]
        [Display(Name = "What type of contraception do you use?")]
        public string ContraceptionComment { get; set; }
        [FormField(FieldType = "radiogroup", Choices = new string[] { "yes", "no", "retired" })]
        [Display(Name = "Are you employed?")]
        public string Employment { get; set; }
        [Display(Name = "Type of work")]
        public string EmploymentComment { get; set; }
        [Display(Name = "Do you exercise?")]
        public bool DoExercise { get; set; }
        [Display(Name = "Type of activity")]
        public string ExerciseActivityType { get; set; }
        [Display(Name = "How often?")]
        public string ExerciseActivityFrequency { get; set; }
        [Display(Name = "How long per activity?")]
        public string ExerciseActivityDuration { get; set; }
        [Display(Name = "Do you have children?")]
        public bool HaveChildren { get; set; }
        [Display(Name = "# of children")]
        public int ChildrenNumber { get; set; }
        [Display(Name = "Children Ages")]
        public string ChildrenAges { get; set; }

        [FormPage(Title = "Surgical history / recent hospitalizations")]
        [FormField(FieldType = "comment")]
        [Display(Name = "Date and type of surgery / procedure")]
        public string SurgeryDescription { get; set; }


        [FormPage(Title = "Family history")]
        [Display(Name = "Family history")]
        public List<FamilyHistoryItem> FamilyHistory { get; set; }

        [FormPage(Title = "Preventive care")]
        [Display(Name = "Recent shots from a doctor or pharmacist")]
        public List<ShotInfo> RecentShots { get; set; }
        [Display(Name = "Recent tests or procedures")]
        public List<TestInfo> RecentTests { get; set; }
        public List<SpecialistInfo> Specialists { get; set; }
        public List<MedicationInfo> Medications { get; set; }
        public List<AllergyInfo> Allergies { get; set; }
        
        [FormPage(Title = "Symptoms")]
        [Display(Name = "Please select any symptoms you have now or have had in the past month")]
        [FormField(FieldType ="tagbox", Choices = new string[] { "Fever", "Chills", "Feeling poorly",
            "Feeling tired", "Weight gain", "Weight loss", "Chest pain", "Heart pounding", "Fast pulse", "Slow pulse",
            "Leg pain with exercise", "Leg swelling", "Joint pain", "Neck pain", "Joint swelling", "Joint stiffness", "Muscle aches",
            "Back pain", "Sores", "Rash", "Itching", "Change in a mole", "Unusual growth/spot" })]
        public string[] Symptoms { get; set; }
        [Display(Name = "Today's date:")]
        public DateTime CurrentDate { get; set; }
    }
    public class FamilyHistoryItem
    {
        [Display(Name = "Relation")]
        public string Relation { get; set; }
        [Display(Name = "Health conditions")]
        public string HealthCondition { get; set; }
        [Display(Name = "Family history of cancer")]
        public string CancerHistory { get; set; }
    }
    public class ProcedureInfo
    {
        public DateTime Date { get; set; }
        public string Place { get; set; }
    }
    public class ShotInfo : ProcedureInfo
    {
        [FormField(Choices = new string[] { "flu", "shingles", "pneumonia", "tetanus", "other" })]
        [Required]
        public string Name { get; set; }
    }
    public class TestInfo : ProcedureInfo
    {
        [FormField(Choices = new string[] { "colonoscopy", "cologuard", "mammogram", "pap", "other" })]
        [Required]
        public string Name { get; set; }
    }
    public class SpecialistInfo
    {
        [Display(Name = "Provider's first and last name")]
        public string Provider { get; set; }
        public string Speciality { get; set; }
        [Display(Name = "Town/City")]
        public string City { get; set; }
    }
    public class MedicationInfo
    {
        public string Name { get; set; }
        public string Dose { get; set; }
        public int TimesPerDay { get; set; }
    }
    public class AllergyInfo
    {
        public string Type { get; set; }
        public string Reaction { get; set; }
    }
}