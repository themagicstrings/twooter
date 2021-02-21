FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY ./publish /publish
WORKDIR /publish
EXPOSE 5000/tcp
ENTRYPOINT ["dotnet", "Api.dll"]