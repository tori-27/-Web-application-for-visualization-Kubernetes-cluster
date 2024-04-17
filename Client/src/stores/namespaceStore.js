import { defineStore } from 'pinia';
import signalRService from "../services/signalRService.js";
import { HubConnectionState } from "@microsoft/signalr";

export const useNamespaceStore = defineStore('namespace', {
    state: () => ({
        namespaces: [],
        selectedNamespace: '',
    }),
    actions: {
        async fetchNamespaces() {
            try {
                if (!signalRService.connection) {
                    await signalRService.startConnection();
                }
                if (signalRService.connection.state === HubConnectionState.Connected) {
                    await signalRService.connection.invoke("JoinToNamespaces");
                    signalRService.registerNamespacesUpdateCallback(this.setNamespaces);
                }
            } catch (error) {
                console.error('Error during namespaces fetching: ', error);
            }
        },
        setNamespaces(namespaces){
            this.namespaces = namespaces;
            if(!this.namespaces.includes(this.selectedNamespace)){
                this.selectedNamespace = this.namespaces[0] || '';
            }
        },

        changeNamespace(namespace){
            if(this.selectedNamespace){
                signalRService.leaveNamespaceGroup(this.selectedNamespace);
            }
            this.selectedNamespace = namespace;
            signalRService.joinNamespaceGroup(namespace);
        },
        handleNamespaceUpdate(update){
            const {type, namespace} = update;
            const namespaceName = namespace.metadata.name;

            switch(type){
                case 'Added':
                    if(!this.namespaces.includes(namespaceName)){
                        this.namespaces.push(namespaceName);
                    }
                    break;
                case 'Deleted':
                    this.namespaces = this.namespaces.filter(ns => ns !== namespaceName);
                    break;
                case 'Modified':
                    //create modified logic
                    break;
            }
            if(!this.namespaces.includes(this.selectedNamespace)){
                this.selectedNamespace = this.namespaces[0] || '';
                this.changeNamespace(this.selectedNamespace);
            }
        }
    }
});