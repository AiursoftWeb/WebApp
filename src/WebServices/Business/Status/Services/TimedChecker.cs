﻿using Aiursoft.Scanner.Interfaces;
using Aiursoft.Status.Data;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.Status.Services
{
    public class TimedChecker : IHostedService, IDisposable, ISingletonDependency
    {
        private Timer _timer;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public TimedChecker(
            ILogger<TimedChecker> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!EntryExtends.IsProgramEntry())
            {
                _logger.LogInformation("Skip cleaner in development environment.");
                return Task.CompletedTask;
            }
            _logger.LogInformation("Timed Background Service is starting.");
            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(10));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                _logger.LogInformation("Cleaner task started!");
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MonitorDataProvider>();
                    var http = scope.ServiceProvider.GetRequiredService<HttpService>();
                    await AllCheck(dbContext, http);
                }
                _logger.LogInformation("Cleaner task finished!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cleaner crashed!");
            }
        }

        private async Task AllCheck(MonitorDataProvider dbContext, HttpService http)
        {
            foreach (var item in dbContext.MonitorRules)
            {
                _logger.LogInformation($"Checking status for: {item.ProjectName}");
                try
                {
                    var content = await http.Get(new AiurUrl(item.CheckAddress));
                    var success = content.Contains(item.ExpectedContent);
                    if (!success)
                    {
                        var errorMessage = $"Status check for {item.ProjectName} did not pass. Expected: {item.ExpectedContent}. Got content: {content}";
                        _logger.LogError(errorMessage);
                    }
                    item.LastHealthStatus = success;
                    item.LastCheckTime = DateTime.UtcNow;
                }
                catch (Exception e)
                {
                    var errorMessage = $"Status check for {item.ProjectName} did not pass. Expected: {item.ExpectedContent}. Got error: {e.Message}";
                    _logger.LogError(errorMessage);
                    item.LastHealthStatus = false;
                    item.LastCheckTime = DateTime.UtcNow;
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
