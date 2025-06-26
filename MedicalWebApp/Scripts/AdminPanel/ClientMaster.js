

var grid;
$(".datepicker").datepicker({
    buttonText: "Select date",
    dateFormat: "dd-MM-yy"
}).datepicker("setDate", new Date());
$("#anim").change(function () {
    $(".datepicker").datepicker("option", "showAnim", "fadeIn");
});

$(document).ready(function () {


    var Value = 'All Type';
    $('#ddlHospitalServiceCategory').val($.trim(Value));

    var dataSearchGrid = { location: "local" };
    var colSearchGrid = [
        { title: "", dataIndx: "ClientId", dataType: "integer", hidden: true },
        {
            title: "Client Code", dataIndx: "Code", width: 400,
            filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] }
        },
        {
            title: "Client Name", dataIndx: "Name", width: 400,
            filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] }
        }

    ];

    var setSearchGrid = {
        width: '100%',
        height: 250,
        sortable: false,
        numberCell: { show: true },
        hoverMode: 'cell',
        showTop: true,
        resizable: true,
        scrollModel: { autoFit: true },
        draggable: false,
        wrap: false,
        editable: false,
        filterModel: { on: true, mode: "AND", header: true },
        selectionModel: { type: 'row', subtype: 'incr', cbHeader: true, cbAll: true },
        colModel: colSearchGrid,
        dataModel: dataSearchGrid,
        pageModel: { type: "local", rPP: 20 },
        rowClick: function (evt, ui) {
            if (ui.rowData) {
                $("#logoImages").html("");
                var data = ui.rowData;
                $("#ClientId").val(data.ClientId);
                $("#ddlDeduction").val(data.DeductionModeId);
                $("#Name").val(data.Name); $("#Code").val(data.Code); $("#ExpiryDate").datepicker("setDate", data.strExpiryDate); $("#Deactive").prop('checked', data.Deactive); $("#IsShowLnk").prop('checked', data.IsShowLnk); $("#Society").val(data.Society); $("#Street").val(data.Street); $("#Landmark").val(data.Landmark);
                $("#ddlCity").val(data.CityId); $("#Pincode").val(data.Pincode); $("#ddlState").val(data.State); $("#ContactPerson").val(data.ContactPerson);
                $("#ContactDesignation").val(data.ContactDesignation); $("#Phone1").val(data.Phone1); $("#Phone2").val(data.Phone2); $("#CellPhone").val(data.CellPhone); $("#Fax").val(data.Fax);
                $("#Web").val(data.Web); $("#Email").val(data.Email); $("#GSTIN").val(data.GSTIN);
                $("#Deactive").attr("disabled", false); $("#IsShowLnk").attr("disabled", false);
                $("#ddlHospitalType").val(data.HospitalTypeId);
                $("#DeductionAmt").val(data.DeductionAmt);
                GetStateCityOnRowClick(data.CityId);
                $("#logoImages").html("");
                $("#ddlHospitalType").val(data.HospitalTypeId);
                $("#ddlHospitalServiceCategory").val(data.HospitalServiceCategoryId);
                $('#ddlClientType').val(data.ClientTypeId);
                DisplayUploadedImages("logoImages", data.LogoPath);

            }
        }
    }
    var $SearchGrid = $("#Searchgrid").pqGrid(setSearchGrid);

    loadSearchGrid();
    function loadSearchGrid() {
        $.ajax({
            type: 'GET',
            url: "/ClientMaster/AllClient",
            dataType: "json",
            beforeSend: function () {
                $SearchGrid.pqGrid("showLoading");
            },
            complete: function () {
                $SearchGrid.pqGrid("hideLoading");
            },
            success: function (response) {
                $("#Searchgrid").pqGrid("hideLoading");
                $("#Searchgrid").pqGrid("option", "dataModel.data", response);
                $("#Searchgrid").pqGrid("refreshDataAndView");
            }
        });
    }

    function returnUploadPath(file) {
        var filecontrol = $("#" + file);
        var filename = "";
        var files = filecontrol.get(0).files;
        if (files.length > 0) {
            filename = files[0].name;
        }
        return filename;
    }

    $("#btnSave").click(function () {
        if ($("#Code").val() == "") {
            ShowAlert("error", "code is missing! Please enter the code");
            return;
        }
        if ($("#Name").val() == "") {
            ShowAlert("error", "Name is missing! Please enter the Name");
            return;
        }


        if ($('#ddlDeduction').val() === "0") {
            ShowAlert('warning', 'Please Select Deduction Type');
            return;
        }

        if ($('#ddlClientType').val() === "") {
            ShowAlert('warning', 'Please Select Client Type');
            return;
        }


        var DeductionAmt = $('#DeductionAmt').val();
        if (($('#ddlDeduction').val() == 1 || $('#ddlDeduction').val() == 2) && DeductionAmt.length == 0 || DeductionAmt.length == null) {
            ShowAlert('warning', 'Please Provide Deduction Details!');
            return;
        }


        let deductionAmtVal = 0;
        if ($('#ddlDeduction').val() == 1 || $('#ddlDeduction').val() == 2) {
            deductionAmtVal = DeductionAmt;
        }




        SaveDeductionAmt();
        function SaveDeductionAmt() {
            debugger;
            $.ajax({
                type: "POST", //HTTP POST Method
                url: '/ClientMaster/SaveClient', // Controller/View
                data: { //Passing data
                    ClientId: $("#ClientId").val(), //Reading text box values using Jquery
                    Name: $("#Name").val(),
                    Code: $("#Code").val(),
                    ExpiryDate: $("#ExpiryDate").val(),
                    Street: $("#Street").val(),
                    Society: $("#Society").val(),
                    Landmark: $("#Landmark").val(),
                    City: $("#ddlCity").val(),
                    Pincode: $("#Pincode").val(),
                    State: $("#ddlState").val(),
                    HospitalTypeId: $("#ddlHospitalType").val(),
                    ContactPerson: $("#ContactPerson").val(),
                    ContactDesignation: $("#ContactDesignation").val(),
                    Fax: $("#Fax").val(),
                    Phone1: $("#Phone1").val(),
                    Phone2: $("#Phone2").val(),
                    CellPhone: $("#CellPhone").val(),
                    Web: $("#Web").val(),
                    Email: $("#Email").val(),
                    Deactive: $("#Deactive").prop('checked'),
                    GSTIN: $("#GSTIN").val(),
                    DeductionAmt: deductionAmtVal,
                    LogoPath: returnUploadPath("logoinput"),
                    DeductionModeId: $("#ddlDeduction").val(),
                    HospitalServiceCategoryId: $("#ddlHospitalServiceCategory").val(),
                    ClientTypeId: $('#ddlClientType').val(),
                    IsShowLnk: $("#IsShowLnk").prop('checked'),
                },
                success: function (msg) {
                    if (msg.success === true) {
                        SaveScandoc("logoinput", msg.clientId, "AdminPanel", "ClientMaster");
                        $("#logoinput").val('');
                        ClearForm();
                        ShowAlert("success", msg.message);
                        loadSearchGrid();
                    }
                    else {
                        ShowAlert("error", msg.message);
                        loadSearchGrid();
                    }
                }

            });
        }
    });
});
$('#ddlState')
    .change(function () {
        $('#ddlCity').val(""); $('#ddlCity').html("");
        GetStatesCity();
        GetHospitalServiceCategory();
    });
