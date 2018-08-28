var app = angular.module('SearchApp', []);

app.controller('SearchController', ['$scope', '$http', function ($scope, $http) {

    $scope.userId = _userId;
    $scope.results = [];

    $scope.startDate = new Date(moment().add(-30, 'days').format('MM-DD-YYYY'));
    $scope.endDate = new Date();

    


    $scope.SearchReports = function () {

        var data = {
            userId: $scope.userId,
            clientName: $scope.clientName,
            startDate: $scope.startDate,
            endDate: $scope.endDate
        };


        $http.post('/api/reports/search', data).then(function (searchResponse) {
            $scope.results = searchResponse.data;
        });


    };



    $scope.ViewReport = function (incidentId) {
        window.open('/reports/incidentreport/' + incidentId);
    };



}]);