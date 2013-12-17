using System.Collections.Generic;

namespace DD4TWebApiBase.Models
{
    public class ComponentModel
    {
        public string Title { get; set; }
        public string TcmId { get; set; }

        public List<FeildModel> Feilds { get; set; }

    }
}