function GetStateCityOnRowClick(cityid) {
    $("#ddlCity").val(""); $("#ddlCity").html("");
    $.ajax({
        type: "GET",
        url: "/ClientMaster/GetCityById",
        datatype: "Json",
        data: { stateId: $('#ddlState').val() },
        success: function (data) {
            $.each(data, function (index, value) {
                $('#ddlCity').append('<option value="' + value.CityId + '">' + value.CityName + '</option>');
                $("#ddlCity").val(cityid);
            });
        }
    });
}
function GetStatesCity() {
    $.ajax({
        type: "GET",
        url: "/ClientMaster/GetCityById",
        datatype: "Json",
        data: { stateId: $('#ddlState').val() },
        success: function (data) {
            $.each(data, function (index, value) {
                $('#ddlCity').append('<option value="' + value.CityId + '">' + value.CityName + '</option>');
            });
        }
    });
}


//function GetHospitalServiceCategory() {
//    debugger;
//    $.ajax({
//        type: "GET",
//        url: "/ClientMaster/GetHospitalServiceCategory",
//        datatype: "Json",
//        data: { stateId: $('#ddlState').val() },
//        success: function (data) {
//            $.each(data, function (index, value) {
//                $('#ddlCity').append('<option value="' + value.CityId + '">' + value.CityName + '</option>');
//            });
//        }
//    });
//}

