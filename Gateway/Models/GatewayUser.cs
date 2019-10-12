﻿using Aiursoft.Gateway.Data;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GatewayUser : AiurUserBase
    {
        [InverseProperty(nameof(OAuthPack.User))]
        public IEnumerable<OAuthPack> Packs { get; set; }
        [InverseProperty(nameof(AppGrant.User))]
        public IEnumerable<AppGrant> GrantedApps { get; set; }
        [InverseProperty(nameof(UserEmail.Owner))]
        public IEnumerable<UserEmail> Emails { get; set; }
        [InverseProperty(nameof(AuditLogLocal.User))]
        public IEnumerable<AuditLogLocal> AuditLogs { get; set; }

        public virtual string SMSPasswordResetToken { get; set; }

        [JsonProperty]
        [NotMapped]
        public override bool EmailConfirmed => Emails?.Any(t => t.Validated) ?? false;
        [JsonProperty]
        [NotMapped]
        public override string Email => Emails?
            .OrderByDescending(t => t.Validated)
            .ThenByDescending(t => t.Priority)
            .First()?
            .EmailAddress ?? string.Empty;

        public async virtual Task GrantTargetApp(GatewayDbContext dbContext, string appId)
        {
            if (!await HasAuthorizedApp(dbContext, appId))
            {
                var appGrant = new AppGrant
                {
                    AppID = appId,
                    GatewayUserId = Id
                };
                dbContext.LocalAppGrant.Add(appGrant);
                await dbContext.SaveChangesAsync();
            }
        }

        public async virtual Task<OAuthPack> GeneratePack(GatewayDbContext dbContext, string appId)
        {
            var pack = new OAuthPack
            {
                Code = Math.Abs(Guid.NewGuid().GetHashCode()),
                UserId = Id,
                ApplyAppId = appId
            };
            dbContext.OAuthPack.Add(pack);
            await dbContext.SaveChangesAsync();
            return pack;
        }

        public async Task<bool> HasAuthorizedApp(GatewayDbContext dbContext, string appId)
        {
            return await dbContext.LocalAppGrant.AnyAsync(t => t.AppID == appId && t.GatewayUserId == Id);
        }
    }

    public class UserEmail : AiurUserEmail
    {
        [JsonIgnore]
        public string OwnerId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public GatewayUser Owner { get; set; }
        [JsonIgnore]
        public string ValidateToken { get; set; }
        [JsonIgnore]
        public DateTime LastSendTime { get; set; } = DateTime.MinValue;
        [JsonIgnore]
        public int Priority { get; set; }
    }
}
