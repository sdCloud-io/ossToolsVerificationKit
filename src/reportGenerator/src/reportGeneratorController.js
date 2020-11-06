function appendtoTable(state,type,index) {
    const $table = $("#" + type + index);
    $("#" + type + index + " tbody").empty();
    if (Object.keys(state).length > 0) {
        $table.append(
            "<tr><th>Key</th><th>Value</th></tr>"
        )
        Object.keys(state)
            .forEach((key) => {
                var field = state[key];
                $table.append(
                    "<tr>" +
                    "<td>" +
                    key +
                    "</td>" +
                    "<td>" +
                    field +
                    "</td>" +
                    "</tr>");
            });
    } else {
        $table.append(
            "<tr><td>No last state defined</td></tr>"
        );
    }
}

$(document).ready(function() { 
var jqxhr = $.getJSON("data/testReport.json", function() {

	})

.complete(function(data) { 
 	getData(data);
});

}); //end of document.ready

function compareStates(sdeState, pysdState) {
    var comarableKeys = [];
    var isStateEqual = true;
    var pysdKeys = Object.keys(pysdState);
    var sdeKeys = Object.keys(sdeState);
    if (sdeKeys.length > 0 && pysdKeys.length > 0) {
        for (var i = 0; i < pysdKeys.length; i++) {
            for (var j = 0; j < sdeKeys.length; j++) {
                if (pysdKeys[i] === sdeKeys[j]) {
                    if (Math.abs(pysdState[pysdKeys[i]] - sdeState[sdeKeys[j]]) > 0.001)
                        isStateEqual = false;
                }
            }
        }
    } else {
        isStateEqual = false;
    }
    return isStateEqual;
}

function sortObject(o) {
    var sorted = {},
        key, a = [];

    for (key in o) {
        if (o.hasOwnProperty(key)) {
            a.push(key);
        }
    }

    a.sort();

    for (key = 0; key < a.length; key++) {
        sorted[a[key]] = o[a[key]];
    }
    return sorted;
}

function getData(certData) {
	let myData = JSON.parse(certData.responseText);
    var template = $('#runResult').html();
    var xmileTemplate = $('#runXmileResult').html();
    Mustache.parse(template);   // optional, speeds up future uses
    Mustache.parse(xmileTemplate);   // optional, speeds up future uses
    tabcontent = document.getElementsByClassName("tabcontent");
    tabcontent[1].style.display = "none";
    tabcontent[0].class += " active";
            var xmileExecutions = [];
            var vensimExecutions = [];
                for (let i = 0; i < myData.sdeResults.length; i++) {

                var sdeState = sortObject(myData.sdeResults[i].lastModelState);

                var pysdState = sortObject(myData.pysdResults[i].lastModelState);
                       
                    var isStateEqual = compareStates(sdeState, pysdState);

                        vensimExecutions.push({
                            index: i, isStateEqual: isStateEqual, sdeElmnt: myData.sdeResults[i], sdeState: JSON.stringify(sdeState, null, 4),
                            pysdState: JSON.stringify(pysdState, null, 4), pysdElemnt: myData.pysdResults[i], modelName: myData.sdeResults[i].modelPath.replace(/^.*[\\\/]/, '')
                        });
                }
            var length = myData.pysdXmileResults.length;
            for (let i = 0; i < myData.pysdXmileResults.length; i++) {
                xmileExecutions.push({ index: 'xmile' + i, pysdElemnt: myData.pysdXmileResults[i], modelName: myData.pysdXmileResults[i].modelPath.replace(/^.*[\\\/]/, '') });
            }

            var rendered = Mustache.render(template, {
                sdeResults: vensimExecutions
            });

            var renderedXmile = Mustache.render(xmileTemplate, {
                xmileResults: xmileExecutions
            });

            $('#runTarget').html(rendered);
            $('#runXmileTarget').html(renderedXmile);

}
function openTab(evt, tabName) {
    // Declare all variables
    var i, tabcontent, tablinks;
    // Get all elements with class="tabcontent" and hide them
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    // Show the current tab, and add an "active" class to the button that opened the tab
    document.getElementById(tabName).style.display = "block";
    document.getElementById(tabName).class += " active";    
}