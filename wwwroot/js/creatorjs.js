// Add the `name` property to the survey and limit the length for question and column names
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
    creator.saveSurveyFunc = () => {
        saveForm(formName, creator.text);
    }
    creator.readOnly = true;
    // Only product managers can add new questions
    SurveyCreator.settings.designer.showAddQuestionButton = isAdmin;
    if (isAdmin) {
        setupCreatorForProductManager(creator);
    }
    else {
        setupCreatorForContentManager(creator);
    }
    loadForm(formName, creator);
}
// Content managers can only make those changes in a form JSON schema that do not require server-side code modification.
function setupCreatorForContentManager(creator) {
    const panelItem = creator.toolbox.getItemByName("panel");
    // Allow content managers to add only panels. If you want to hide the entire Toolbox, set `creator.showToolbox = false;`
    creator.toolbox.addItems([panelItem], true);
    // Change the default question type to "panel"
    creator.currentAddQuestionType = "panel";
    // Hide the "Add Question" button from the design surface
    creator.showAddQuestionButton = false;
    creator.onElementAllowOperations.add((_, options) => {
        if (!options.obj.isQuestion) return;
        // Disallow content managers to change question types, delete questions, or copy them
        options.allowChangeType = false;
        options.allowCopy = false;
        options.allowDelete = false;
    });
    creator.onCollectionItemAllowOperations.add((_, options) => {
        // Disallow content managers to delete columns via adorners on the design surface
        options.allowDelete = !isObjColumn(options.item)
    });
    creator.onSetPropertyEditorOptions.add((_, options) => {
        // Disallow content managers to add or delete matrix columns via the Property Grid
        options.editorOptions.allowAddRemoveItems = options.propertyName !== "columns";
    });
    creator.onGetPropertyReadOnly.add((_, options) => {
        // Disallow content managers to change cell question type in matrixes
        if (options.property.name === "cellType") {
            options.readOnly = true;
        }
        // Disallow content managers to modify the `name` property for questions and matrix columns
        if (options.property.name === "name") {
            options.readOnly = options.obj.isQuestion || isObjColumn(options.obj);
        }
    });
}

const SERVER_CODE_TAB_COMPONENT_NAME = "svc-tab-servercode";

// Product managers can make any changes in a form JSON schema.
// If these changes require server-side code modification, backend developers can use generated code to adjust domain models.
function setupCreatorForProductManager(creator) {
    // Register a component that displays server-side code generated based upon the form JSON schema
    ko.components.register(SERVER_CODE_TAB_COMPONENT_NAME, {
        viewModel: {
            createViewModel: (params) => {
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
    creator.onPropertyValidationCustomError.add((_, options) => {
        if (options.propertyName !== "name") return;
        // Validate the `name` property for the survey, questions, and matrix columns
        if (options.obj.isQuestion || ["survey", "matrixdropdowncolumn"].indexOf(options.obj.getType()) > -1) {
            if (!isNameCorrect(options.value)) {
                options.error = "The current name cannot be used as a property in the server-side code. Please correct it.";
            }
        }
    });
    // An object that configures the behavior of the tab that displays server-side code.
    // No actions are performed when users select the tab (activate) or move away from it (deactivate).
    const templatesPlugin = {
        activate: () => { },
        deactivate: () => { return true; }
    };
    // Add the tab as a first tab to the Survey Creator
    creator.addPluginTab("servercode", templatesPlugin, "Domain Model Code", SERVER_CODE_TAB_COMPONENT_NAME, 0);
}
function isObjColumn(obj) {
    return !!obj && obj.getType() === "matrixdropdowncolumn";
}