(function()
{
    'use strict';

    angular
    .module('app.chat')
    .controller('ChatController', ChatController);

    /** @ngInject */
    function ChatController(Contacts, ChatsService, $mdSidenav, User, $timeout, $document, $mdMedia, Products)
    {

        var vm = this;

        // Data
        vm.contacts = ChatsService.conversations;
        vm.chats = ChatsService.chats;
        vm.user = User.data;
        vm.leftSidenavView = false;
        vm.rightSidenavView = false;
        vm.chat = undefined;
        vm.botId = "KwiklyId";
        
        // Methods
        vm.getChat = getChat;
        vm.toggleSidenav = toggleSidenav;
        vm.toggleLeftSidenavView = toggleLeftSidenavView;
        vm.toggleRightSidenavView = toggleRightSidenavView;

        vm.reply = reply;
        vm.setUserStatus = setUserStatus;
        vm.clearMessages = clearMessages;
        vm.uuid = 'a1cd7ac1-585e-478e-925b-65d17ce62f7d';
        vm.selectedChannel = 'travela';

        // Products
        vm.products = Products.data;

       /* Pubnub.init({
            publish_key: 'pub-c-91e04d8f-4ae7-44b2-8d0c-57bef7b6366f',
            subscribe_key: 'sub-c-5aec2474-3157-11e6-8bc8-0619f8945a4f',
        });

        //////////

        Pubnub.subscribe({
            channel: vm.selectedChannel,
            restore:true,
            message: function(m) {
                //m.content.who = 'contact';
                ChatsService.setContactChat(m);
                getChat(vm.chatContactId)
                // Scroll to the new message
                scrollToBottomOfChat();
                console.log(m)
                //  vm.replyMessage = m;
            }
            //triggerEvents: ['message', 'presence']
        });

      /*  Pubnub.history({
            channel: vm.selectedChannel,
            callback: function(m) {
                console.log(JSON.stringify(m))
                var chats = m[0];
                chats.forEach(ChatsService.setContactChat);
            },
            count: 5,
            // 100 is the default
            reverse: false // false is the default
        });
        
        */
        /**
         * Get Chat by Contact id
         * @param contactId
         */
        function getChat(conversationId)
        {
                        
                vm.chat =  ChatsService.getConversationMessages(conversationId)
            
                vm.chatContactId = conversationId;
                vm.currentConversation = ChatsService.getConversation(conversationId);
                
                // Reset the reply textarea
                resetReplyTextarea();

                // Scroll to the last message
                scrollToBottomOfChat();

                if (!$mdMedia('gt-md'))
                {
                    $mdSidenav('left-sidenav').close();
                }

                // Reset Left Sidenav View
                vm.toggleLeftSidenavView(false);
            
        }

        /**
         * Reply
         */
        function reply($event)
        {
            // If "shift + enter" pressed, grow the reply textarea
            if ($event && $event.keyCode === 13 && $event.shiftKey)
            {
                vm.textareaGrow = true;
                return;
            }

            // Prevent the reply() for key presses rather than the"enter" key.
            if ($event && $event.keyCode !== 13)
            {
                return;
            }

            // Check for empty messages
            if (vm.replyMessage === '')
            {
                resetReplyTextarea();
                return;
            }

            // Message
            var message = {
                who: 'team',
                message: vm.replyMessage,
                time: new Date().toISOString(),
                userId:vm.currentConversation.recipientId,
                conversationId:vm.currentConversation.conversationId,
                channelId:vm.currentConversation.channelId,
                botId:vm.currentConversation.botId,
                serviceurl: vm.currentConversation.serviceurl
            };

            // Add the message to the chat
            //vm.chat.push(message);

            // Update Contact's lastMessage
            //vm.contacts.getById(vm.chatContactId).lastMessage = message;

            // Reset the reply textarea
            resetReplyTextarea();

            // Scroll to the new message
            scrollToBottomOfChat();

            ChatsService.replyToConversation(vm.chatContactId, message);
          /*  var currentContact = vm.contacts.getById(vm.chatContactId);
            Pubnub.publish({
                channel: 'travela',
                message: {
                    id: vm.chatContactId,
                    name: currentContact.name,
                    isTeamReply: true,
                    reply: message,
                    conversationId: currentContact.conversationId,
                    channelId: currentContact.channelId
                },
                callback: function(m) {
                    console.log(m);
                }
            });*/

        }

        /**
         * Clear Chat Messages
         */
        function clearMessages()
        {
            vm.chats[vm.chatContactId] = vm.chat = [];
            vm.contacts.getById(vm.chatContactId).lastMessage = null ;
        }

        /**
         * Reset reply textarea
         */
        function resetReplyTextarea()
        {
            vm.replyMessage = '';
            vm.textareaGrow = false;
        }

        /**
         * Scroll Chat Content to the bottom
         * @param speed
         */
        function scrollToBottomOfChat()
        {
            $timeout(function()
            {
                var chatContent = angular.element($document.find('#chat-content'));

                chatContent.animate({
                    scrollTop: chatContent[0].scrollHeight
                }, 400);
            }, 0);

        }

        /**
         * Set User Status
         */
        function setUserStatus(status)
        {
            vm.user.status = status;
        }

        /**
         * Toggle sidenav
         *
         * @param sidenavId
         */
        function toggleSidenav(id)
        {
            $mdSidenav(id).toggle();
        }

        /**
         * Toggle Left Sidenav View
         *
         * @param view id
         */
        function toggleLeftSidenavView(id)
        {
            vm.leftSidenavView = id;
        }

      function toggleRightSidenavView(id)
      {
        vm.rightSidenavView = id;
        //$mdSidenav(id).toggle();
        $mdSidenav('right-sidenav').toggle();
      }

        /**
         * Array prototype
         *
         * Get by id
         *
         * @param value
         * @returns {T}
         */
        Array.prototype.getById = function(value)
        {
            return this.filter(function(x)
            {
                return x.id === value;
            })[0];
        }
        ;
    }
})();
