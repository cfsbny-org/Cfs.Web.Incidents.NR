var app = angular.module('ReviewReportApp', ['ui.bootstrap']);

app.controller('ReviewIncidentController', ['$scope', '$http', function ($scope, $http) {

    $scope.incidentId = _incidentId;
    $scope.userId = _userId;
    $scope.staffName = _staffName;
    $scope.staffTitle = _staffTitle;
    $scope.staffEmail = _staffEmail;
    $scope.stationName = _stationName;
    $scope.isReadOnly = true;
    $scope.newAdminComment = '';
    $scope.credentials = '';
    $scope.signOnBehalf = null;


    $scope.levels = [{ name: 'Low', value: 1 }, { name: 'Medium', value: 3 }, { name: 'High', value: 5 }];


    $scope.notifyDate = new Date();
    $scope.notifyTime = moment().format('hh:mm a');


    $scope.accessData = {
        reportId: $scope.incidentId,
        userId: $scope.userId,
        comments: '',
        stationName: ''
    };



    $http.post('/api/reports/approver-access', $scope.accessData).then(function (accessResponse) {

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

                $http.get('/api/notifyparties').then(function (notifyPartiesResponse) {
                    $scope.notifyParties = notifyPartiesResponse.data;


                    $http.get('/api/report/header/' + $scope.incidentId).then(function (header) {
                        $scope.header = header.data;

                        $http.get('/api/report/details/' + $scope.incidentId).then(function (details) {
                            $scope.details = details.data;
                            $scope.incidentTypeSelected = '';


                            $http.get('/api/staff/' + $scope.incidentId).then(function (staffResponse) {
                                $scope.incidentStaff = staffResponse.data;
                            });

                            $http.get('/api/documents/' + $scope.incidentId).then(function (documentsResponse) {
                                $scope.incidentDocuments = documentsResponse.data;
                            });


                            $http.get('/api/comments/report/' + $scope.incidentId).then(function (comments) {
                                $scope.comments = comments.data;
                            });


                            $http.get('/api/incidentTypes/' + $scope.header.incidentTypeId).then(function (types) {
                                $scope.incidentTypeSelected = types.data.incidentTypeText;
                            });


                            $scope.GetNotifications();
                            $scope.GetReportSignatures();
                            $scope.GetAvailableSignatures();

                        });
                    });


                });
            });


        });


    });





    $scope.GetReportSignatures = function () {

        $http.get('/api/signatures/report/' + $scope.incidentId).then(function (signatures) {
            $scope.signatures = signatures.data;
        });
    };



    $scope.GetAvailableSignatures = function () {

        var params = {
            reportId: $scope.incidentId,
            userId : $scope.userId,
            comments: '',
            stationName: ''
        };

        $http.post('/api/signatures/available', params).then(function (signatures) {
            $scope.availableSignatures = signatures.data;
        });
    };



    $scope.SaveComment = function () {

        if ($scope.newAdminComment != '') {
            var comment = {
                incidentId: $scope.incidentId,
                adminUserId: $scope.userId,
                adminUserName: $scope.staffName,
                adminCommentText: $scope.newAdminComment,
                adminCommentStamp: new Date()
            };

            $http.post('/api/comments', comment).then(function () {
                $http.get('/api/comments/report/' + $scope.incidentId).then(function (comments) {
                    $scope.comments = comments.data;
                    $scope.newAdminComment = '';
                });
            },
            function (errorResponse) {
                // Handle error
                alert(response.statusText);
            });
        }
    };



    $scope.DeleteComment = function (id) {
        $http.delete('/api/comments/delete/' + id).then(function () {
            $http.get('/api/comments/report/' + $scope.incidentId).then(function (comments) {
                $scope.comments = comments.data;
            });
        });
    };



    $scope.NotifyCompliance = function () {
        $http.post('/api/signatures/compliance', '"' + $scope.incidentId + '"').then(function () {
            window.alert('Compliance notified.');
            $scope.GetReportSignatures();
        });
    };


    $scope.NotifyCoo = function () {
        $http.post('/api/signatures/coo', '"' + $scope.incidentId + '"').then(function () {
            window.alert('COO notified.');
            $scope.GetReportSignatures();
        });
    };


    $scope.SelectNotifier = function () {
        $scope.notifyMethod = $scope.notifyParty.methodDefault;
    };




    $scope.AddNotification = function () {

        var isTimeValid = /^([0-1]\d):([0-5]\d)\s*(?:AM|am|PM|pm)?$/.test($scope.notifyTime);

        if (!isTimeValid) {
            window.alert('Contacted Time is not valid. Entry must be in \'hh:mm am\' format.');
            return;
        }


        var notifyDateTime = moment($scope.notifyDate).format('MM/DD/YYYY') + ' ' + $scope.notifyTime;
        
        var notification = {
            notificationId: 0,
            incidentId: $scope.incidentId,
            notifyPartyId: $scope.notifyParty.notifyPartyId,
            notifyDateTime: notifyDateTime,
            notifyMethod: $scope.notifyMethod,
            notifyContact: $scope.notifyContact,
            notifyComments: $scope.notifyComments,
            acknowledgeUserId: $scope.userId
        };

        $http.post('/api/notifications', notification).then(function () {
            $scope.GetNotifications();
        });
    };



    $scope.GetNotifications = function () {

        $http.get('/api/notifications/' + $scope.incidentId).then(function (notificationsResponse) {
            $scope.incidentNotifications = notificationsResponse.data;
        });
    };



    $scope.ApproveReport = function () {

        $http.post('/api/user/validate', '"' + $scope.credentials + '"').then(function (isValidated) {


            if ($scope.signOnBehalf == null) {      // SIGN REPORT FOR CURRENT USER


                var data = {
                    reportId: $scope.incidentId,
                    userId: $scope.userId,
                    comments: $scope.approvalComments,
                    stationName: $scope.stationName
                };


                $http.post('/api/signature/final-approve', data).then(function (response) {
                    window.location = '/';
                });


            }
            else {                                  // SIGN REPORT FOR ANOTHER SIGNATOR
                var data = {
                    reportSigId: $scope.signOnBehalf,
                    comments: 'Signed by ' + $scope.staffName,
                    stationName: $scope.stationName
                };


                $http.post('/api/signature/delegated-approval', data).then(function (response) {
                    $scope.GetReportSignatures();
                    $scope.GetAvailableSignatures();
                });
            }


        });

        
        
    };




    $scope.RejectReport = function () {


        $http.post('/api/user/validate', '"' + $scope.credentials + '"').then(function (isValidated) {

            var data = {
                reportId: $scope.incidentId,
                userId: $scope.userId,
                comments: $scope.approvalComments,
                stationName: $scope.stationName
            };


            $http.post('/api/signature/reject', data).then(function (response) {
                window.location = '/';
            });

        });
    };




    $scope.PrintReport = function () {
        window.open('/reports/incidentreport/' + $scope.incidentId);
    };



}]);