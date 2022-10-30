# Introduction 
Uploader Service Api

```sh
git branch -m master main
git fetch origin
git branch -u origin/main main
git remote set-head origin -a
git pull
```

# Run sql server locally (docker)

```sh
docker pull mcr.microsoft.com/mssql/server
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=password@1" -p 1433:1433 --name sql -d mcr.microsoft.com/mssql/server:2017-latest

```

# Local development and migrations

```sh



dotnet ef migrations add InitialCreate
dotnet ef database update InitialCreate

docker build .

```

# Run locally using dapr
```sh
dapr run --app-ssl --app-port 2000 --app-id uploader --app-protocol http --dapr-http-port 2500 --components-path ../../dapr/components -- dotnet run

cd Evolution.Uploader/src/Uploader.Api
dapr run --app-port 2000 --app-id uploader --dapr-http-port 2500 --components-path ../../dapr/components -- dotnet run

```

# Run in K8s
```sh
#spin up the infra
#from https://github.com/cloud-first-approach/Evolution.infra/tree/main/deploy/k8s/infra/overlays/dev
kubectl apply -k deploy/k8s/infra/overlays/dev

kubectl apply -k deploy/k8s/uploader/overlays/dev

#check port-forward 
kubectl port-forward svc/uploader-api-cluster-ip 80 -n evolution

#deploy the service
kubectl delete -k deploy/k8s/uploader/overlays/dev

#deploy the service
kubectl delete -k deploy/k8s/infra/overlays/dev

#deploy the service
helm uninstall redis

```
