import {defineStore} from "pinia";
import signalRService from "../services/signalRService.js";


export const useResourcesStore = defineStore('resources', {
    state: () => ({
        resources: {
            Pods: [],
            ConfigMaps: [],
            CronJobs: [],
            DaemonSets: [],
            Deployments: [],
            Ingresses: [],
            Jobs: [],
            LimitRanges: [],
            NetworkPolicies: [],
            PersistentVolumesClaim: [],
            ReplicaSets: [],
            ResourceQuotas: [],
            Secrets: [],
            Services: [],
            StatefulSets: [],
        }
    }),
    actions: {
        setResources(newResources) {
            Object.entries(newResources).forEach(([key, value]) => {
                if (key in this.resources) {
                    this.resources[key] = value;
                }
            });
            console.log("data from store");
            console.log(this.resources);
        },

        fetchResources() {
            signalRService.registerNamespaceResourcesUpdateCallback((resources) => {
                this.setResources(resources);
            });
        },


        initializeUpdateListener() {
            const resourceTypes = [
                'Deployments',
                'Pods',
                'ConfigMaps',
                'CronJobs',
                'DaemonSets',
                'Ingresses',
                'Jobs',
                'LimitRanges',
                'NetworkPolicies',
                'PersistentVolumesClaim',
                'ReplicaSets',
                'ResourceQuotas',
                'Secrets',
                'Services',
                'StatefulSets'
            ];
            resourceTypes.forEach(resourceTypeFromClient => {
                signalRService.registerUpdateCallback(`Receive${resourceTypeFromClient}Update`, (data) => {
                    const {type: updateType, resourceTypeFromServer, item: updatedResource} = data;
                    if(resourceTypeFromClient === resourceTypeFromServer){
                        const resourcesItems = this.resources[resourceTypeFromServer]?.items || [];

                        if(updateType === 'Added' || updateType === 'Modified'){
                            const existingIndex = resourcesItems.findIndex(i => i.metadata.name === updatedResource.metadata.name);
                            if(existingIndex === -1){
                                resourcesItems.push(updatedResource);

                            }else{
                                resourcesItems[existingIndex] = updatedResource;
                            }
                        }else if(updateType === 'Deleted'){
                            this.resources[resourceTypeFromServer].items = resourcesItems.filter(i => i.metadata.name !== updatedResource.metadata.name);
                        }
                    }else{
                        console.error(`Expected update for ${resourceTypeFromClient}, but got ${resourceTypeFromServer}`);
                    }
                });
            });
        },
    }
})
