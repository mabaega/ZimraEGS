using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Localization;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.Json.Serialization;

namespace ZimraEGS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var builder = WebApplication.CreateBuilder(args);
            // Configure JSON options
            builder.Services.Configure<JsonOptions>(options =>
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull);

            //builder.Services.AddControllers()
            //.AddNewtonsoftJson(options =>
            //{
            //    options.SerializerSettings.DateParseHandling = DateParseHandling.None; // Prevent automatic date parsing
            //    options.SerializerSettings.Converters.Add(new ZimraEGS.ApiClient.Helpers.LocalDateTimeConverter()); // Add your custom converter
            //    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; // Optional: ignore null values
            //});

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });


            // Add services to the container.
            builder.Services.AddControllersWithViews();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("AllowAllOrigins");

            // Use request localization
            var defaultCulture = new CultureInfo("en-US");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = new List<CultureInfo> { defaultCulture },
                SupportedUICultures = new List<CultureInfo> { defaultCulture }
            };
            app.UseRequestLocalization(localizationOptions);


            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
