﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace PcAnketProject.Core.Dto
{
    public class ResimYukleDto
    {
        public IFormFile File { get; set; }
    }
}
