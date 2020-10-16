var chatComponent;

function setupChat(chatId) {
    chatComponent = document.getElementById(chatId);
}

function onMessageAdd() {
    setTimeout(() => {
        chatComponent.scrollTop = chatComponent.scrollHeight;
    }, 2);
}