function setupSurveyModel(survey, formName) {
    survey.commentSuffix = "Comment";
    survey.storeOthersAsComment = false;
    survey.completeText = "Submit";
    survey.showQuestionNumbers = false;
    survey.onComplete.add((_, options) => {
        options.showSaveInProgress();
        saveFormData(formName, survey, (result) => {
            if (result) {
                // Show a notification that form data is successfully saved and go to the index page in one second
                options.showSaveSuccess();
                setTimeout(() => {
                    window.location.href = "/";
                }, 1000);
            } else {
                options.showSaveError();
            }
        });
    });
}