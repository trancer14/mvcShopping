using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcProjee.Models
{
    public class yorum
    {
        public int yorumid { get; set; }
        public string yorumuserad { get; set; }
        public string yorumurunad { get; set; }
        public string yorummetin { get; set;}
        public int yorumonay { get; set; }
        public string yorumtarih { get; set; }
        public int yorumpuan { get; set; }
        public int yorumaltid { get; set; }
    }
}