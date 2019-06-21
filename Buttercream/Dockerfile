#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 54627
EXPOSE 44301

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["Buttercream/Buttercream.csproj", "Buttercream/"]
RUN dotnet restore "Buttercream/Buttercream.csproj"
COPY . .
WORKDIR "/src/Buttercream"
RUN dotnet build "Buttercream.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Buttercream.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
CMD ["dotnet", "Buttercream.dll"]

