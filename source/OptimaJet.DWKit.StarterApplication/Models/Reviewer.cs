﻿using System.Linq;
using System.Collections.Generic;
using JsonApiDotNetCore.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptimaJet.DWKit.StarterApplication.Models
{
    public class Reviewer : Identifiable
    {
        [Attr("name")]
        public string Name { get; set; }

        [Attr("email")]
        public string Email { get; set; }

        [HasOne("project")]
        public virtual Project Project { get; set; }
        public int ProjectId { get; set; }
    }
}
