using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PcAnketProject.Core.Dto
{
    public class PusulaLogin
    {

    }
    public class PusulaLoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string apikey { get; set; }
    }
    public class Error
    {
        public string errorCode { get; set; }
        public string fieldName { get; set; }
        public string message { get; set; }
    }

    public class ResponseStatus
    {
        public string errorCode { get; set; }
        public string message { get; set; }
        public string stackTrace { get; set; }
        public List<Error> errors { get; set; }
    }

    public class PusulaLoginResponse
    {
        public string adSoyad { get; set; }
        public string nufusID { get; set; }
        public string email { get; set; }
        public bool staff { get; set; }
        public string ldapUsername { get; set; }
        public string tcNo { get; set; }
        public ResponseStatus responseStatus { get; set; }
    }

}