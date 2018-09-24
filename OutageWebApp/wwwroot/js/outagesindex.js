$(document).ready(function () {
    // Setup - add a text input to each footer cell
    $('#outageResultsDiv tfoot th').each(function (i) {
        var title = $('#outageResultsDiv thead th').eq($(this).index()).text();
        $(this).html('<input placeholder="Search ' + title + '" data-index="' + i + '" type="text" />');
    });

    // DataTable
    var table = $('#outageResultsDiv').DataTable({
        scrollY: "1000px",
        scrollX: true,
        scrollCollapse: true,
        fixedColumns: true,        
        lengthMenu: [[10, 25, 50, 100, 500, 1000, -1], [10, 25, 50, 100, 500, 1000, "All"]],
        buttons: [
            'copy', 'csv', 'excel', 'pdf', 'print'
        ],
        pageLength: 50
    });

    // Filter event handler
    $(table.table().container()).on('keyup', 'tfoot input', function () {
        table
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });
});