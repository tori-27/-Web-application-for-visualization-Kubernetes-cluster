// src/services/signalRService.js
import * as signalR from "@microsoft/signalr";

export default {
    connection: null,

    startConnection() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7254/kubernetesHub")
            .configureLogging(signalR.LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        this.connection.start().then(function () {
            console.log("Connected to SignalR hub");
        }).catch(function (err) {
            return console.error(err.toString());
        });
    },

    joinNamespaceGroup(namespaceName) {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            this.connection.invoke("JoinNamespaceGroup", namespaceName).catch(function (err) {
                return console.error(err.toString());
            });
        }else{
            console.log("SignalR connection is not established")
        }
    },

    leaveNamespaceGroup(namespaceName) {
        if (this.connection) {
            this.connection.invoke("LeaveNamespaceGroup", namespaceName).catch(function (err) {
                return console.error(err.toString());
            });
        }
    },

    registerUpdateCallback(eventName, callback) {
        if (this.connection) {
            this.connection.on(eventName, (data) => {
                console.log(data);
                callback(data);
            });
        }
    },

    registerNamespaceResourcesUpdateCallback(callback) {
        if (this.connection) {
            this.connection.on("ReceiveNamespaceResources", (data)=>{
                callback(data)
            });
        }
    },

    registerNamespacesUpdateCallback(callback) {
        if (this.connection) {
            this.connection.on("ReceiveNamespaces", callback);
        }
    }
};
