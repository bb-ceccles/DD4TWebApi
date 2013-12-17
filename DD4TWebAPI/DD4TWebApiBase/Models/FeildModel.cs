using System.Collections.Generic;

namespace DD4TWebApiBase.Models
{
    public class FeildModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public List<ComponentModel> RelatedFeilds { get; set; }
    }
}