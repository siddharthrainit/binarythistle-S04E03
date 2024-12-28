Docker command
----------------------------------------------------------------------------------------
            docker build -t <user Id>>/<container name> .
            docker push <user Id>>/<container name>            
----------------------------------------------------------------------------------------


kubernetes
----------------------------------------------------------------------------------------
kubectl apply -f <yaml file>
kubectl get deployments
kubectl rollout restart deployment <deployment name>
kubectl get svc
kubectl get pods
kubectl delete service <service name>
kubectl create secret generic mssql --from-literal=SA_PASSWORD="<your strong password>"

kubectl logs <pod name> -c fix-permissions
# -c fix-permissions:
# The -c flag specifies which container's logs you want to see.
# fix-permissions is the name of the container in the pod.
# If a pod has multiple containers, you need to specify the container explicitly with -c, or you will get an error.

kubectl exec -it <pod name> -- nslookup mssql-clusterip-srvcls

# kubectl exec :- Executes a command inside a running pod in the Kubernetes cluster.
# -i : Interactive mode, keeps the session open for input.
# -t : Allocates a pseudo-terminal for the command.
# -- : Indicates the end of kubectl options and the start of the command to be run inside the pod.
# nslookup : A network utility used to query DNS servers and resolve domain names or service names into IP addresses.
# mssql-clusterip-srvcls :  This is the DNS name being queried.