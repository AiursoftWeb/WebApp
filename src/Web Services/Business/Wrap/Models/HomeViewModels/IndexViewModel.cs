﻿using Aiursoft.SDKTools.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Wrap.Models.HomeViewModels
{
    public class IndexViewModel
    {
        [Required]
        [ValidDomainName]
        [MaxLength(50)]
        [MinLength(5)]
        public string NewRecordName { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 7);

        [Required]
        [MaxLength(1000)]
        [MinLength(5)]
        [Url]
        public string Url { get; set; }
    }
}
