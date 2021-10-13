# Raimun

A sample Task for Raimun Company

## Getting Started

To run Raimnu in development mode; Just use steps below:

1. Install `dotnet`, `docker`, in your system.
2. Clone the project `https://github.com/modarreszadeh/Raimun`.
3. Make development environment ready using commands below;

  ```bash
  git clone https://github.com/modarreszadeh/Raimun && cd Raimun/src/Web
  sudo docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.9-management
  ```

4. Run `Raimnu` using `dotnet run`
5. Go to [http://localhost:5001](http://localhost:5001) to see your Raimun version.

## Run On Windows

If You're On A Windows Machine , Make Environment Ready By Following Steps Below:
1. Install `dotnet`, `docker` 
2. Clone the project using:  `git clone https://github.com/modarreszadeh/Raimun`.
3. Make Environment Ready Like This:
``` Command Prompt
cd Raimun/src/Web
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.9-management
```
4. Run `Raimun` using `dotnet run`
5. Go to [http://localhost:5001](http://localhost:5001) to see your Raimun version.
