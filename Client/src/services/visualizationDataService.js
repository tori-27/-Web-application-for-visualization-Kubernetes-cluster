import {useResourcesStore} from "../stores/resourcesStore.js";

export default {

    transformedDataCache: null,

    getTransformedData(){

        const store = useResourcesStore();
        const rawData = store.resources;
        let nodes = this.getNodes(rawData);
        let links = this.getLinks(rawData, nodes);
        const transformedData = {nodes, links};
        this.transformedDataCache = transformedData;
        console.log("from data anal")
        console.log(transformedData)
        return transformedData;
    },



    getNodes(rawData){
        let nodes = [];
        Object.entries(rawData).forEach(([resourceType, resourceWrapper]) => {

            const resourceItems = resourceWrapper.items;
            if(resourceItems && resourceItems.length > 0){
                resourceItems.forEach((resource) => {
                    const {metadata} = resource;
                    const nodeId = `${resourceType}-${metadata.name}`;
                    nodes.push({
                        id: nodeId,
                        label: metadata.name,
                        type: resourceType,
                    })
                })
            }
        })
        return nodes;
    },

    // getLinks(rawData){
    //     let links = [];
    //     Object.entries(rawData).forEach(([resourceType, resourceWrapper]) => {
    //         const resourceItems = resourceWrapper.items;
    //         if(resourceItems && resourceItems.length > 0){
    //             resourceItems.forEach((resource) => {
    //                 const {metadata} = resource;
    //                 if (metadata.ownerReferences && metadata.ownerReferences.length > 0){
    //                     metadata.ownerReferences.forEach((owner) =>{
    //                         const ownerType = owner.kind;
    //                         const ownerName = owner.name;
    //                         const childName = metadata.name;
    //
    //                         const linkId = `link-${ownerType}-${ownerName}-to-${resourceType}-${childName}`;
    //
    //                         links.push({
    //                             id: linkId,
    //                             source: `${ownerType}-${ownerName}`,
    //                             target: `${resourceType}-${childName}`,
    //                         });
    //                     });
    //                 }
    //             });
    //         }
    //     });
    //
    //     return links;
    // }


    getLinks(rawData, nodes) {
        let links = [];
        const nodeMap = new Map(nodes.map(node => [node.id, node]));
        Object.entries(rawData).forEach(([resourceType, resourceWrapper]) => {
            const resourceItems = resourceWrapper.items;
            if (resourceItems && resourceItems.length > 0) {
                resourceItems.forEach((resource) => {
                    const {metadata} = resource;
                    if (metadata.ownerReferences && metadata.ownerReferences.length > 0) {
                        metadata.ownerReferences.forEach((owner) => {
                            const sourceId = `${owner.kind}-${owner.name}`;
                            const targetId = `${resourceType}-${metadata.name}`;
                            if (nodeMap.has(sourceId) && nodeMap.has(targetId)) {
                                links.push({
                                    source: nodeMap.get(sourceId),
                                    target: nodeMap.get(targetId),
                                });
                            }
                        });
                    }
                });
            }
        });
        return links;
    }

}