//Set default State and city

SetDefStateCity();

function SetDefStateCity() {
    $.ajax({
        type: "GET",
        traditional: true,
        url: "/CommonMaster/GetAllStateMaster",
        success: function (response) {
            $('#ddlState').val("");
            $('#ddlState').html("");
            $.each(response,
                function (index, value) {
                    $('#ddlState').append('<option value="' + value.Id + '">' + value.Name + '</option>');
                });
            let selectedState = $.grep(response, function (item) { return item.IsClientState == true });
            if (selectedState.length > 0) {
                $("#ddlState").val(selectedState[0].Id);
                
                $.ajax({
                    type: "GET",
                    traditional: true,
                    data: { stateId: selectedState[0].Id },
                    url: "/CommonMaster/GetAllCityByState",
                    success: function (Citydata) {
                        $('#ddlCity').val("");
                        $('#ddlCity').html("");
                        $.each(Citydata,
                            function (index, value) {
                                $('#ddlCity').append('<option value="' + value.Id + '">' + value.Name + '</option>');
                            });
                        let selectedcity = $.grep(Citydata, function (item) { return item.IsClientCity == true });
                        if (selectedcity.length > 0) {
                            $("#ddlCity").val(selectedcity[0].Id);
                           
                        }

                    },
                    error: function (a, b, c) {
                    }
                });
            }

        }
    });
}




$('#ddlState').on('change', function () {
    $('#ddlCity').val("");
    $('#ddlCity').html("");
    $.ajax({
        type: "GET",
        traditional: true,
        data: { stateId: $("#ddlState").val() },
        url: "/CommonMaster/GetAllCityByState",
        success: function (response) {
            $.each(response,
                function (index, value) {
                    $('#ddlCity').append('<option value="' + value.Id + '">' + value.Name + '</option>');
                });
        },
        error: function (a, b, c) {
        }
    });
});
function rowclickShowcity(stateId, cityId) {
    $.ajax({
        type: "GET",
        traditional: true,
        data: { stateId: $("#ddlState").val() },
        url: "/CommonMaster/GetAllCityByState",
        success: function (response) {
            $('#ddlCity').val(""); $('#ddlCity').html("");
            $.each(response,
                function (index, value) {
                    if (value.Id === cityId) {
                        $('#ddlCity').append('<option value="' + value.Id + '">' + value.Name + '</option>');
                    }
                });
        },
        error: function (a, b, c) {
        }
    });
}
$.ajax({
    type: "GET",
    traditional: true,
    url: "/PatientTypeMaster/GetAllActivePatientTypeMaster",
    success: function (response) {
        $.each(response,
            function (index, value) {
                $('#ddlPatientType').append('<option value="' + value.PatientTypeId + '">' + value.PatientType + '</option>');
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
$.ajax({
    type: "GET",
    traditional: true,
    url: "/CommonMaster/Gender",
    success: function (response) {
        $.each(response, function (index, value) {
            if(value.Id!==4)
            $('#ddlPatientgender').append('<option value="' + value.Id + '">' + value.Name + '</option>');
        });
    }
});

$.ajax({
    type: "GET",
    traditional: true,
    url: "/CommonMaster/GetActiveTypeOfAdmission",
    success: function (response) {
        $.each(response,
            function (index, value) {
                $('#ddlTypeOfAddmission').append('<option value="' + value.Id + '">' + value.Name + '</option>');
            });
    }
});
$.ajax({
    type: "GET",
    traditional: true,
    url: "/CommonMaster/GetActiveTypeofManagementNew",
    success: function (response) {
        $.each(response,
            function (index, value) {
                $('#ddlMangementType').append('<option value="' + value.Id + '">' + value.Name + '</option>');
            });
    }
});

$.ajax({
    type: "GET",
    url: "/CommonMaster/GetctiveRoomType",
    success: function (data) {
        $.each(data, function (index, value) {
            if (value.Id !== 1) {
                //Commnetd by Ankit Mane 
                //arrRoomType.push({ value: value.Id, text: value.Name });
            }
        });
    }
});

function fillOtDetailGrid() {
    $.ajax({
        type: "GET",
        url: "/RequestSubmission/GetServiceMasterByCategoryId",
        data: { categoryId: 5, hospitalType: $("#ddlHospitalType").val(), patientType: $("#ddlPatientType").val(), stateId: $("#ddlState").val(), cityId: $("#ddlCity").val(), gender: $("#ddlPatientgender").val() },
        success: function (data) {
            $.each(data,
                function (index, value) {
                    arrOTServiceType.push({ value: value.ServiceId, text: value.ServiceName });
                });
        }
    });
}
function fillAdmissionGrid() {
    $.ajax({
        type: "GET",
        url: "/RequestSubmission/GetServiceMasterByCategoryId",
        data: { categoryId: 6, hospitalType: $("#ddlHospitalType").val(), patientType: $("#ddlPatientType").val(), stateId: $("#ddlState").val(), cityId: $("#ddlCity").val(), gender: $("#ddlPatientgender").val() },
        success: function (data) {
            $.each(data,
                function (index, value) {
                    arrServiceType.push({ value: value.ServiceId, text: value.ServiceName });
                });
        }
    });
}



//var updcellName;
//var boxvalue;
//function openPopupToupdateGridDate(globalrowIndex, gridDetl, c,cellName) {
//    debugger;
//    updcellName = cellName;
//    gridname = gridDetl.$grid_center.context.id;
//    $("#popupmodelDatetimepicker").dialog({
//        height: 380,
//        width: 400,
//        modal: true,
//        open: function (evt, ui) {
//            $('#poPupDateTimepicker').datetimepicker({
//                format: 'MMM DD YYYY hh:mm A'
//        });
            
//        }
//    });
//    $(".ui-dialog-titlebar").hide();
//    $('#updateTheGridDate').on('click', function () {
//        debugger;
//         boxvalue = $('#poPupDateTimepicker').val();
//        var row = $("#" + gridname).pqGrid('getRowData', { rowIndx: globalrowIndex });
//        ClosePopupWindow("popupmodelDatetimepicker");
//        $("#" + gridname).pqGrid('updateRow', { rowIndx: globalrowIndex, newRow: { "StrDischargeDateTime": boxvalue } });
//        $("#"+gridname).pqGrid("refreshRow", { rowIndx: rowIndx });
//    });

//    $('#CancelTheGridDate').on('click', function () {
//        debugger;
//        ClosePopupWindow("popupmodelDatetimepicker");
//    });
   
//}

//$("#popupmodelDatetimepicker").dialog({
//    height: 450,
//    width: 700,
//    modal: true,
//    open: function (evt, ui) {
//        $('#poPupDateTimepicker').datetimepicker();
//        var abc = $('#poPupDateTimepicker').val();
//    }
//});