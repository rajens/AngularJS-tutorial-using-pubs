var pubsTitles = angular.module("pubsTitles", ["ngResource", "ngRoute"]).
    config(function ($routeProvider) {
        $routeProvider.
            when('/', { controller: ListCtrl, templateUrl: 'list.html' }).
            otherwise({ redirectTo: '/' }).
            when('/edit/:editId', { controller: EditCtrl, templateUrl: 'details.html' }).
            otherwise({ redirectTo: '/' }).
            when('/new', { controller: CreateCtrl, templateUrl: 'details.html' }).
            otherwise({ redirectTo: '/' });
    });

pubsTitles.factory('titles', function ($resource) {
    return $resource('/api/titles/:id', { id: '@id' }, { update: { method: 'PUT' } });
});

var EditCtrl = function ($scope, $location, $routeParams, titles) {
    $scope.action = "Update";
    var id = $routeParams.editId;
    $scope.title = titles.get({ id: id });

    $scope.save = function () {
        title.update({ id: id }, $scope.title, function(){
            $location.path('/');
        })
    };
};

var CreateCtrl = function ($scope, $location, titles) {
    $scope.action = "Add";
    $scope.save = function () {
        titles.save($scope.title, function () {
            $location.path('/');
        });
    };
};

var ListCtrl = function ($scope, $location, titles) {
    $scope.sort_order = "title1";
    $scope.sort_desc = false;

    $scope.sort_by = function (col) {
        if ($scope.sort_order === col) {
            $scope.sort_desc = !$scope.sort_desc;
        } else {
            $scope.sort_order = col;
            $scope.sort_desc = false;
        }
            $scope.reset();     // Start searching from the first set of data
    };

    $scope.reset = function () {
        $scope.offset = 0;
        $scope.titles = [];
        $scope.limit = 5;
        $scope.more = true;
        $scope.search();
    };

    $scope.search = function () {
        titles.query({
                    q: $scope.query,
                    sort: $scope.sort_order,
                    desc: $scope.sort_desc,
                    offset: $scope.offset,
                    limit: $scope.limit
        },
        function (data) {
            $scope.more = (data.length == $scope.limit);
            $scope.titles = $scope.titles.concat(data);
        });
    };

    $scope.delete_title = function () {
        var id = this.eachtitle.title_id;
        titles.delete({ id: id }, function () {
            $('#title_' + id).fadeOut();
        });
    };

    $scope.istheremore = function () {
        return ($scope.more);
    };

    $scope.show_more = function() {
        $scope.offset += $scope.limit;
        $scope.search();
    };

    $scope.reset();
};

pubsTitles.directive('sorted', function () {
    return {
        scope: true,
        transclude: true,
        template: '<a ng-click="do_sort()" ng-transclude></a>' +
                    '<span ng-show="do_show(true)"><i class="glyphicon glyphicon-circle-arrow-down"></i></span>' +
                    '<span ng-show="do_show(false)"><i class="glyphicon glyphicon-circle-arrow-up"></i></span>',
        controller: function ($scope, $element, $attrs) {
            $scope.sort = $attrs.sorted;

            $scope.do_sort = function () { $scope.sort_by($scope.sort); };

            $scope.do_show = function (asc) {
                return (asc != $scope.sort_desc) && ($scope.sort_order == $scope.sort);
            };
        }
    }
});
