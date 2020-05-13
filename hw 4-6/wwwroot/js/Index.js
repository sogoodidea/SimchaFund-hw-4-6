$(() => {


    $("#view-new-simcha-modal").on('click', () => {
        $("#new-simcha-modal").modal();
    });

    $("body").on('click', '.contributions', function () {
        console.log('i was clicked');
        const id = $(this).data('id');
        console.log(`${id}`);
    });


});