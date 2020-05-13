$(() => {
    let allRows = $(".contributor-table tr ");
    $(".search-contributor").keyup(function () {
        let value = $(this).val().toLowerCase();
        allRows.show().filter(function () {
            let text = $(this).text().toLowerCase();
            return !~text.indexOf(value);
        }).hide();
    });

    $(".clear-search").on('click', () => {
        $(".search-contributor").val('');
        $(".contributor-table tr").show();
    });





});