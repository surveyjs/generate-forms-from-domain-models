function isNameCorrect(name) {
    return /^@?[a-zA-Z_]\w*(\.@?[a-zA-Z_]\w*)*$/.test(name);
}

function isListQuestionType(el) {
    return ["paneldynamic", "matrixdynamic", "matrixdropdown"].indexOf(el.getType()) > -1;
}

function getNestedItemClassName(name) {
    return name + "Item";
}

function getElementTypeName(el) {
    if (isListQuestionType(el)) return "IList<" + getNestedItemClassName(el.name) + ">";
    if (el.getType() === "text") {
        if (el.inputType === "number") return "int";
        if (el.inputType === "date" || el.inputType === "localedate") return "DateTime";
    }
    if (el.getType() === "boolean") return "bool"
    return "string";
}
function getCodePadding() { return "    "; }

function generateClassByElements(className, elements, lines) {
    lines.push("public class " + className + " {");
    elements.forEach(el => {
        if (el.isQuestion || el.getType() === "matrixdropdowncolumn") {
            lines.push(getCodePadding() + getElementTypeName(el) + " " + el.name + " { get; set; }")
        }
    });
    lines.push("}")
}

function generateDomainModelsCode(survey) {
    const lines = [];
    generateClassByElements(survey.name, survey.getAllQuestions(), lines);
    survey.getAllQuestions().forEach(q => {
        if (!isListQuestionType(q)) return;
        if (q.getType() === "paneldynamic") {
            lines.push("");
            generateClassByElements(getNestedItemClassName(q.name), q.elements, lines);
        } else {
            lines.push("");
            generateClassByElements(getNestedItemClassName(q.name), q.columns, lines);
        }
    });
    return lines.join("\n");
}