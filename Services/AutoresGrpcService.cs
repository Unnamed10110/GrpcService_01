using Grpc.Core;
using GrpcService_01;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace GrpcService_01.Services
{
    public class AutoresGrpcService : AutoresGrpc.AutoresGrpcBase
    {
        private readonly ILogger<AutoresGrpcService> logger;

        public AutoresGrpcService(ILogger<AutoresGrpcService> logger)
        {
            this.logger = logger;
        }

        public override async Task<ReplyModel> Autores(RequestModel request, ServerCallContext context)
        {
            var resultado = new List<AutoresModel>();
            var connString = "Server=localhost;Port=5432;Database=WebApiAutores;User Id=postgres;Password=634510;";


            await using (var conn = new NpgsqlConnection(connString))
            {
                await conn.OpenAsync();

                var query = "SELECT * FROM public.\"Autores\"";

                var cmd = new NpgsqlCommand(query, conn);
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        resultado.Add(new AutoresModel
                        {
                            Id = (int)reader.GetValue(0),
                            NombreCompleto = (string)reader.GetValue(1)

                        });
                    }
                }
                
            }
            

            ReplyModel replyModel = new();

            replyModel.Autor.AddRange(resultado.OrderBy(x => x.Id));

            return replyModel;
        }
    }
}
