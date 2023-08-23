FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /publish
COPY src .
RUN dotnet restore ./Template.Presentation/Template.Presentation.csproj
RUN dotnet publish ./Template.Presentation/Template.Presentation.csproj -c Release --output ./out 

FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal as release
WORKDIR /app
COPY --from=build /publish/out .
ENV ASPNETCORE_URLS "http://0.0.0.0:5000/"
ENTRYPOINT ["dotnet", "Template.Presentation.dll"]