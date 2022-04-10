let filterInput = document.querySelector("#FilterInput");
filterInput.addEventListener("keyup", FilterFilms);
let exportBtn = document.querySelector("#export-btn");
exportBtn.addEventListener("click", ExportFile);

function ExportFile() {
    let input = document.getElementById("FilterInput");
    if (input.validationMessage!="") {
        let failMessage = document.querySelector("#failSearch");
        failMessage.hidden = false;
    } 
}

function FilterFilms() {
    // Declare variables
    var input, filter, table, trs, td, i, txtValue;
    input = document.getElementById("FilterInput");
    filter = input.value.toUpperCase();
    table = document.getElementById("FilterTable");

    trs = table.getElementsByTagName("tr");
    trs = Array.from(trs);
    trs = trs.slice(1);

    let trHiddenCnt=0;
    // Loop through all table rows, and hide those who don't match the search query
    for (i = 0; i < trs.length; i++) {
        td = trs[i].getElementsByTagName("td")[0];
        if (td) {
            txtValue = td.textContent || td.innerText;
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                trs[i].hidden = false;
            } else {
                trs[i].hidden = true;
                trHiddenCnt += 1;
            }
        }
    }
    if (trHiddenCnt === trs.length) {
        input.setCustomValidity("Enter another criteria");
        
    } else {
        input.setCustomValidity("");
    }
}
