// Send a hit when text is copied or pasted
function selectAndCopy() {
    document.addEventListener("copy", () => {
        gtag("event", "copy_text", {
            "value": document.getSelection().toString()
        });
    });
    document.addEventListener("paste", (e) => {
        gtag("event", "paste_text", {
            "value": e.clipboardData.getData("text")
        });
    });
}

// For checkboxes: Send a hit when a checkbox becomes unchecked
// For radios and text inputs: Send a hit when the value of the input is changed (excluding when no value exists or none was selected)
function changeInputValue() {
    const inputs = document.getElementsByTagName("input");
    const inputAnswers = new Map();

    for (const input of inputs) {
        input.addEventListener("change", () => {
            let name = input.name;
            let value = input.value;
            let type = input.type;
            
            if (type === "checkbox") {
                if (!input.checked) {
                    gtag("event", `change_input_${type}`, {
                        "name": name,
                        "value": value,
                    });
                }
            } else if (type === "radio" || type === "text") {
                if (inputAnswers.has(name)) {
                    gtag("event", `change_input_${type}`, {
                        "name": name,
                        "value": value,
                        "previous_value": inputAnswers.get(name)
                    });
                }
                inputAnswers.set(name, value);
            }
        });
    }
}

// Send a hit when the custom back link is clicked
function backLink() {
    const backButton = document.getElementById("back-link");
    if (backButton !== null) {
        backButton.addEventListener("click", () => {
            gtag("event", "back_button_pressed", {
                "value": backButton.innerText
            })
        });
    }
}

// Send a hit when a drop down list is expanded
function expandDropDown() {
    const details = document.getElementsByTagName("details");
    for (const detail of details) {
        detail.addEventListener("toggle", () => {
            if (detail.open) {
                // Send a hit when the user expands the drop-down
                const summaries = detail.getElementsByTagName("summary");
                if (summaries.length > 0) {
                    // We don't care about other summary elements other than the very first child of the details tag
                    const summary = summaries[0];
                    gtag("event", "expand_drop_down", {
                        "value": summary.innerText
                    });
                }
            }
        });
    }
}

// Send a hit when a link is clicked and redirects to an external site
function clickExternalLink() {
    const anchors = document.getElementsByTagName("a");
    for (const anchor of anchors) {
        if (!anchor.href.includes(document.location.hostname)) {
            anchor.addEventListener("click", () => {
                gtag("event", "visit_external_site", {
                    "name": anchor.innerText,
                    "value": anchor.href
                });
            });
        }
    }
}

// Send a hit if the page contains error messages
// The error summary component includes an <ul> element with the 'govuk-error-summary__list' class
function errorMessageDisplayed() {
    const unorderedLists = document.getElementsByTagName("ul");
    for (const unorderedList of unorderedLists) {
        if (unorderedList.className.includes("govuk-error-summary__list")) {
            const inputs = document.getElementsByTagName("input");
            let value = "";
            for (const input of inputs) {
                if (input.type === "hidden") {
                    continue;
                }
                value = value.concat(`${input.name}=${input.value};`);
            }
            gtag("event", "error_displayed", {
                "value": value
            });
            
            // No need to iterate further
            return;
        }
    }
}

// Send a hit when the user skips a question using the 'skip this question' link
function skipQuestion() {
    const anchors = document.getElementsByTagName("a");
    for (const anchor of anchors) {
        if (anchor.innerText.toLowerCase().includes("skip this question")) {
            anchor.addEventListener("click", () => {
                gtag("event", "skip_question");
            });
        }
    }
}

function setUpGoogleAnalytics() {
    selectAndCopy();
    changeInputValue();
    backLink();
    expandDropDown();
    clickExternalLink();
    errorMessageDisplayed();
    skipQuestion();
}