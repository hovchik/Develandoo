using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Develandoo.Infrastructure
{
    public static class Counter
    {
        private static int count;
        public static int Count => ++count;
    }
}