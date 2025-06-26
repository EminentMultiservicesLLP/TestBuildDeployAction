$(document).ready(function () {
    //GetUserDetails();
    $.ajax({
        type: "GET",
        url: "/ClientMaster/AllActiveClient",
        success: function (data) {
            $.each(data,
                function (index, value) {
                    $('#ddlClient').append('<option value="' + value.ClientId + '">' + value.Name + '</option>');
                });
        }
    });

    var tempPwd;
    $("#UserCode").prop('disabled', true);
    $("#LoginName").prop('disabled', true);
    //$("#UserName").prop('disabled', true);
    $("#Password").prop('disabled', true);
    $("#PasswordValidate").prop('disabled', true);



    /****************Auto complete fn*******************/

    $("#UserName").autocomplete({
        source: function (request, response) {
            $("#UserId").val("");
            if ($("#UserName").val().trim().length > 2) {
                $.ajax({
                    url: "/ClientMaster/AllClient",
                    data: JSON.stringify({ prefix: request.term }),
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data,
                        //response($.each(data,
                            function (item) {

                                // $("#txtUserId").val(item.EmpId);
                                return { label: item.ClientName, value: item.Name, clientId: item.ClientId, clientCode: item.Code }
                            }));
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            }
        },
        select: function (e, i) {
            $("#UserId").val(i.item.clientId);
            $("#UserCode").val(i.item.clientCode);
            $("#UserName").val(i.item.label);

        },
        minLength: 1
    });

    /****************Block Speacial Characters fn*******************/
    $("#UserName").keypress(function (e) {
        debugger;
        var regex = new RegExp("^[a-zA-Z \b]+$");//(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z]{8,}$/)
        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }

        e.preventDefault();
        return false;
    });

    /****************GetUserCode Fn*******************/
    $("#btnAdd").click(function () {
        debugger;
        getCode();
        Clearform();
        $("#UserName").prop('disabled', false);
        $("#EmailID").prop('disabled', false);
    });
    function getCode() {
        debugger;
        $('#Code').empty().html("");

        $.ajax({
            type: "GET",
            url: "/UserCreation/GetUserCode",
            datatype: "Json",
            async: true,
            success: function (data) {
                $("#UserCode").val(data[0].UserCode);
            }
        });
    }


    /****************RowIndex Fn*******************/

    function getRowIndx() {
        var arr = $("#Usergrid").pqGrid('selection', { type: 'row', method: 'getSelection' });
        if (arr && arr.length > 0) {
            return arr[0].rowIndx;
        }
        else {
            alert("Select a row.");
            return null;
        }
    }
    /****************Get User Details and Grid Details Fn*******************/
    function getUserDetails() {
        $.ajax({
            type: "GET",
            url: "/UserCreation/GetUserDetails",
            datatype: "Json",
            async: true,
            success: function (data) {
                $("#Usergrid").pqGrid("option", "dataModel.data", data);
                $("#Usergrid").pqGrid("refreshDataAndView");
            }
        });
    }

    var dataList = { location: 'local', sorting: 'local', paging: 'local', dataType: 'JSON' };
    var setUserCol = [
        { title: "User Code", dataIndx: "UserCode", width: 90 },
        { title: "UserID", dataIndx: "UserID", width: 90, hidden: true },
        {
            title: "User Name", dataIndx: "UserName", width: 600,
            filter: { type: 'textbox', condition: 'begin', listeners: ['keyup'] }
        },
        //{ title: "Active/Inactive", dataIndx: "IsDeactive", width: 90 },
        { title: "Login Name", dataIndx: "LoginName", width: 90, hidden: true },
        { title: "Password", dataIndx: "Password", width: 90, hidden: true }



    ];
    var setUserList = {
        width: "auto", //auto width
        height: 400, //height in %age
        autoSizeInterval: 0,
        dragColumns: { enabled: false },
        hoverMode: 'cell',
        editable: false,
        filterModel: { on: true, mode: "AND", header: true },
        showTop: true,
        resizable: true,
        virtualX: true,
        colModel: setUserCol,
        selectionModel: { type: 'row', subtype: 'incr', cbHeader: true, cbAll: true },
        dataModel: dataList,
        pageModel: { type: "local", rPP: 20 },
        rowClick: function (evt, ui) {
            if (ui.rowData) {
                debugger;
                var details = ui.rowData;
                $("#UserCode").val(details.UserCode);
                $("#UserName").val(details.UserName);
                $("#EmailID").val(details.EmailID);
                $("#UserID").val(details.UserID);
                $("#LoginName").val(details.LoginName);
                $("#Password").val(details.Password);
                $("#ddlClient").val(details.ClientId);
                $("#IsDeactive").prop('checked', details.IsDeactive);
                $("#UserName").prop('disabled', false);
                $("#EmailID").prop('disabled', false);
                $("#LoginName").prop('disabled', false);
                $("#Password").prop('disabled', false);
                $("#PasswordValidate").prop('disabled', false);
                //tempPwd = details.Password;
            }
        }
    };
    $("#Usergrid").pqGrid(setUserList);
    getUserDetails();

    /****************User Deactivate Fn*******************/

    $('#btnDelete').on('click', function () {
        if ($("#UserName").val() === "") {
            ShowAlert("error", " Please Select the User to Deactivate");
            return;
        }
        var rowIndx = getRowIndx();
        if (rowIndx == null) {
            return;
        }
        $("#Usergrid").pqGrid("deleteRow", { rowIndx: rowIndx });
        var details = JSON.stringify({
            UserCode: $("#UserCode").val(),
            UserName: $("#UserName").val(),
            UserID: $("#UserID").val(),
            EmailID: $("#EmailID").val(),


        });
        $.ajax(
        {
            type: "POST", //HTTP POST Method
            traditional: true,
            contentType: 'application/json; charset=utf-8',
            url: '/UserCreation/DeleteUser', // Controller/View
            data: details,
            success: function (msg) {
                debugger;
                if (msg.success) {
                    getUserDetails();
                    Clearform();
                    ShowAlert("success", "Request Updated Successfully");
                }
                else {
                    ShowAlert("error", msg.Message);
                }
            },
            error: function () {
            }
        });
    });
    /****************Add New user Fn*******************/

    $("#btnAddUser").click(function () {
        if ($("#UserName").val() === "") {
            ShowAlert("error", "UserName is missing! Please enter the UserName");
            return;
        }
        if ($("#UserCode").val() === "") {
            ShowAlert("error", "UserCode is missing! Please Click Add User to Enter UserCode");
            return;
        }
        if ($("#ddlClient").val() === "") {
            ShowAlert("error", "Client is missing!");
            return;
        }
        if ($("#EmailID").val() === "") {
            ShowAlert("error", "Email Id is missing! Please enter the Email Id");
            return;
        }
        var details = JSON.stringify({
            UserCode: $("#UserCode").val(),
            UserName: $("#UserName").val(),
            UserID: $("#UserID").val(),
            LoginName: $("#LoginName").val(),
            Password: $("#Password").val(),
            ClientId: $("#ddlClient").val(),
            EmailID: $("#EmailID").val(),
            IsDeactive: $("#IsDeactive").prop('checked')

        });
        debugger;
        $.ajax(
        {
            type: "POST",
            traditional: true,
            contentType: 'application/json; charset=utf-8',
            url: '/UserCreation/SaveUser',
            data: details,
            success: function (msg) {
                debugger;
                if (msg.success) {
                    getUserDetails();
                    Clearform();
                    ShowAlert("success", "Request Updated Successfully");
                }
                else {
                    ShowAlert("warning", msg.Message);
                }
            },
            error: function () {
            }
        });

    });

    /***********Add Login Details Fn********************/

    function addLoginDetails() {
        var details = JSON.stringify({
            LoginName: $("#LoginName").val(),
            Password: $("#Password").val(),
            OldPassword: $("#Password1").val(),
            UserCode: $("#UserCode").val(),
            UserName: $("#UserName").val(),
            EmailID: $("#EmailID").val(),
            UserID: $("#UserID").val()
        });
        $.ajax(
        {
            type: "POST", //HTTP POST Method
            traditional: true,
            contentType: 'application/json; charset=utf-8',
            url: '/UserCreation/SaveUserLogin', // Controller/View
            data: details,
            success: function (msg) {
                if (msg.success) {
                    ShowAlert("success", msg.Message);
                    ClosePopupWindow("ConfirtmationPopUp");
                    getUserDetails();
                    Clearform();
                }
                else {
                    ShowAlert("warning", msg.Message);
                }
            },
            error: function () {
            }
        });
    }
    $("#btnAddUserDetails").click(function () {
        var re;
        $("#Password1").val("");
        if ($("#LoginName").val() === "") {
            ShowAlert("error", "LoginName is missing! Please enter the LoginName");
            return;
        }
        if ($("#Password").val() === "") {
            ShowAlert("error", "Password is missing! Please enter the Password");
            return;
        }

        if ($("#Password").val() !== "" && $("#Password").val() === $("#PasswordValidate").val()) {
            if ($("#Password").val().length < 8 || $("#Password").val().length > 16) {
                ShowAlert("error", " Password Length Should Be Between 8-16 Characters!");
                return;
            }
        }
        re = /[0-9]/;
        if (!re.test($("#Password").val())) {
            ShowAlert("error", " Password must contain atleast 1 number!");
            return;
        }
        re = /[a-z]/;
        if (!re.test($("#Password").val())) {
            ShowAlert("error", " Password must contain atleast 1 lower case letter!");
            return;
        }
        re = /[A-Z]/;
        if (!re.test($("#Password").val())) {
            ShowAlert("error", " Password must contain atleast 1 upper case letter!");
            return;
        }

        re = /[#^&*%\$]/;
        if (!re.test($("#Password").val())) {
            ShowAlert("error", " Password must contain atleast 1 Special Character!");
            return;
        }


        if ($("#Password").val() !== $("#PasswordValidate").val()) {
            ShowAlert("error", "Passwords Dont Match! Plese Check your Password");
            return;
        }
        if ($("#UserName").val() === "") {
            ShowAlert("error", "UserName is missing! Please enter the UserName");
            return;
        }
        //if (tempPwd == null) {
        //    addLoginDetails();
        //}
        //else {
        //    $("#ConfirtmationPopUp").dialog({
        //        height: 200,
        //        width: 300,
        //        modal: true,
        //        open: function () {

        //        },

        //        close: function () {
        //        },
        //        show: {
        //            effect: "blind",
        //            duration: 500
        //        }
        //    });
        //}
        $("#ConfirtmationPopUp").dialog({
            height: 200,
            width: 300,
            modal: true,
            open: function () {

            },

            close: function () {
            },
            show: {
                effect: "blind",
                duration: 500
            }
        });
        $("#btnConfirm").click(function () {
            addLoginDetails();
        });

    });
    $("#btnClose").click(function () {
        ClosePopupWindow("ConfirtmationPopUp");
    });
});

/****************Reset Login Details Fn*******************/
$("#btnReset").on("click", Clearform1);
function Clearform1() {
    $("#LoginName").val("");
    $("#Password").val("");
    $("#PasswordValidate").val("");
    $("#UserName").val("");
    $("#UserCode").val("");
    $("#UserID").val("");
    $("#ddlClient").val("");
    $("#EmailID").val("");
    $("#IsDeactive").prop('checked', false);
}
/****************Reset Complete Details Fn*******************/
$("#btnReset1").on("click", Clearform);
function Clearform() {
    $("#LoginName").val("");
    $("#Password").val("");
    $("#PasswordValidate").val("");
    $("#UserName").val("");
    $("#EmailID").val("");
    $("#UserCode").val("");
    $("#UserID").val("");
    $("#ddlClient").val("");
    $("#IsDeactive").prop('checked', false);

}
