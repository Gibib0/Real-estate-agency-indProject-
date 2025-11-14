using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq; // Потрібно для .Any()

namespace Integration
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RealEstateDbContext>
    {
        public RealEstateDbContext CreateDbContext(string[] args)
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            DirectoryInfo solutionRoot = new DirectoryInfo(currentDirectory);

            while (solutionRoot != null && !solutionRoot.GetFiles("*.sln").Any())
            {
                solutionRoot = solutionRoot.Parent;
            }

            if (solutionRoot == null)
            {
                throw new DirectoryNotFoundException("Could not find the solution root (.sln file). Ensure this project is part of a solution.");
            }

            string presentationProjectPath = Path.Combine(solutionRoot.FullName, "Presentation");

            if (!Directory.Exists(presentationProjectPath))
            {
                throw new DirectoryNotFoundException($"Could not find the Presentation project directory at the expected path: {presentationProjectPath}");
            }

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(presentationProjectPath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var builder = new DbContextOptionsBuilder<RealEstateDbContext>();
            var connectionString = configuration.GetConnectionString("RealEstateDb");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'RealEstateDb' not found in Presentation/appsettings.json");
            }

            builder.UseSqlite(connectionString);

            return new RealEstateDbContext(builder.Options);
        }
    }
}