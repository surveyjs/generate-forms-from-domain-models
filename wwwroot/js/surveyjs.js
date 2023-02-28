function setupSurveyModel(survey, formName) {
    survey.commentSuffix = "Comment";
    survey.storeOthersAsComment = false;
    survey.completeText = "Submit";
    survey.showQuestionNumbers = false;
    survey.onComplete.add((sender, options) => {
        saveFormData(formName, survey);
        //It is better to do it on successful saving data callback from the server
        setTimeout(() => {
            //Go to index page
            window.location.href = "/";
        }, 1000);
    });
}