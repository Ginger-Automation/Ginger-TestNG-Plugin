﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GingerTestNgPlugin
{
    public class TestNGTestParameter
    {
        public string Name { get; set; } //using get;set; this attribute will be shown as input value in Ginger side

        public string ParentNodeName { get; set; } //using get;set; this attribute will be shown as input value in Ginger side

        public string Value { get; set; } //using get;set; this attribute will be shown as input value in Ginger side
    }
}
