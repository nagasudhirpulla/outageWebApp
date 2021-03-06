﻿$(document).ready(function () {
    // Setup - add a text input to each footer cell
    $('#outageResultsDiv tfoot th').each(function (i) {
        var title = $('#outageResultsDiv thead th').eq($(this).index()).text();
        $(this).html('<input placeholder="' + title + '" data-index="' + i + '" type="text" />');
    });

    // DataTable
    var table = $('#outageResultsDiv').DataTable({
        scrollY: "1000px",
        scrollX: true,
        dom: 'Bfrtip',
        scrollCollapse: true,
        fixedColumns: true,
        lengthMenu: [[10, 25, 50, 100, 500, 1000, -1], [10, 25, 50, 100, 500, 1000, "All"]],
        buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5'],
        pageLength: 50,
        "order": [[3, "desc"]]
    });

    // Filter event handler
    $(table.table().container()).on('keyup', 'tfoot input', function () {
        table
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });
});