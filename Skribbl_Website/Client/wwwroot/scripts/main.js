
playersReferences = {};

addNewReference = function (reference, playerName) {
    playersReferences[playerName] = reference;
};

changeShowBan = function (newState, playerName) {
    dotNetObject = playersReferences[playerName];
    dotNetObject.invokeMethodAsync('ChangeShowBanPlayer', newState);
};
