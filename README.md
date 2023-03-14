# Generate Forms from Domain Models with SurveyJS

This example demonstrates how to generate forms in JSON format based on strongly-typed domain models and vice versa. The generated forms can be displayed by SurveyJS Form Library and edited in Survey Creator. This solution will be beneficial for content and product managers who regularly create forms and for backend developers who implement domain models based on these forms.

<!-- TODO: Add illustration -->

This application was built using ASP.NET Core. Follow the same instructions if you need to implement this functionality with any other server-side framework.

- [Domain Models and Attributes](#domain-models-and-attributes)
- [Generate Form JSON Schemas from Server-Side Domain Models](#generate-form-json-schemas-from-server-side-domain-models)
- [Display a Form](#display-a-form)
- [Edit JSON Schemas](#edit-json-schemas)
- [Generate Domain Model Code Based on Form JSON Schemas](#generate-domain-model-code-based-on-form-json-schemas)

## Domain Models and Attributes

Domain models are declared in server-side code. You can generate form JSON schemas based on them. Domain model properties become form fields. You can then feed the JSON schemas into SurveyJS Form Library to display a form on your website or in your application. This project contains three sample domain models, each describes an individual form:

- [`JobApplication`](/DomainModels/JobApplication.cs)
- [`NPSSurvey`](/DomainModels/NPSSurvey.cs)
- [`PatientAssessment`](/DomainModels/PatientAssessment.cs)

A domain model and its properties can include [attributes](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/). A form generator uses attribute parameters to generate form JSON schemas. For instance, all domain models have a `DomainModelForm` attribute shown below. Its parameters configure a form's `name` and `title` properties.

```csharp
[DomainModelForm("nps-survey", "NPS Survey (Domain Model without attributes)")]
public class NPSSurvey: DomainModel {
    // ...
}
```

A form generator can support predefined .NET attributes, such as [`Required`](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.requiredattribute) or [`Display`](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.displayattribute), as well as custom attributes, like `DomainModelForm`. The following file shows how this project implements custom attributes: [FormAttributes.cs](/Code/FormAttributes.cs).

You can get a list of all domain models in the running assembly. Call the `DomainModelList.GetAllForms()` static method. It finds all classes that are inherited from `DomainModel` and have the `DomainModelForm` attribute. Refer to the following file for full code: [DomainModelList.cs](/DomainModels/DomainModelList.cs).

## Generate Form JSON Schemas from Server-Side Domain Models

Form JSON schemas are generated based on property type and attributes. Each public writable property in a `DomainModel` class becomes a form field in the form JSON schema. Refer to the [`JSONGeneratorByModelClass`](/DomainModelsViews/JsonFormGenerator.cs#L13) class to view the form generator code. The class implements a `Generate()` method that extracts all properties from a domain model, gets their type and attributes, and uses this information to build a JSON schema. You can extend the form generator's functionality: implement new custom attributes or add more properties to the existing attributes.

## Display a Form

SurveyJS ships with a tool that displays surveys and forms&mdash;[SurveyJS Form Library](https://surveyjs.io/form-library). You no longer need to create a separate page for each form you have in your application. Set up SurveyJS Form Library and assign different JSON schemas to it to display different forms. This tool supports all most popular JavaScript frameworks, including React, Angular, and Vue. However, this project uses the Form Library version for Knockout because it is the easiest version to set up. The [FormResponse.cshtml](/Views/Home/FormResponse.cshtml) file shows the Form Library configuration code.

In this project, SurveyJS Form Library displays JSON schemas that come from different sources. The "NPS Survey" and "Patient Assessment" forms are pre-generated and stored as JSON files in the [Data](/Data/) directory. The "Job Application" form is generated from the `JobApplication` domain model on the fly. If a form JSON schema has been edited, its most recent version is stored in a database (or [database emulator](/DomainModels/DataStorage.cs), as in this application). When Form Library requests a JSON schema of a certain type, the server first looks into the database to find the most recently edited schema of that type. If the schema is not found, the server returns a pre-generated schema from one of the JSON files. If a file with a schema of that type is also absent, the server generates the schema on the fly. Refer to the following file to find methods that implement this logic: [JsonForms.cs](/DomainModelsViews/JsonForms.cs).

<!-- TODO: Add illustration -->

## Edit JSON Schemas

JSON schemas can be edited in any text editor, but they are easier to edit in [Survey Creator](https://surveyjs.io/survey-creator). This JavaScript component is a UI form designer by SurveyJS that content and product managers can use to create and modify JSON schemas without writing code. Similar to SurveyJS Form Library, Survey Creator supports React, Angular, Vue, Knockout, and jQuery and is easy to integrate into your application. The project in this repository sets up the Knockout version (view the [EditForm.cshtml](/Views/Home/EditForm.cshtml) file).

If in your team more than one person creates and edits forms, you can configure multiple roles with different access rights. For example, you may define two roles: content manager and product manager. Content managers cannot create new forms and can edit only those form properties that do not require server-side code modification. These restrictions ensure the synchronization between domain models and form JSON schemas. Product managers on the other hand have unlimited capabilities and can request the backend development team to update domain models on the server. The role separation is implemented at the Survey Creator level. Refer to the [`creatorjs.js`](/wwwroot/js/creatorjs.js) file to see the implementation. You can also view the restricted UI for content managers in the following demo: [Setup for Content Managers](https://surveyjs.io/survey-creator/examples/setup-for-content-manager/).

To make edited schemas available to the users, you do not need to wait until the backend development team rebuilds and redeploys the entire application. Edited schemas are saved in a database, and you can implement the functionality to prioritize them when SurveyJS Form Library loads a schema for display (see the [Display a Form](#display-a-form) section above). As a result, users can use an edited form immediately after the content or product manager modifies it. If the schema modification requires an update of domain models to maintain the model&ndash;schema synchronization, the backend development team may perform this update and rebuild and redeploy the application according to their own schedule.

## Generate Domain Model Code Based on Form JSON Schemas

Previously, you saw how to [implement a form generator](#generate-form-json-schemas-from-server-side-domain-models) that creates JSON schemas for client-side forms based on server-side domain models. You can also implement code that does the opposite&mdash;generates domain model code based on JSON schemas. This capability saves time for backend developers who implement domain models based on JSON schemas created by product managers.

To implement a domain model generator, you need to parse the JSON schema being edited and convert the form fields to model properties. Users can view the generated domain model code under the Domain Model Code tab when they edit a form in Survey Creator as a product manager. View the following demo to see this functionality in action: [Create Domain Models](https://surveyjs.io/survey-creator/examples/create-domain-models/). Refer to the following file for full code: [codegenerator.js](/wwwroot/js/codegenerator.js).

## Useful Links

- [SurveyJS Website](https://surveyjs.io/)
- [Form Library Documentation](https://surveyjs.io/form-library/documentation/overview)
- [Form Library Demos](https://surveyjs.io/form-library/examples/nps-question/)
- [Survey Creator Documentation](https://surveyjs.io/survey-creator/documentation/overview)
- [Survey Creator Demos](https://surveyjs.io/survey-creator/examples/free-nps-survey-template/)