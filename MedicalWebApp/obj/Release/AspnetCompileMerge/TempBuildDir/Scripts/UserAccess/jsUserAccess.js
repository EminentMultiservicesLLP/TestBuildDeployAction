var $gridAccessMain;
$(document).ready(function () {
    $("#UserRow").hide();
    $("input[name='chklAccessByUser']").click(function () {
        ClearParamGrid("grid_md");

        var chk = $("input:radio[name='chklAccessByUser']:checked").val();
        if (chk === "Yes") {
            $("#UserRow").show();
            $("#NewRoleRow").hide();
            $("#ddlRole").prop("disabled", false);
        } else {
            $("#UserRow").hide();
            $("#NewRoleRow").show();
            $("#ddlRole").selectedIndex = 0;
            $("#ddlRole").val(0).trigger('chosen:updated');
        }
        
    });

    $("#txtNewRole")
        .on('propertychange change keyup paste input',
            function () {
                $("#ddlRole").selectedIndex = 0;
                if ($(this).val().trim().length > 0) {
                    $("#ddlRole").prop("disabled", true);
                }
                else
                    $("#ddlRole").prop("disabled", false);
                $("#ddlRole").val(0).trigger('chosen:updated');
                ClearParamGrid("grid_md");
            });
    //$("#txtUser").change($("#txtUserId").val(item.EmpId));
    $("#txtUser").autocomplete({
            source: function (request, response) {
                $("#txtUserId").val("");
                if ($("#txtUser").val().trim().length > 2) {
                $.ajax({
                    url: '/UserAccess/UserAccess/AllStaffEmployees/',
                    data: JSON.stringify({ prefix: request.term }),
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data,
                        //response($.each(data,
                            function(item) {
                              
                               // $("#txtUserId").val(item.EmpId);
                                return { label: item.EmployeeName, value: item.EmployeeName ,userid:item.UserId}
                            }));
                    },
                    error: function(response) {
                        alert(response.responseText);
                    },
                    failure: function(response) {
                        alert(response.responseText);
                    }
                });
            }
        },
        select: function (e, i) {
            $("#txtUserId").val(i.item.userid);
            $("#txtUser").val(i.item.label);
          
        },
        minLength: 1
    });

    $.ajax({
        type: "GET",
        url: "/UserAccess/UserAccess/GetRoleList",
        datatype: "Json",
        success: function (data) {
            $('#ddlRole').append('<option value=0>-- Select Role --</option>');
            $.each(data.data, function (index, value) {
                $('#ddlRole').append('<option value="' + value.RoleId + '">' + value.RoleName + '</option>');
            });
        }
    });

    var colM = [
        { title: "", minWidth: 27, width: 27, type: "detail", resizable: false, editable: false, show:true },
       { title: "MenuId", width: 10, dataIndx: "MenuId", hidden:true },
        { title: "Menu", width: 180, dataIndx: "MenuName", dataType: "string" }
      ];

    var menuColModel = [
        { title: "MenuId", width: 10, dataIndx: "MenuId", hidden: true },
        { title: "Sub Menu", width: 300, dataIndx: "MenuName", dataType: "string" },
       
        {
            title: "<input type='checkbox'/> &nbsp;Access &nbsp;",
            width: 120,
            dataIndx: "IsAccess",
            align: "center",
            sortable: false,
            type: 'checkBoxSelection',
            cls: 'ui-state-default',
            dataType: 'bool',
            cb: { select: true, all: true, header: true }
        }
       
    ];

    var detailsICon = [{ title: "", minWidth: 27, width: 27, type: "detail", resizable: false, editable: false }];

    var dataModel = {
        location: "remote",
        sorting: "local",
        dataType: "JSON",
        method: "GET",
        rPPOptions: [1, 10, 20, 30, 40, 50, 100, 500, 1000],
        url: "/UserAccess/UserAccess/GetMenuRoleList",
        getData: function (dataJson) {
            var data = dataJson.data;
            //if (data && data.length) {
            //    data[0]['pq_detail'] = { 'show': true }; // this is to expande first row//
            //}
            return { curPage: dataJson.curPage, totalRecords: dataJson.totalRecords, data: data };
        }
    }

    $gridAccessMain = $("div#grid_md").pqGrid({
        width: '100%',
        height: 'flex',
        //dataModel: dataModel,
        virtualX: true, virtualY: true,
        editable: false,
        colModel: colM,
        wrap: false,
        hwrap: false,
        numberCell: { show: false },
        title: "<b>Menu Details</b>",
        resizable: true,
        freezeCols: 1,
        selectionModel: { type: 'cell' },

        detailModel: {
            cache: true,
            collapseIcon: "ui-icon-plus",
            expandIcon: "ui-icon-minus",
            init: function (ui) {
                var rowData = ui.rowData,
                    detailobj = showSubMenu(rowData),                       
                    $grid = $("<div/>").pqGrid(detailobj); 

                return $grid;
            }
        }
    });

    var showSubMenu = function (rowData) {
        debugger;
        var childData = rowData.ChildMenu;
        return {
            dataModel: { data: childData },
            height: 130,
            pageModel: { type: "local", rPP: 5, strRpp: "" },
            colModel: detailsICon.concat(menuColModel),
            detailModel: {
                cache: true,
                collapseIcon: "ui-icon-plus",
                expandIcon: "ui-icon-minus",
                init: function (ui) {
                    var rowData = ui.rowData,
                        leafobj = showLeafMenu(rowData),                       
                        $leafGrid = $("<div/>").pqGrid(leafobj); //init the detail grid.
                    return $leafGrid;
                }
            },
            flexHeight: true,
            flexWidth: true,
            numberCell: { show: false },
            showTop: false,
            showBottom: false
        };
    };

    var showLeafMenu = function (rowData) {
        debugger;
        var leafMenu = rowData.ChildMenu;
        return {
            dataModel: { data: leafMenu },
            height: 130,
            pageModel: { type: "local", rPP: 5, strRpp: "" },
            colModel: menuColModel,
            flexHeight: true,
            flexWidth: true,
            numberCell: { show: false },
            showTop: false,
            showBottom: false
        };
    };

    //$("#ddlRole").change(function () {
    //    $("#txtUserId").val('');
    //    $("#txtNewRole").val('');
    //    var roleid = $("#ddlRole").val();
    //    if (roleid > 0) {
    //        LoadRoleBasedMenu(roleid);
    //    }
    //    else
    //        ClearParamGrid("grid_md");
    //});
});

