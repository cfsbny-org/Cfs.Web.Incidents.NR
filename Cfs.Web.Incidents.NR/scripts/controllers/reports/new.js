var app = angular.module('NewReportApp', [])
.directive('appDatetime', function ($window) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            var moment = $window.moment;

            ngModel.$formatters.push(formatter);
            ngModel.$parsers.push(parser);

            element.on('change', function (e) {
                var element = e.target;
                element.value = formatter(ngModel.$modelValue);
            });

            function parser(value) {
                var m = moment(value);
                var valid = m.isValid();
                ngModel.$setValidity('datetime', valid);
                if (valid) return m.valueOf();
                else return value;
            }

            function formatter(value) {
                var m = moment(value);
                var valid = m.isValid();
                if (valid) return m.format("MM/DD/YYYY HH:mm:ss");
                else return value;

            }

        } //link
    };

}) //appDatetime
.controller('NewReportController', ['$scope', '$http', function ($scope, $http) {

    

    $scope.userId = _userId;
    $scope.staffName = _staffName;
    $scope.staffTitle = _staffTitle;
    $scope.staffEmail = _staffEmail;
    $scope.stationName = _stationName;
    $scope.incidentStaff = [];
    //$scope.incidentDate = new moment().format('YYYY-MM-dd');
    $scope.incidentTime;
    $scope.incidentType = 0;
    $scope.signatureValidated = true;

    $scope.isSaving = false;
    $scope.progressMessage = '';





    // Documents
    $scope.documentFileType = 0;
    $scope.documentTitle = '';
    $scope.documentComments = '';
    $scope.documentFile = [];
    $scope.documents = [];
    $scope.attachments = [];



    
    $scope.incidentStaff.push({
        incidentStaffId: 0,
        incidentId: 0,
        userId: $scope.userId,
        staffName: $scope.staffName,
        staffTitle: $scope.staffTitle,
        isPrimary: true
    });



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

            var staff = {
                userId: ui.item.userId,
                staffName: ui.item.staffName,
                staffTitle: ui.item.staffTitle
            };

            $scope.PushIncidentStaff(staff);

            $(this).val('');
            return false;
        },
        minLength: 3
    });



    $http.get('/api/report/header/new').then(function (headerResponse) {
        $scope.header = headerResponse.data;
        $scope.header.clientDob = null;
        $scope.header.incidentDate = moment(new Date()).format('YYYY-MM-DD');

        $http.get('/api/report/details/new').then(function (detailResponse) {
            $scope.details = detailResponse.data;
        });

        

    });


    $http.get('/api/programs').then(function (response) {
        $scope.programs = response.data;
    });


    $http.get('/api/incidenttypes').then(function (response) {
        $scope.incidentTypes = response.data;
    });



    $scope.PushIncidentStaff = function (staff) {

        $scope.incidentStaff.push({
            incidentStaffId: 0,
            incidentId: 0,
            userId: staff.userId,
            staffName: staff.staffName,
            staffTitle: staff.staffTitle,
            isPrimary: false
        });
        $scope.$apply();
    };



    $scope.RemoveStaff = function (staff) {
        var index = $scope.incidentStaff.indexOf(staff);
        $scope.incidentStaff.splice(index, 1);
        return false;
    };




    $scope.UploadFile = function (files) {
        var formData = new FormData();
        formData.append("file", files[0]);
        formData.append("uploadedBy", $scope.userId);
        formData.append("documentTitle", $scope.documentTitle);
        formData.append("documentComments", $scope.documentComments);
        
        //console.log(files[0]);

        $http.post('/api/documents/', formData, {
            withCredentials: true,
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).then(function (uploadResponse) {

            var document = {
                documentId: uploadResponse.data,
                documentFile: files[0].name,
                documentTitle: $scope.documentTitle,
                documentUrl: '/content/documents/' + moment().format('YYYY-MM-DD') + '/' + files[0].name,
                documentComments: $scope.documentComments
            };

            $scope.documents.push(document);

            

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

                    $.each($scope.incidentStaff, function (index, value) {
                        $scope.incidentStaff[index].incidentId = incidentId;
                    });


                    $.each($scope.documents, function (index, value) {
                        var attachment = {
                            incidentAttachmentId: 0,
                            incidentId: incidentId,
                            documentId: $scope.documents[index].documentId
                        };
                        $scope.attachments.push(attachment);
                    });




                    $http.post('/api/staff', $scope.incidentStaff).then(function () { });


                    $http.post('/api/attachments', $scope.attachments).then(function () { });



                    $http.post('/api/reportdetails', $scope.details).then(function (detailResponse) {


                        $scope.progressMessage = 'Saving report details...';

                        var reportIds = {
                            reportId: incidentId,
                            userId: $scope.userId
                        };

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


        if ($scope.header.programId == 0) {
            alert('You must select a program.');
            $scope.isSaving = false;
            return;
        }

        if ($scope.details.harmLevel == null) {
            alert('You must indicate a level of harm.');
            $scope.isSaving = false;
            return;
        }

        if ($scope.details.riskLevel == null) {
            alert('You must indicate a level of risk.');
            $scope.isSaving = false;
            return;
        }



        var incidentTime = moment($scope.incidentDate).format('HH:mm');
        $scope.header.incidentDate = moment($scope.header.incidentDate).format('YYYY-MM-DD') + ' ' + incidentTime;

        $scope.progressMessage = 'Validating user...';


        $http.post('/api/user/validate', '"' + $scope.credentials + '"').then(function (response) {

            

            if (response.data === "true") {

                $scope.progressMessage = 'Saving report header...';

                $http.post('/api/reportheaders', $scope.header).then(function (headerResponse) {

                    var incidentId = headerResponse.data;
                    $scope.details.incidentId = incidentId;


                    $.each($scope.incidentStaff, function (index, value) {
                        $scope.incidentStaff[index].incidentId = incidentId;
                    });



                    $.each($scope.documents, function (index, value) {
                        var attachment = {
                            incidentAttachmentId: 0,
                            incidentId: incidentId,
                            documentId: $scope.documents[index].documentId
                        };
                        $scope.attachments.push(attachment);
                    });



                    $scope.progressMessage = 'Saving incident staff...';

                    $http.post('/api/staff', $scope.incidentStaff).then(function () { });

                    $scope.progressMessage = 'Attaching documents...';

                    $http.post('/api/attachments', $scope.attachments).then(function () { });



                    $scope.progressMessage = 'Saving report details...';

                    $http.post('/api/reportdetails', $scope.details).then(function (detailResponse) {

                        var reportIds = {
                            reportId: incidentId,
                            userId: $scope.userId,
                            stationName: $scope.stationName
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



}]);