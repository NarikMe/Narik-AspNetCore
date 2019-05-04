using System.Collections.Generic;

namespace Narik.Common.Data.DomainService
{
    public class EntityUpdateFiledsInfo
    {
        public EntityUpdateFiledsInfo(List<string> preventFromUpdateFileds, List<string> onlyUpdateFileds)
        {
            PreventFromUpdateFileds = preventFromUpdateFileds;
            OnlyUpdateFileds = onlyUpdateFileds;
        }

        public EntityUpdateFiledsInfo():this(null,null)
        {
           
        }

        public EntityUpdateFiledsInfo(List<string> preventFromUpdateFileds):this(preventFromUpdateFileds,null)
        {
            
        }

        public List<string> PreventFromUpdateFileds { get; set; }
        public List<string> OnlyUpdateFileds { get; set; }
    }
}
