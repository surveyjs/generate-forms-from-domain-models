function setupSurveyModel(survey, formName) {
    survey.commentSuffix = "Comment";
    survey.storeOthersAsComment = false;
    survey.completeText = "Submit";
    survey.showQuestionNumbers = false;
    survey.onComplete.add((sender, options) => {
        options.showDataSaving();
        saveFormData(formName, survey, (result) => {
            if (result) {
                //Show the successful saving and return to the index page in a second
                options.showDataSavingSuccess();
                setTimeout(() => {
                    //Go to index page
                    window.location.href = "/";
                }, 1000);
            } else {
                options.showDataSavingError();
            }
        });
    });
}