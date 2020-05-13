$(() => {


    $("body").on('click', '.view-deposit-modal', function () {
        $("#deposit-contributor-id").attr('value', `${$(this).data('id')}`);
        $("#depositer-name").text(`Deposit for ${$(this).data('firstname')} ${$(this).data('lastname')}`)
        $("#deposit-modal").modal();
    });

    $("#view-new-contributor-modal").on('click', () => {
        $("#form-contributor").attr('action', '/home/newcontributor');          //resets url to new
        $("#initial-deposit").show();                                           //resets to show the deposit amount textbox
        $("#contributor-header").text('New Contributor');                       //resets text of header
        $("#date-created-contributor").val('');                                 //resets date to empty
        $("#new-contributor-modal").modal();
    });

    let setDate = (date) => {
        let date_components = date.split("/");
        let month = ('0' + date_components[0]).slice(-2);
        let day = ('0' + date_components[1]).slice(-2);
        let year = date_components[2];
        let dateToInsert = `${year}-${month}-${day}`;
        $("#date-created-contributor").val(`${dateToInsert}`);
    }

    $("body").on('click', '.view-edit-contributor-modal', function () {
        $("#form-contributor").attr('action', '/home/updatecontributor');                              //changes url to update
        $("#initial-deposit").hide();                                                                  //hides deposit amount textbox
        $("#contributor-header").text(`${$(this).data('firstname')} ${$(this).data('lastname')}`);     //sets name in header

        let date = $(this).data('datecreated');                                  //gets value of datecreated
        setDate(date);                                                           //sets date input to be that date

        $("#first-name").val(`${$(this).data('firstname')}`);                    //sets first name textbox
        $("#last-name").val(`${$(this).data('lastname')}`);                      //sets last name textbox
        $("#phone-number").val(`${$(this).data('phonenumber')}`);                //sets phone number textbox

        let checking = true;
        if ($(this).data('alwaysinclude') === 'False') {                         //parses to bool value of alwaysinclude
            checking = false;
        }
        $("#always-include").prop('checked', checking);                          //sets checkbox according to the value

        $("#edit-contributor-id").attr('value', `${$(this).data('id')}`);        //sets id of contr being updates to be sent with inputs
        $("#new-contributor-modal").modal();
    });

    
   




});

