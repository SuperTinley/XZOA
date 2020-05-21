using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class ProposalEntity : IEntity<ProposalEntity>
    {
        public int ID { get; set; }
        public int? sequ_no { get; set; }
        public string pro_id { get; set; }
        public string pro_title { get; set; }
        public string pro_dep { get; set; }
        public string pro_man { get; set; }
        public DateTime? pro_date { get; set; }
        public string pro_send_dep { get; set; }
        public string is_pass { get; set; }
        public string reasons { get; set; }
        public string imp_man { get; set; }
        public DateTime? plan_finish_date { get; set; }
        public string eff_che_dep { get; set; }
        public string reward { get; set; }
        public DateTime? plan_che_date { get; set; }
        public string annex { get; set; }
        public DateTime? annex_date { get; set; }
        public string testMan { get; set; }
        public string make_bill_man { get; set; }
        public DateTime? make_bill_date { get; set; }
        public string tag_state { get; set; }
        public DateTime? imp_finish_date { get; set; }
    }
}
