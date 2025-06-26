var selectedControls;

function GetRequestPharmacyDetailById(requestId) {
    $.ajax({
        type: "GET",
        traditional: true,
        url: "/RequestSubmission/GetRequestPharmacyDetail",
        data: { requestId: requestId },
        success: function (response) {
            $("#PharmacyServicePoPupModalGrid").pqGrid("option", "dataModel.data", response);
            $("#PharmacyServicePoPupModalGrid").pqGrid("refreshDataAndView");
        },
        error: function (a, b, response) {
            ShowAlert("error", "Please try again or contact to your superior");
        },

    });
}


/********* Add Item to  Pharmacy detail grid*********/
$('#btnAddRowPharmacydetail').on('click', function () {
    var dataMGrid = [];
    var getData = $("#PharmacyServicePoPupModalGrid").pqGrid("option", "dataModel.data");
    $.each(getData, function (key, value) {
        var grdData = {
            'RequestId': value.RequestId, 'PharmacyDtlId': value.PharmacyDtlId, 'ConsumeDate': value.ConsumeDate, 'BillRate': value.BillRate
        }
        dataMGrid.push(grdData);
    });
    var grdnewData = {
        'RequestId': 0, 'PharmacyDtlId': 0, 'ConsumeDate': "", 'BillRate': 0
    }
    dataMGrid.push(grdnewData);
    $("#PharmacyServicePoPupModalGrid").pqGrid("option", "dataModel.data", dataMGrid);
    $("#PharmacyServicePoPupModalGrid").pqGrid("refreshDataAndView");
});


$('#btnRemoveRowPharmacydetail').on('click', function () {
    var getData = $("#PharmacyServicePoPupModalGrid").pqGrid("option", "dataModel.data");
    var rowIndx = getRowIndxOfPharmacyGrid();
    if (rowIndx == null) {
        return;
    }
    $("#PharmacyServicePoPupModalGrid").pqGrid("deleteRow", { rowIndx: rowIndx });
});

function getRowIndxOfPharmacyGrid() {
    
    //var arr = $("#PharmacyServicePoPupModalGrid").pqGrid('selection', { type: 'row', method: 'getSelection' });
    var ss = $("#PharmacyServicePoPupModalGrid").pqGrid("selection", { type: 'row', method: 'getSelection' });
    alert("arr:" + ss); alert(ss.length); alert(ss[0].rowIndx)
    if (ss && ss.length > 0) {
        return ss[0].rowIndx;
    }
    else {
        ShowAlert("info", "No row Selected.");
        return null;
    }
}



function LoadDCDetailAttachmentPoPupModal() {
    $("#popupModelRight_DCDetails").dialog({
        height: 400,
        width: 650,
        modal: true,
        open: function (evt, ui) {

        }
    });
}
function LoadviewDCDetailAttachmentPoPupModal() {
    $("#displayModelRight_DCDetails").dialog({
        height: 400,
        width: 650,
        modal: true,
        open: function (evt, ui) {

        }
    });
}


$('#ddlMangementType').on('change', function () {
    $("#btnOTDetailleft").prop("disabled", false);
    if ($('#ddlMangementType').val() === "2") {
        if (!showAlertOnBlank($("#ddlHospitalType"), "Hospital Type is missing! Please Select Hospital type")) return;
        if (!showAlertOnBlank($("#ddlPatientType"), "Patient Type is missing! Please Select Patient type")) return;
        fillOtDetailGrid();
        $("#OTdetailleftPoPupModal").dialog({
            height: 450,
            width: 700,
            modal: true,
            open: function (evt, ui) {
                var reqId = $("#RequestId").val();
                if (parseInt(reqId) > 0) GetRequestOtDetailById(reqId);
            }
        });
    }
    else if ($('#ddlMangementType').val() === "1") {
        $("#btnOTDetailleft").prop("disabled", true);
    }
    var mngmnt = parseInt($('#ddlMangementType').val());
    selectedControls = mngmnt;
    if (mngmnt > 0) {
        debugger;
        loadNotificationData(mngmnt);
    }
    else {
        loadNotificationData(0);
    }
});
//$('#btnExpertReview').on('click', function () {
//    debugger;
//    $('.attachment').each(function () {
//        debugger;
//        var id = this.id;
//            $("#"+id).prop("disabled", false);
//    });
//});



//$('#ddlMangementType').on('change', function () {
//    if ($('#ddlMangementType').val() === "1") {

//    }
//});




























loadNotificationDataInitialy();

//$("#EntryForm").click(function(evt) {
//    var clicked = evt.target;
//    selectedControls = clicked.id;
//    if (parseInt($('#ddlMangementType').val()) > 0) loadNotificationData(parseInt($('#ddlMangementType').val()));
//    else {
//        loadNotificationData(0);
//    }
//});

function onFocusOfElement(parameters) {
    $("#pushNotificaton").show();
    selectedControls = parameters;
   var mngmnt = parseInt($('#ddlMangementType').val());
   if (mngmnt > 0) {
       loadNotificationData(mngmnt);
   }
        else {
            loadNotificationData(0);
        }
}

var notificationArr = [];
function loadNotificationData(param) {
    $.ajax({
        type: "GET",
        data: { Managment: param },
        traditional: true,
        url: "/HintNotification/GetAllNotification",
        success: function (record) {
            debugger;
            $('#NotificationPtag').val("");
            $('#NotificationPtag').html("");
            var selectedIId;
            var rows = record.length;
            var table = document.createElement('table');
            for (var i = 0; i < rows; i++) {
                if (record[i].ControlId === selectedControls) {
                    selectedIId = record[i].StepNo;
                } else if (selectedControls==="") {
                    loadNotificationDataInitialy();
                }
                if (record[i].StepNo >= selectedIId) {
                    var tr1 = document.createElement('tr');
                    var td1 = document.createElement('td');
                    if (record[i].NotificationId === selectedIId) td1.className = 'SelectedcssEven';
                    else {
                        td1.className = 'OtherscssEven';
                    }
                    var text1 = document.createTextNode(record[i].Message);
                    
                    td1.appendChild(text1);
                    tr1.appendChild(td1);
                    table.appendChild(tr1);
                }

            }
            $("#NotificationPtag").append(table.innerHTML);
        },
        error: function (a, exception, b) {
        }
    });
}

function loadNotificationDataInitialy() {
    $.ajax({
        type: "GET",
        traditional: true,
        data: { Managment: 0 },
        url: "/HintNotification/GetAllNotification",
        success: function (record) {
            debugger;
            $('#NotificationPtag').val("");
            $('#NotificationPtag').html("");
            var rows = record.length;
            var table = document.createElement('table');
            for (var i = 0; i < rows; i++) {
                    var tr1 = document.createElement('tr');
                    var td1 = document.createElement('td');
                    var text1 = document.createTextNode(record[i].Message);
                    td1.className = 'OtherscssEven';
                    td1.appendChild(text1);
                    tr1.appendChild(td1);
                    table.appendChild(tr1);
            }
            $("#NotificationPtag").append(table.innerHTML);
        },
        error: function (a, exception, b) {
            debugger;
        }
    });
}

//$(function () {
//    setTimeout(function () {
//        $("#pushNotificaton").hide('blind', {}, 100)
//    }, 3000);
//});

$('#btnCloseNotificaton').on('click', function () {
    $("#pushNotificaton").hide('blind', {}, 100), 3000;
});