docker build -f .\src\CFProxy.API\Dockerfile -t flexlabs.cfproxy.api .
docker tag flexlabs.cfproxy.api artiomchi/flexlabs.cfproxy.api
docker push artiomchi/flexlabs.cfproxy.api
