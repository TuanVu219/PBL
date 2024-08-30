let ascending = true; // Variable to track the current sort order

document.getElementById('sortButton').addEventListener('click', function () {
    let table = document.getElementById('patientTable').getElementsByTagName('tbody')[0];
    let rows = Array.from(table.getElementsByTagName('tr'));

    rows.sort(function (a, b) {
        let dateA = new Date(a.cells[1].innerText.split('/').reverse().join('-'));
        let dateB = new Date(b.cells[1].innerText.split('/').reverse().join('-'));
        return ascending ? dateA - dateB : dateB - dateA;
    });

    rows.forEach(function (row) {
        table.appendChild(row);
    });

    ascending = !ascending; // Toggle the sorting order
});
