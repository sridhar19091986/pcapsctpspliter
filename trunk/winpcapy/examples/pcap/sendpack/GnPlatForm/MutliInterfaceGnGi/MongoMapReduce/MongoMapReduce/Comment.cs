﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDBBlog.Tester
{
    public class Comment
    {
        public DateTime TimePosted { get; set; }

        public string Email { get; set; }

        public string Body { get; set; }
    }
}
