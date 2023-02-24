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
    if (!isAdmin) {
        //Setup Creator for a content manager
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
function isObjColumn(obj) {
    return !!obj && obj.getType() === "matrixdropdowncolumn";
}