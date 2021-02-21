## Create Personal Access Token at [github.com/settings/tokens](https://github.com/settings/tokens)

`echo <Personal Access Token> | docker login https://docker.pkg.github.com -u <GitHub Username> --password-stdin`

## Create Docker container locally

Dockerfile in App/
```
FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY ./publish /publish
WORKDIR /publish
EXPOSE 5000/tcp
ENTRYPOINT ["dotnet", "Api.dll"]
```

Run `dotnet publish -c Release -o publish` in Api/

`docker build -t twooter .`

Check if the image works `docker run -it --rm -p 8080:80 --name twooter-instance twooter`

`docker tag twooter docker.pkg.github.com/themagicstrings/twooter/twooter`

`docker push docker.pkg.github.com/themagicstrings/twooter/twooter`

*if login error* `echo <Token> | docker login https://docker.pkg.github.com -u <GitHub Username> --password-stdin`

## Pull image and run container on server

`ssh root@<server_ip>`

`docker pull docker.pkg.github.com/themagicstrings/twooter/twooter:latest`

`docker run -it --rm -p 80:80 --name twooter-instance docker.pkg.github.com/themagicstrings/twooter/twooter:latest`
