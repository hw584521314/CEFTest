using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class UserInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public int type { get; set; }
        public DateTime createTime { get; set; }
        public int enable { get; set; }
    }

    public class Tag
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Problem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string URL { get; set; }
        public string answer { get; set; }
        public int level { get; set; }
    }

    public class Session
    {
        public int id { get; set; }
        public int teacherID { get; set; }
        public DateTime createTime { get; set; }
        public DateTime endTime { get; set; }
    }

    public class SessionDetail
    {
        public int id { get; set; }
        public int sessionID { get; set; }
        public int problemID { get; set; }
        public int sortIdx { get; set; }
        public string comment { get; set; }
    }

    public class Submit
    {
        public int id { get; set; }
        public int studentID { get; set; }
        public int sessionID { get; set; }
        public int problemID { get; set; }
        public string code { get; set; }
        public int score { get; set; }
        public DateTime createTime { get; set; }
        public string comment { get; set; }
    }

    public class SessionTemplate
    {

    }
    public class SessionTemplateDetail
    {

    }
}
