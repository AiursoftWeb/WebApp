﻿using Aiursoft.Probe.Data;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe.SiteAddressModels;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class SiteController : Controller
    {
        private readonly ProbeDbContext _dbContext;
        private readonly ACTokenManager _tokenManager;

        public SiteController(
            ProbeDbContext dbContext,
            ACTokenManager tokenManager)
        {
            _dbContext = dbContext;
            _tokenManager = tokenManager;
        }

        [HttpPost]
        public async Task<JsonResult> DeleteApp(DeleteAppAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            if (appid != model.AppId)
            {
                return this.Protocol(ErrorType.Unauthorized, "The app you try to delete is not the access token you granted!");
            }
            var target = await _dbContext.Apps.FindAsync(appid);
            if (target != null)
            {
                _dbContext.Folders.Delete(t => target.Sites.Select(p=>p.FolderId).Contains(t.Id));
                _dbContext.Sites.Delete(t => t.AppId == appid);
                _dbContext.Apps.Remove(target);
                await _dbContext.SaveChangesAsync();
                return this.Protocol(ErrorType.Success, "Successfully deleted that app and all sites.");
            }
            return this.Protocol(ErrorType.HasDoneAlready, "That app do not exists in our database.");
        }

        //public async Task<JsonResult> ViewMyBuckets(ViewMyBucketsAddressModel model)
        //{
        //    var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
        //    var appLocal = await _dbContext.Apps.SingleOrDefaultAsync(t => t.AppId == appid);
        //    if (appLocal == null)
        //    {
        //        appLocal = new OSSApp
        //        {
        //            AppId = appid,
        //            MyBuckets = new List<Bucket>()
        //        };
        //        _dbContext.Apps.Add(appLocal);
        //        await _dbContext.SaveChangesAsync();
        //    }

        //    var buckets = await _dbContext
        //        .Bucket
        //        .Include(t => t.Files)
        //        .Where(t => t.BelongingAppId == appid)
        //        .ToListAsync();
        //    buckets.ForEach(t => t.FileCount = t.Files.Count());
        //    var viewModel = new ViewMyBucketsViewModel
        //    {
        //        AppId = appLocal.AppId,
        //        Buckets = buckets,
        //        Code = ErrorType.Success,
        //        Message = "Successfully get your buckets!"
        //    };
        //    return Json(viewModel);
        //}
    }
}
