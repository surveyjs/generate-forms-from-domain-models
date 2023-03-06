//Add survey name property and set the maximum length for question and column names
Survey.Serializer.addProperty("survey", { name: "name", maxLength: 50, required: true, category: "general", visibleIndex: 0 });
Survey.Serializer.findProperty("question", "name").maxLength = 50;
Survey.Serializer.findProperty("matrixdropdown", "name").maxLength = 50;

function getCreatorOptions(isAdmin) {
    return {
        showLogicTab: true,
        isAutoSave: true,
        showTranslationTab: true,
        showJSONEditorTab: isAdmin
    };
}
function setupCreator(creator, formName, isAdmin) {
    creator.saveSurveyFunc = (saveNo, callback) => {
        saveForm(formName, creator.text);
    }
    creator.readOnly = true;
    SurveyCreator.settings.designer.showAddQuestionButton = isAdmin;
    if (isAdmin) {
        setupCreatorForProductManager(creator);
    }
    else {
        setupCreatorForContentManager(creator);
    }
    loadForm(formName, creator);
}
function setupCreatorForContentManager(creator) {
    const panelItem = creator.toolbox.getItemByName("panel");
    //We allow to add only Panel. You can hide toolbox by calling `creator.showToolbox = false;`
    creator.toolbox.addItems([panelItem], true);
    //The default question to add is panel
    creator.currentAddQuestionType = "panel";
    //Hide Add Question button from the designer surface
    creator.showAddQuestionButton = false;
    creator.onElementAllowOperations.add((sender, options) => {
        if (!options.obj.isQuestion) return;
        //We not allow to change question type, delete a question or copy it.
        options.allowChangeType = false;
        options.allowCopy = false;
        options.allowDelete = false;
    });
    creator.onCollectionItemAllowOperations.add((sender, options) => {
        //Do not allow to delete a column
        options.allowDelete = !isObjColumn(options.item)
    });
    creator.onSetPropertyEditorOptions.add((sender, options) => {
        //Do not allow to add/remove colums
        options.editorOptions.allowAddRemoveItems = options.propertyName !== "columns";
    });
    creator.onGetPropertyReadOnly.add((sender, options) => {
        //Do not allow change cell question type in colum
        if (options.property.name === "cellType") {
            options.readOnly = true;
        }
        //Do not allow to modify property name for a question and a column
        if (options.property.name === "name") {
            options.readOnly = options.obj.isQuestion || isObjColumn(options.obj);
        }
    });
}
function setupCreatorForProductManager(creator) {
    ko.components.register("svc-tab-servercode", {
        viewModel: {
            createViewModel: (params, componentInfo) => {
                const creator = params.creator;
                var model = {
                    generatedCode: generateDomainModelsCode(creator.survey)
                };
                return model;
            }
        },
        template: `
<textarea data-bind="value:generatedCode" style="width:100%;height:100%;padding:'5px'">
</textarea>
`
    });
    creator.onPropertyValidationCustomError.add((sender, options) => {
        if (options.propertyName !== "name") return;
        //We need to validate the name property for a question, the survey and a matrix column
        if (options.obj.isQuestion || ["survey", "matrixdropdowncolumn"].indexOf(options.obj.getType()) > -1) {
            if (!isNameCorrect(options.value)) {
                options.error = "The current name can't be used as a property in the server code. Please correct it.";
            }
        }
    })
    const templatesPlugin = {
        activate: () => { },
        deactivate: () => { return true; }
    };
    //Add plug-in. We do nothing on activate/deactivate. Place it as first tab and set to "svc-tab-servercode" the component name
    creator.addPluginTab("servercode", templatesPlugin, "Domain Models Code", "svc-tab-servercode", 0);
}
function isObjColumn(obj) {
    return !!obj && obj.getType() === "matrixdropdowncolumn";
}