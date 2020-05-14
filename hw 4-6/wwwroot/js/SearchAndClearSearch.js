$(() => {


    let allRows = $(".contributor-table tr ");
    $(".search-contributor").keyup(function () {
        let value = $(this).val().toLowerCase();
        allRows.show().filter(function () {
            let text = $(this).find('td:eq(1)').text().toLowerCase();
            return !~text.indexOf(value);
        }).hide();
    });

    $(".clear-search").on('click', () => {
        $(".search-contributor").val('');
        $(".contributor-table tr").show();
    });





});