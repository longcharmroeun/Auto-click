using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class GamingPoint
    {
        public int new_gaming_point_types__id { get; set; }
        public int points { get; set; }
    }

    public class Visibility
    {
        public bool is_design { get; set; }
        public bool is_vacancy { get; set; }
        public bool is_signal { get; set; }
        public bool is_promo { get; set; }
        public bool is_test { get; set; }
        public bool is_email_verified { get; set; }
    }

    public class UserInfo
    {
        public int student_id { get; set; }
        public int group_id { get; set; }
        public int current_group_id { get; set; }
        public string full_name { get; set; }
        public object university_group { get; set; }
        public List<GamingPoint> gaming_points { get; set; }
        public List<object> spent_gaming_points { get; set; }
        public int achieves_count { get; set; }
        public int stream_id { get; set; }
        public string stream_name { get; set; }
        public string group_name { get; set; }
        public int level { get; set; }
        public string photo { get; set; }
        public Visibility visibility { get; set; }
    }
}
