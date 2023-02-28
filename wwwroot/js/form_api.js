const apiUrl = "/api/form/";
function loadForm(name, creator) {
    const url = apiUrl + "loadform?name=" + name;
    var xhttp = new XMLHttpRequest();
    xhttp.open("GET", url, true);
    xhttp.send();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            const json = JSON.parse(this.responseText);
            creator.JSON = json;
            creator.readOnly = false;
        }
    };
}
function saveForm(name, jsonAsText) {
    const url = apiUrl + "saveform";
    var xhttp = new XMLHttpRequest();
    xhttp.open("POST", url);
    xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    xhttp.send(JSON.stringify({ "name": name, "data": jsonAsText }));
}

function loadFormAndData(name, id, onLoad) {
    const url = apiUrl + "loadform_and_data?name=" + name + "&id=" + id;
    const xhttp = new XMLHttpRequest();
    xhttp.open("GET", url, true);
    xhttp.send();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            const json = JSON.parse(this.responseText);
            const formData = JSON.parse(json.data);
            onLoad(json.form, formData);
        }
    };
}
function saveFormData(name, survey, oncallback) {
    const url = apiUrl + "savedata";
    const xhttp = new XMLHttpRequest();
    xhttp.open("POST", url);
    xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    const data = {
        "name": name,
        "id": survey.getValue("Id"),
        "data": JSON.stringify(survey.data)
    };
    xhttp.send(JSON.stringify(data));
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            oncallback(true);
        } else {
            oncallback(false);
        }
    };
}