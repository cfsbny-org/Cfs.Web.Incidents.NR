var app = angular.module('HomePageApp', []);

app.controller('HomePageController', ['$scope', '$http', function ($scope, $http) {
    

    $scope.userId = _userId;
    $scope.IE = false;



    if (_browser == 'IE') {
        $scope.IE = true;
    }
    


    $scope.NewReport = function () {
        window.location = '/incidents/';
    };

    $scope.SearchReports = function () {
        window.location = '/search/';
    };

    $scope.LaunchAnalysis = function () {
        window.open('http://cfs-sisense/app/main#/dashboards/5a94656657d1391e34d0b0b6');
    };


    $scope.EditReport = function (incidentId) {
        window.location = '/incidents/edit/' + incidentId;
    };


    $scope.ReviewReport = function (incidentId) {
        window.location = '/incidents/review/' + incidentId;
    };


    $http.get('/api/reports/' + $scope.userId).then(function (reports) {
        $scope.myReports = reports.data;
    });


    $http.get('api/reports/signatures/' + $scope.userId).then(function(reports) {
        $scope.mySigns = reports.data;
    });

}]);