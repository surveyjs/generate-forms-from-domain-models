# SurveyJS as front-end form library for ASP.Net Core app

The examples show how to use SurveyJS Library to edit Domain Models defined as C# classes.
You can edit JSON Form Definition in our SurveyJS Creator and your end-user will see
a different form for editing/filling forms without an application re-building/re-deploying.

There are 3 Domain Classes in this example:
 * [JobApplication](/DomainModels/JobApplication.cs)
 * [NPSSurvey](/DomainModels/NPSSurvey.cs)
 * [PatientAssessment](/DomainModels/PatientAssestment.cs)

## Mock database
To make an example simple, the current implementation doesn't work with a database.
DomainModel objects are stored in the memory by using a singleton [DataStorage](/DomainModels/DataStorage.cs) class.
This singleton stores and retrieve data by class type and object Id (Unique string) 
and serialize/deserialize objects by using MS .Net JsonSerializer Serialize/Deserialize functions.
You can change our [DataStorage](/DomainModels/DataStorage.cs) class to write the code that will work with any database of your choice.
As an alternative, you can use an ORM library.

## Form List. Register a new form / DomainModel.
To register a [DomainModelFormAttribute](/Code/FormAttributes) class attribute is added.
It has two parameters: "Name" and "Title".
Here is the example of registering a new form:
````csharp
    [DomainModelForm("nps-survey", "NPS Survey (Domain Model without attributes)")]
    public class NPSSurvey: DomainModel {
        ...
    }
````
The [DomainModelList](/DomainModels/DomainModelList.cs) can return the list of all registered DomainModel classes (forms)
by calling static function `DomainModelList.GetAllForms();`. It get all classes in the running assembly
and check all classes inherited from "DomainModel" with "DomainModelForm" attribute.
You can get a DomainModel type by form name by calling a static function `DomainModelList.GetTypeByFormName(string formName);`.

## Form Response View to fill out forms
[Form Response View](/Views/Home/FormResponse.cshtml) shows SurveyJS runner to fill out a form.
It uses SurveyJS Library for knockout to render the form. We use it instead of React or Angular or Vue versions, because of simplicity.
The page contains minimim code related to JavaScript framework implementation.
The survey runner shows on loading JSON form definition and survey data, by calling [loadFormAndData](/wwwroot/js/form_api) function.
The form (survey) model is setup in [setupSurveyModel](/wwwroot/js/surveyjs.js) function. 
Inside this function, on submitting form the [saveFormData](wwwroot/js/form_api.js) function is calling.

## Getting JSON form definition
There are two API server functions [loadform and loadform_and_data](/Controllers/FormController.cs).
They return the JSON form definition by form name and the second function returns a previous entered form response by Id.
The previous entered response data is getting by using our [DataStorage](/DomainModels/DataStorage.cs).
JSON form definition is returning by the following rule. If there is a JSON with the same form name in "Data" directory then the context of this file is returned.
If there is no such file in the directory, then the JSON form definition is generated on fly by DomainModel class corresponded with this form name.

## Generating JSON by DomainModel
By default the JSON form definition is generated based on DomainModel.
[JsonFormGenerator](/DomainModelsViews/JsonFormGenerator.cs) class generates JSON by using .Net reflection API.
Every public writable property in DomainModel is corresponded with a question in Form (Survey) JSON.
Generator class uses property type and property attributes to generate the correct JSON.
You can use .Net standard "Required" and "DisplayAttributes" as well as custom [FormPage and FormField](/Code/FormAttributes.cs) attributes.
You can improve our current JSON generator class by adding new custom attributes or by adding more properties into our attributes.

## Editing generating and existing Form JSONs
You can edit default JSON Form Definition on [EditForm](/Views/Home/EditForm.cshtml) page.
It loads form definition located in "Data" directory or generated by [JsonFormGenerator](/DomainModelsViews/JsonFormGenerator.cs) class
and store changes in [DataStorage](/DomainModels/DataStorage.cs). After changing the JSON Form Definition, Form Response page will use new updated JSON for form editing.
There are two modes for editing JSON Form Definition: admin and content manager.
The first one has no limitation. Admin can add/delete any question/columns and so on.
The mode for content manager doesn't allow to add/delete remove question or column or change question/column type or change their names.
It means that content manager is not able to make form definition incorrect. It can change question title or change question location,
but every question will be corresponded with a correct DomainModel property. 