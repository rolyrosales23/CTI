var app = angular.module('app', []);

app.controller('appCtrl', ['$scope', '$http', '$rootScope', function ($scope, $http, $rootScope) {
    var vm = this;
    vm.LoginForm = {};
    vm.hideStation = true;

    vm.submitForm = function () {

        if (vm.LoginForm.roll == 1) {
            var myAvayaSocket = new AvayaWebSocket('199.47.69.35', '9102');
            var mysoc = myAvayaSocket.connect();
            myAvayaSocket.InitializeDevice(vm.station);

            //myAvayaSocket.sendcall();
            //myAvayaSocketonmessage(e);
            //myAvayaSocket.onReceive(msj);

            var url = '/' + window.location.pathname.split('/')[1] + "/Index/";
            //$(location).attr('href', url);
        }
        else {

            $.ajax({
            type: "get",
            url: '/' + window.location.pathname.split('/')[1] + '/LogUser',
            data: vm.LoginForm,
            success: function (answ) {
                if (answ.msg == "log_ok") {
                    // El logeo fue satisfactorio redirecciono el navegador al index del usuario
                    if (answ.Roll == 2) {
                        var url = "/Supervisor/Supervisor";
                        $(location).attr('href', url);
                    }
                    else if (answ.Roll == 3) {
                        var url = "/Admin/Admin";
                        $(location).attr('href', url);
                    }
                    //userLog = answ;
                }
                else {
                    if (answ.msg == "inactive") {
                        alert('Error: El usuario con el que intenta entrar al sistema no está activo.');

                        //$.messager.show({
                        //    title: "Mensaje", msg: "El usuario con el que intenta entrar al sistema no está activo.", timeout: 4000, showType: 'slide', style: {
                        //        right: '',
                        //        top: document.body.scrollCenter + document.documentElement.scrollCenter,
                        //        bottom: ''
                        //    }
                        //});
                    }
                    else {
                        alert('Error: Usuario o contraseña incorrectos.');

                        //$.messager.show({
                        //    title: "Error", msg: "Usuario o contraseña incorrectos.", timeout: 2000, showType: 'slide', style: {
                        //        right: '',
                        //        top: document.body.scrollCenter + document.documentElement.scrollCenter,
                        //        bottom: ''
                        //    }
                        //});
                    }
                }
            }
        });
        }
        
    };

    vm.ChangeRoll = function () {
        if (vm.LoginForm.roll == 1) {
            vm.hideStation = false;
            vm.station = '';
        }
        else {
            vm.hideStation = true;
            vm.station = 'null';
        }
    }
}]);