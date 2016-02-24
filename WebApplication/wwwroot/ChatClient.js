angular
    .module('chatClient', ['SignalR'])
    .controller('userController', function($scope, $log, $rootScope, chatService) {
        $scope.users = [];

        $rootScope.$on('chatService', function (data, args) {
            if (args === 'connected') {
                chatService.getUsers().subscribeOnNext(function(users) {
                    $scope.users = users;
                    $scope.$apply();
                });
            } else {
                $scope.users = [];
                $scope.$apply();
            }
        });

        chatService.userStatusStream.subscribeOnNext(function(args) {
            for (var i = 0; i < $scope.users.length; ++i) {
                if ($scope.users[i].Name === args.name) {
                    $scope.users[i].IsOnline = args.isOnline;
                    break;
                }
            }
            $scope.$apply();
        });

    })
    .controller('chatRoomController', function ($scope, $rootScope, $log, chatService) {
        $scope.rooms = [];
        $scope.isInRoom = false;
        $scope.currentRoom = '';
        $scope.messages = [];
        $scope.inputMessage = '';

        var token = null;

        $rootScope.$on('chatService', function (data, args) {
            if (args === 'connected') {
                chatService.getRooms().subscribeOnNext(function (rooms) {
                    $scope.rooms = rooms;
                    $scope.$apply();
                });
            } else {
                $scope.rooms = [];
                $scope.$apply();
            }
        });

        $scope.onEnterRoom = function(roomName) {
            $scope.onLeaveRoom();
            $log.debug('entering chat room ' + roomName);

            $scope.currentRoom = roomName;
            $scope.isInRoom = true;
            token = chatService.enterRoom(roomName).subscribeOnNext(function(msg) {
                $scope.messages.unshift(msg);
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            });
        };

        $scope.onLeaveRoom = function() {
            if (token) {
                $scope.isInRoom = false;
                $log.debug('leaving chat room ' + $scope.currentRoom);
                $scope.messages = [];
                $scope.currentRoom = '';
                $scope.inputMessage = '';
                token.dispose();
            }
        }

        $scope.onKeyPress = function ($event) {
            if ($event.key === 'Enter' && $scope.inputMessage.length > 0) {
                $scope.onSendMessage();
            }
        };

        $scope.onSendMessage = function () {
            var msg = {
                Room: $scope.currentRoom,
                Message: $scope.inputMessage
            };
            chatService.sendMessage(msg);
            $scope.inputMessage = '';
        };
    })
    .controller('chatController', function ($scope, chatService, $rootScope, $log) {
        $scope.serviceState = '';
        $scope.isConnected = false;

        var onConnect = function() {
            $scope.isConnected = true;
        }

        var onDisconnect = function() {
            $scope.isConnected = false;
        }

        $rootScope.$on('chatService', function (data, args) {
            $log.info('received chatService event: ' + JSON.stringify(args));
            $scope.serviceState = args;
            if (args === 'connected') {
                onConnect();
            } else {
                onDisconnect();
            }
            $scope.$apply();
        });
    })
    .factory('chatService', function (Hub, $log, $rootScope) {
        var userNotificationStream = new Rx.Subject();
        var messageStream = new Rx.Subject();

        var hub = new Hub('ChatService', {
            listeners: {
            },
            methods: [],
            errorHandler: function(error) {
                $log.error(error);
            },
            stateChanged: function(state) {
                switch (state.newState) {
                case $.signalR.connectionState.connecting:
                    $log.info('connecting...');
                    $rootScope.$emit('chatService', 'connecting');
                    break;
                case $.signalR.connectionState.connected:
                    $log.info('connected!');
                    $rootScope.$emit('chatService', 'connected');
                    break;
                case $.signalR.connectionState.reconnecting:
                    $log.info('reconnecting...');
                    $rootScope.$emit('chatService', 'reconnecting');
                    break;
                case $.signalR.connectionState.disconnected:
                    $log.info('disconnected :(');
                    $rootScope.$emit('chatService', 'disconnected');
                    break;
                }
            }
        });

        var getUsers = function() {
            return Rx.Observable.return([]);
        };

        var getRooms = function() {
            return Rx.Observable.return([]);
        }
        
        var sendMessage = function(msg) {
            $log.debug(JSON.stringify(msg));
        }

        var enterRoom = function (roomName) {
            return Rx.Observable.return({});
        }

        $log.debug('chatService client initialized');

        return {
            userStatusStream: userNotificationStream,
            getUsers: getUsers,
            getRooms: getRooms,
            enterRoom: enterRoom,
            sendMessage: sendMessage
        };
    });