$.ajax({
    type: "GET",
    url: "/ClientMaster/AllStates",
    datatype: "Json",
    success: function (data) {
        $.each(data, function (index, value) {
            $('#ddlState').append('<option value="' + value.StateId + '">' + value.StateName + '</option>');
        });
    }
});
$.ajax({
    type: "GET",
    url: "/ClientMaster/GetHospitalServiceCategory",
    datatype: "Json",
    success: function (data) {
        $.each(data, function (index, value) {
            $('#ddlHospitalServiceCategory').append('<option value="' + value.HospitalServiceCategoryId + '">' + value.HospitalServiceCategory + '</option>');
        });
    }
});

$.ajax({
    type: "GET",
    url: "/ClientMaster/GetClientType",
    datatype: "Json",
    success: function (data) {
        $.each(data, function (index, value) {
            $('#ddlClientType').append('<option value="' + value.ClientTypeId + '">' + value.TypeName + '</option>');
        });
    }
});


$.ajax({
    type: "GET",
    traditional: true,
    url: "/CommonMaster/HospitalTypeDropDown",
    success: function (response) {

        $.each(response, function (index, value) {
            $('#ddlHospitalType').append('<option value="' + value.Value + '">' + value.Key + '</option>');
        });
    }
});

function ClearForm() {
    $("#ClientId").val(""); $("#Name").val(""); $("#Code").val(""); $("#ExpiryDate").datepicker("setDate", new Date()); $("#Deactive").prop('checked', false); $("#IsShowLnk").prop('checked', false); $("#Society").val(""); $("#Street").val(""); $("#Landmark").val(""); $("#ddlCity").val(""); $("#Pincode").val("");
    $("#ddlState").val(""); $("#ContactPerson").val(""); $("#ContactDesignation").val(""); $("#Phone1").val(""); $("#Phone2").val(""); $("#CellPhone").val(""); $("#Fax").val(""); $("#Web").val(""); $("#Email").val("");
    $("#VATTINNo").val(""); $("#ServiceTaxNo").val(""); $("#ExciseCode").val(""); $("#PANNo").val(""); $("#CST").val(""); $("#IncomeTaxNo").val(""); $("#RTGSCODE").val(""); $("#IFSCCODE").val(""); $("#CreditPeriod").val("");
    $("#EligableForAdv").prop('checked', false); $("#ExportCode").val(""); $("#BankName").val(""); $("#BankAcNo").val(""); $("#MICRNo").val(""); $("#BankBranch").val(""); $("#Note").val("");
    $("#ddlCity").html(""); $("#ddlState").val(""); $("#ddlHospitalType").val("");
    $("#Deactive").attr("disabled", true); $("#DateOfAssociation").datepicker().datepicker("setDate", new Date()); $("#GSTIN").val("");
    $("#DeductionAmt").val(""); $("#ddlDeduction").val(""); $("#ddlHospitalServiceCategory").val(""); $('#ddlClientType').val("");
}

$(document).ready(function () {
    $("#btnAdd").click(function () {
        ClearForm();
    });
});


