# Introduction 
Uploader Service Api

## Key Notes
- The service follows the `Open Api Spec` and `REST` standards.
- The service is configured to run using `kestrel` server on port `2000` 
- The service exposes a health check at `/health` and `/healthz` endpoint.
- The service exposes a swagger endpoint for `/swagger` only in `Development` env.
- The service exposes a metric endpoint `/metricstext` for text based and `/metrics` for protobuf in `prometheus` format.
- The service uses `dapr components`

# Run sql server locally (docker)

```sh
docker pull mcr.microsoft.com/mssql/server
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=password@1" -p 1433:1433 --name sql -d mcr.microsoft.com/mssql/server:2017-latest

```

# Local development and migrations

```sh

#ENV VAR 

AWS_ACCESS_KEY = ASDASDOQWYH128e912***
AWS_SECRET_KEY = eytabsj3eildu23dl32ib,1i2eld23cSDASD*********

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
kubectl apply -k ./Evolution.infra/deploy/k8s/infra/overlays/dev

kubectl apply -k ./Evolution.Uploader/deploy/k8s/uploader/overlays/dev

#check port-forward 
kubectl port-forward svc/uploader-api-cluster-ip 2000:80 -n evolution

# DELETE

#deploy the service
kubectl delete -k deploy/k8s/uploader/overlays/dev

#deploy the service
kubectl delete -k deploy/k8s/infra/overlays/dev

#deploy the service
helm uninstall redis

```
