function setupSurveyModel(survey, formName) {
    survey.commentSuffix = "Comment";
    survey.storeOthersAsComment = false;
    survey.completeText = "Submit";
    survey.showQuestionNumbers = false;
    survey.navigateToUrl = "/";
    survey.onComplete.add((sender, options) => {
        saveFormData(formName, survey);
    });
}