# -Web-application-for-visualization-Kubernetes-cluster


This project is for visualizate Kubernetes cluster using ASP.NET + Vue3.js

Resources that are working now:
Pod, 
Deployment, 
ConfigMap,
CronJob,
DaemonSet,
Ingress,
Job,
LimitRange,
NetworkPolicy,
PersistentVolumeClaim,
ReplicaSet,
ResourceQuota,
Secret,
Service,
StatefulSet,

Real time updation I've created using k8s library( created watchers for each resource). 

Problem that was mark in this library is that Not all auth providers are supported at the moment. You can still connect to a cluster by starting the proxy command:

$ kubectl proxy
Starting to serve on 127.0.0.1:8001

and change configuration inside a service KubernetesClientService. It's not user friendly I'll try to solve this problem




Client:

Created using Vue3.js for a store I use Pinia and for visualization I use d3.js. I guess after 2-3 week it'll be done.


Also I want to create Helm/charts that it works inside a cluster
