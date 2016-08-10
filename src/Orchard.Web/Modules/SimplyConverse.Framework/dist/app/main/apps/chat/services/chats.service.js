(function () {
    'use strict';

    angular
        .module('app.chat')
        .factory('ChatsService', ChatsService);

    /** @ngInject */
    function ChatsService($q, msApi, $firebaseArray, $firebaseObject, $firebaseAuth) {

        //var ref = firebase.database(); //new Firebase('https://kwikly-72df6.firebaseio.com/');

        var ref = firebase.database().ref();

        var authObj = $firebaseAuth();
        authObj.$signInAnonymously().then(function (firebaseUser) {
            console.log("Signed in as:", firebaseUser.uid);
        }).catch(function (error) {
            console.error("Authentication failed:", error);
        });


        var messages = $firebaseArray(ref.child('messages'));
        var conversations = $firebaseArray(ref.child('conversations'));
        var contacts = $firebaseArray(ref.child('users'));

        messages.$loaded()
            .then(function(x) {
                x === messages; // true
            })
            .catch(function(error) {
                console.log("Error:", error);
            });

        var service = {
            chats: {},
            contacts: [],
            getContactChat: getContactChat,
            setContactChat: setContactChat,
            messages: messages,
            conversations: conversations,
            getConversationMessages: getConversationMessages,
            replyToConversation:replyToConversation,
            getConversation:getConversation
        };

        function getConversationMessages(conversationId) {
            return messages.$getRecord(conversationId);
        }

        function getConversation(conversationId) {
            return conversations.$getRecord(conversationId);
        }

        function replyToConversation(conversationId, message) {
           //var msgs =  getConversationMessages(conversationId);
          var conMessages =  $firebaseArray(ref.child('messages/'+conversationId));
          conMessages.$add(message).then(function(ref) {
               var id = ref.key;
               console.log("added message with id " + id);
            });;
        }

        /**
         * Get contact chat from the server
         *
         * @param contactId
         * @returns {*}
         */
        function getContactChat(contactId) {

            // Create a new deferred object
            var deferred = $q.defer();

            // If contact doesn't have lastMessage, create a new chat
            if ((!service.contacts.getById(contactId)).lastMessage) {
                service.chats[contactId] = [];

                deferred.resolve(service.chats[contactId]);
            }

            // If the chat exist in the service data, do not request
            if (service.chats[contactId]) {
                deferred.resolve(service.chats[contactId]);

                return deferred.promise;
            }

            // Request the chat with the contactId
            msApi.request('chat.chats@get', { id: contactId },

                // SUCCESS
                function (response) {
                    // Attach the chats
                    service.chats[contactId] = response.data;

                    // Resolve the promise
                    deferred.resolve(service.chats[contactId]);
                },

                // ERROR
                function (response) {
                    deferred.reject(response);
                }
            );

            return deferred.promise;
        }

        function setContactChat(contact) {

            // Create a new deferred object
            // var deferred = $q.defer();

            var contactId = contact.id;
            if (service.contacts.getById(contactId) == null) {
                service.contacts.push(contact);
            }

            // If contact doesn't have lastMessage, create a new chat
            if (service.contacts.getById(contactId)) {
                if (service.chats[contactId] == null) {
                    service.chats[contactId] = contact.chats;
                }
                else {
                    service.chats[contactId] = service.chats[contactId].concat(contact.chats);
                }
                if (contact.chats)
                    service.contacts.getById(contactId).lastMessage = contact.chats[(contact.chats).length - 1];
            }
            return true;
        }

        /**
         * Array prototype
         *
         * Get by id
         *
         * @param value
         * @returns {T}
         */
        Array.prototype.getById = function (value) {
            return this.filter(function (x) {
                return x.id === value;
            })[0];
        };
        return service;
    }
})();