﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("ReceiverMethodOnServer", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("ReceivedMessageFromServer", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " > " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("ReceivedMessageFromServerPage", function (message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = "Devin: " + " > " + msg;

    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});