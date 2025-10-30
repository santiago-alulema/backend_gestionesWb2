﻿using gestiones_backend.Class;
using gestiones_backend.ConfigurationsMapper;
using gestiones_backend.Context;
using gestiones_backend.helpers;
using gestiones_backend.Interfaces;
using gestiones_backend.Services;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<CustomExceptionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.ReferenceHandler = null;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("http://localhost:5173", "http://localhost:5176", "http://207.180.205.100:5173", "http://207.180.205.100", "http://localhost:5174")
             .WithExposedHeaders("message")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<GestioneService>();
builder.Services.AddScoped<CompromisosPagoService>();
builder.Services.AddScoped<CustomExceptionFilter>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IReportesEmpresaService, ReportesEmpresaService>();
builder.Services.AddScoped<IPagosService, PagosService>();
builder.Services.AddScoped<ITareasService, TareasService>();
builder.Services.AddScoped<IGestionesService, GestionesService>();
builder.Services.AddScoped<IMensajesWhatsapp, MensajesWhatsappServices>();
builder.Services.AddScoped<IGestionarImagenes, GestionarImagenesServices>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddHostedService<MarcarIncumplidosDailyWorker>();
builder.Services.AddSingleton<SftpDownloadService>();
builder.Services.AddSingleton<ZipExtractService>();
builder.Services.AddSingleton<FolderCleanService>();
builder.Services.AddScoped<DeudoresImportService>();
builder.Services.AddHttpClient<IWhatsappNodeClient, WhatsappNodeClient>();
builder.Services.AddHttpClient();

// Options
builder.Services.Configure<SftpOptions>(builder.Configuration.GetSection("Sftp"));
builder.Services.Configure<TrifocusExportOptions>(builder.Configuration.GetSection("TrifocusExport"));

// PgConn (TRANSIENT por seguridad: no compartir conexión entre hilos)
//builder.Services.AddTransient<gestiones_backend.DbConn.PgConn>();

// Servicio principal
builder.Services.AddSingleton<ITrifocusExcelUploader, TrifocusExcelUploader>();
builder.Services.AddHostedService<TrifocusDailyWorker>();

builder.Services.AddMapster();

var mapsterConfig = TypeAdapterConfig.GlobalSettings;

ConfigTelefonoDeudor.Register(mapsterConfig);
ConfigDeudasCliente.Register(mapsterConfig);


builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 200 * 1024 * 1024; // 200 MB
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
}

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
