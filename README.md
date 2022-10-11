# Introduction 
Uploader Service
git branch -m master main
git fetch origin
git branch -u origin/main main
git remote set-head origin -a

run :

dapr run --app-port 2000 --app-id uploader --app-protocol http --dapr-http-port 2501 --components-path ../../dapr/components -- dotnet run


### Getting Started
```sh

#STEP 1
kind create cluster

#STEP 2
dapr init -k

#STEP 3


#STEP 4 (Redis)

> helm repo add bitnami https://charts.bitnami.com/bitnami
> helm repo update
> helm install redis bitnami/redis --set image.tag=6.2

#STEP 4 (step infra on kubernetes)
kubectl apply -k deploy/k8s/infra/overlays/dev

kubectl get pods -n evolution

kubectl apply -f deploy/k8s/services

kubectl get pods -n evolution

kubectl port-forward svc/uploader-api-cluster-ip 2001 -n evolution



kubectl delete -f deploy/k8s/services
kubectl delete -k deploy/k8s/infra/overlays/dev
helm uninstall redis

# FLUX





```

# Flux & fluxctl

```sh
kubectl create ns flux-system

SET GITHUB_USER=iamsourabh-in

flux bootstrap github --owner=%GITHUB_USER% --repository=cloud-first-approach/Evolution.Uploader --branch=main --path=./deploy/K8s/infra/overlays/dev --personal

#setup Flux in K8 for pulling you repo for sync
fluxctl install --git-user=iamsourabh-in --git-email=iamsourabh-in@users.noreply.github.com --git-url=git@github.com:iamsourabh-in/Evolution --git-path=deploy/K8s/infra/overlays/dev --git-branch=flux --namespace=flux-system | kubectl apply -f -

kubectl -n flux-system rollout status deployment/flux

fluxctl list-workloads --k8s-fwd-ns flux-system

fluxctl identity --k8s-fwd-ns flux-system


https://github.com/marcel-dempers/docker-development-youtube-series/settings/keys/new

fluxctl sync --k8s-fwd-ns flux-system

  annotations:
    fluxcd.io/tag.example-app: semver:~1.0
    fluxcd.io/automated: 'true'

fluxctl policy -w default:deployment/example-deploy --tag "example-app=1.0.*"

```
