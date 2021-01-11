using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

class Program
{
	static void Main(string[] args)
	{
		WebHost.CreateDefaultBuilder(args)
			.Configure(config =>
			{
				config.UseDefaultFiles()
					.UseStaticFiles();
			})
			.UseWebRoot("wwwroot").Build().Run();
	}
}
