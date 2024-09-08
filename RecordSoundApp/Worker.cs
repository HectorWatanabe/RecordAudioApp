﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace RecordSoundApp
{
    public class Worker : BackgroundService
    {
        private readonly TcpListener _listener;
        private readonly RecordCommand _command;

        public Worker(IConfiguration configuration)
        {
            string port = configuration["ApplicationSettings:Port"] ?? throw new Exception("Port is required.");
            _listener = new TcpListener(IPAddress.Any, int.Parse(port));
            _command = new RecordCommand();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _listener.Start();

            Console.WriteLine("Servicio de registro de audio disponible en el puerto 4500.");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync(stoppingToken);

                    var buffer = new byte[1024];

                    var stream = client.GetStream();

                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, stoppingToken)) != 0)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        if (string.IsNullOrEmpty(message)) continue;

                        var data = JsonSerializer.Deserialize<Request>(message) ?? throw new Exception($"Deserialize error: {message}");

                        try
                        {
                            var response = _command.Execute(new RequestCommand()
                            {
                                Command = data.Command,
                                Path = data.Path,
                                CancellationToken = stoppingToken,
                                Stream = stream
                            });

                            await Utils.SendResponse(stream, response, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            await Utils.SendResponse(stream, new { Code = 500, ex.Message }, stoppingToken);
                        }
                    }
                } 
                catch (Exception error)
                {
                    Console.Write(error.Message);
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _listener.Stop();

            Console.WriteLine("Servicio de registro de audio detenido en el puerto 4500.");

            return base.StopAsync(cancellationToken);
        }
    }
}
