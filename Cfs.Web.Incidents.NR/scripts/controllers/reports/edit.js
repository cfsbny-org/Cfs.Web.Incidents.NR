var app = angular.module('EditReportApp', []);

app.controller('EditIncidentController', ['$scope', '$http', function ($scope, $http) {

    $scope.incidentId = _incidentId;
    $scope.userId = _userId;
    $scope.staffName = _staffName;
    $scope.staffTitle = _staffTitle;
    $scope.staffEmail = _staffEmail;
    $scope.stationName = _stationName;
    $scope.isReadOnly = true;


    // Documents
    $scope.documentFileType = 0;
    $scope.documentTitle = '';
    $scope.documentComments = '';
    $scope.documentFile = [];
    $scope.documents = [];
    $scope.attachments = [];




    $scope.signatureValidated = true;
    $scope.isSaving = false;
    $scope.progressMessage = '';


    $scope.levels = [{ name: 'Low', value: 1 }, { name: 'Medium', value: 3 }, { name: 'High', value: 5 }];




    $scope.accessData = {
        reportId: $scope.incidentId,
        userId: $scope.userId,
        comments: '',
        stationName: ''
    };



    $http.post('/api/reports/user-access', $scope.accessData).then(function (accessResponse) {

        switch (accessResponse.data) {
            case 0: // No Access
                window.location = '/incidents/noaccess/';
                break;
            case 1: // Read Only
                break;
            case 2: // Read-Write Access
                $scope.isReadOnly = false;
                break;
            default:
                break;
        }

        $http.get('/api/programs').then(function (programsResponse) {
            $scope.programs = programsResponse.data;

            $http.get('/api/incidenttypes').then(function (incidentTypesResponse) {
                $scope.incidentTypes = incidentTypesResponse.data;

                $http.get('/api/report/header/' + $scope.incidentId).then(function (header) {
                    $scope.header = header.data;

                    $http.get('/api/report/details/' + $scope.incidentId).then(function (details) {
                        $scope.details = details.data;

                        
                        $scope.GetIncidentStaff();
                        $scope.GetIncidentDocuments();

                    });

                });



            });


        });
    });



    $scope.GetIncidentStaff = function () {
        $http.get('/api/staff/' + $scope.incidentId).then(function (staffResponse) {
            $scope.incidentStaff = staffResponse.data;
        });
    };


    $scope.GetIncidentDocuments = function () {
        $http.get('/api/documents/' + $scope.incidentId).then(function (documentsResponse) {
            $scope.incidentDocuments = documentsResponse.data;
        });
    };





    $('#staff-search').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/api/staff/suggest/' + request.term,
                type: 'GET',
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.staffName + ', ' + item.staffTitle,
                            staffName: item.staffName,
                            staffTitle: item.staffTitle,
                            userId: item.userId
                        }
                    }));
                }
            });
        },
        select: function (event, ui) {

            
            var incidentStaff = {
                incidentStaffId: 0,
                incidentId: $scope.incidentId,
                userId: ui.item.userId,
                staffName: ui.item.staffName,
                staffTitle: ui.item.staffTitle,
                isPrimary: false
            };


            $http.post('/api/staff/single', incidentStaff).then(function () { $scope.GetIncidentStaff(); });
            

            $(this).val('');
            return false;
        },
        minLength: 3
    });



    $scope.RemoveStaff = function (staff) {
        $http.delete('/api/staff/' + staff.incidentStaffId).then(function (response) {
            $scope.GetIncidentStaff();
        });
    };




    $scope.UploadFile = function (files) {
        var formData = new FormData();
        formData.append("file", files[0]);
        formData.append("uploadedBy", $scope.userId);
        formData.append("documentTitle", $scope.documentTitle);
        formData.append("documentComments", $scope.documentComments);

        console.log(files[0]);

        $http.post('/api/documents', formData, {
            withCredentials: true,
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).then(function (uploadResponse) {

            var attachment = {
                incidentAttachmentId: 0,
                incidentId: $scope.incidentId,
                documentId: uploadResponse.data
            };

            $http.post('/api/attachments/single', attachment).then(function () {
                $scope.GetIncidentDocuments();
            });

        });
    };


    $scope.RemoveDocument = function (document) {
        var index = $scope.documents.indexOf(document);
        $scope.documents.splice(index, 1);
        return false;
    };





    $scope.SaveReport = function () {


        $scope.isSaving = true;

        $scope.header.incidentReportTypeId = 2;  // NON-RESIDENTIAL INCIDENT REPORT FLAG
        $scope.header.userId = $scope.userId;
        $scope.header.staffName = $scope.staffName;
        $scope.header.staffTitle = $scope.staffTitle;
        $scope.header.createdStation = $scope.stationName;
        $scope.header.reportingAgency = '';
        $scope.header.statusId = 1;
        $scope.header.lastModifiedBy = $scope.userId;


        //var incidentTime = moment($scope.incidentTime).format('HH:mm');
        //$scope.header.incidentDate = moment($scope.header.incidentDate).format('YYYY-MM-DD') + ' ' + incidentTime;


        $scope.progressMessage = 'Validating user...';



        $http.post('/api/user/validate', '"' + $scope.credentials + '"').then(function (response) {





            if (response.data === "true") {

                $scope.progressMessage = 'Saving report header...';

                $http.post('/api/reportheaders', $scope.header).then(function (headerResponse) {

                    var incidentId = headerResponse.data;
                    $scope.details.incidentId = incidentId;



                   
                    
                    $scope.progressMessage = 'Saving report details...';


                    $http.post('/api/reportdetails', $scope.details).then(function (detailResponse) {

                      
                        window.location = "/";

                    });
                });
            }
            else {
                //console.log('if response.data === ' + response.data);
                $scope.signatureValidated = false;
            }
        }, function errorCallback(response) {
            $scope.isSaving = false;
            $scope.signatureValidated = false;
        });
    };



    $scope.FinalApproveReport = function () {


        $scope.isSaving = true;

        $scope.header.incidentReportTypeId = 2;  // NON-RESIDENTIAL INCIDENT REPORT FLAG
        $scope.header.userId = $scope.userId;
        $scope.header.staffName = $scope.staffName;
        $scope.header.staffTitle = $scope.staffTitle;
        $scope.header.createdStation = $scope.stationName;
        $scope.header.reportingAgency = '';
        $scope.header.statusId = 5;
        $scope.header.lastModifiedBy = $scope.userId;


        var incidentTime = moment($scope.incidentTime).format('HH:mm');
        $scope.header.incidentDate = moment($scope.header.incidentDate).format('YYYY-MM-DD') + ' ' + incidentTime;


        $scope.progressMessage = 'Validating user...';


        $http.post('/api/user/validate', '"' + $scope.credentials + '"').then(function (response) {



            if (response.data === "true") {


                $scope.progressMessage = 'Saving report header...';


                $http.post('/api/reportheaders', $scope.header).then(function (headerResponse) {

                    var incidentId = headerResponse.data;
                    $scope.details.incidentId = incidentId;


                    $scope.progressMessage = 'Saving report details...';


                    $http.post('/api/reportdetails', $scope.details).then(function (detailResponse) {

                        var reportIds = {
                            reportId: incidentId,
                            userId: $scope.userId
                        };

                        $scope.progressMessage = 'Creating approvals...';

                        $http.post('/api/signatures/create', reportIds).then(function () {

                            $scope.progressMessage = 'Finalizing save and transferring to home page...';
                            window.location = "/";
                        });

                    });

                });
            }
            else {
                $scope.signatureValidated = false;
            }
        }, function errorCallback(response) {
            $scope.isSaving = false;
            $scope.signatureValidated = false;
        });


    };


    $scope.PrintReport = function () {
        window.open('/reports/incidentreport/' + $scope.incidentId);
    };

    

}]);