function LoadRoleBasedMenu() {
    var roleId = 0, userId = 0;
    var chk = $("input:radio[name='chklAccessByUser']:checked").val();
    if (chk === "Yes") {
        userId = $("#txtUserId").val();
        roleId = $("#ddlRole").val();
    } else {
        userId = 0;
        roleId = $("#ddlRole").val();
    }

    $.ajax($.extend({},
                //ajaxObj,
                {
                    context: $gridAccessMain,
                    url: "/UserAccess/UserAccess/GetMenuRoleList",
                    data: { roleId: roleId, userId: userId },
                    success: function (data) {
                        $gridAccessMain.pqGrid("option", "dataModel.data", data.data);
                        $gridAccessMain.pqGrid("refreshDataAndView");

                                            },
                    error: function () {
                        $gridAccessMain.pqGrid("removeClass", { rowData: rowData, cls: 'pq-row-delete' });
                        $gridAccessMain.pqGrid("rollback");
                    }
                }));
}

var accessDetails = []; var iCount = 0;
$('#btnAccessSave').on('click', function () {
    accessDetails = [];
    DisableClick("btnAccessSave");
    var data = $gridAccessMain.pqGrid("option", "dataModel.data");
    if (data.data && data.data.length) {
        $.each(data.data, function (k) {
            data.data[k]['pq_detail'] = { 'show': true };
        });
    }
    for (var i = 0; i < data.length; i++) {
        var rd = data[i],
            dt = rd.pq_detail;
        iCount = 0; // start check counter again

        if (ReadCheckedChildMenu(rd, dt) > 0) {
            //if (accessDetails.indexOf(detail.MenuId) <= 0)
            accessDetails.push(rd);
        }
    }
  
    debugger;
    var roleId = 0, userId = 0, roleName = "", isNewRole = false, saveUrl = "", mAccess ="";
    var chk = $("input:radio[name='chklAccessByUser']:checked").val();
    if (chk === "Yes") {
        userId = $("#txtUserId").val();
        roleId = $("#ddlRole").val();
    } else {
        userId = 0;
        if ($("#txtNewRole").val().trim().length > 0) {
            isNewRole = true;
            roleName = $("#txtNewRole").val().trim();
        }
        roleId = $("#ddlRole").val();
    }

    if (isNewRole) {
        mAccess = JSON.stringify({
            roleName: roleName,
            roleAccess: accessDetails
        });
       
        saveUrl = '/UserAccess/UserAccess/CreateNewRoleAccess';
    } else {
        debugger;
        mAccess = JSON.stringify({
            roleid: roleId,
            userId: userId,
            roleAccess: accessDetails
        });
        saveUrl = '/UserAccess/UserAccess/SaveRoleAccess';
    }
    $.ajax({
        type: "POST",
        traditional: true,
        contentType: 'application/json; charset=utf-8',
        url: saveUrl,
        data: mAccess,
        success: function (msg) {
            if (msg.success === true) {
                ClearParamGrid("grid_md");
                ShowAlert("success", "Access for Role setup successfully.");
                ClearForm();
            }
            else {
                ShowAlert("error", msg.responseText);
            }
        },
        error: function (jqXhr, exception) {
            ShowAlert("error", "Server Error! Please contact administrator!");
        }
    });
});


function ReadCheckedChildMenu(parentRow, childGridDetail) {
    debugger;
    if (childGridDetail && childGridDetail.child) {
        var child = $(childGridDetail.child).pqGrid("option", "dataModel.data");
        if (child != null) {
            $.each(child, function (_, detail) {
                if (detail != null) {
                    if (detail.IsAccess) {
                        ReadCheckedChildMenu(detail, detail.pq_detail);
                        accessDetails.push(parentRow);
                        accessDetails.push(detail);
                        iCount++;
                    } else {
                        ReadCheckedChildMenu(detail, detail.pq_detail);
                    }
                }
            });
        }
    }
    return iCount;
}

//function GetParentGridRow($detailGrid) {   core
//    var $tr = $detailGrid.closest('tr');
//    if ($tr) {
//        var obj = $grid.pqGrid('getRowIndx', { $tr: $tr });
//        var rowData = $grid.pqGrid('getRowData', obj);
//        accessDetails.push(rowData);

//        GetParentGridRow()
//    }
//}

$("#btnReset").on("click", ClearForm);
function ClearForm() {
    $("#txtNewRole").val("");
    $("#ddlRole").val("");
    $("#txtUser").val("");
    $("#txtUserId").val("");
    ClearParamGrid("grid_md");
    $("#ddlRole").prop("disabled", false);
}