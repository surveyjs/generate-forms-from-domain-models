const apiUrl = "/api/form/";

// Load a form JSON schema with a specified name and assign it to Survey Creator for editing.
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

// Save a form JSON schema in a database under a specified name.
function saveForm(name, jsonAsText) {
    const url = apiUrl + "saveform";
    var xhttp = new XMLHttpRequest();
    xhttp.open("POST", url);
    xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    xhttp.send(JSON.stringify({ "name": name, "data": jsonAsText }));
}

// Load a form JSON schema with a specified name and form data (a single survey result) with a specified ID.
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

// Save form data (a single survey result) in a database.
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